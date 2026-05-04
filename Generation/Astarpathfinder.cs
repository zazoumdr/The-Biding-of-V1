using System.Collections.Generic;
using UnityEngine;

namespace TheBindingOfV1.Generation
{
    /// <summary>
    /// Implements the A* pathfinding algorithm on a <see cref="NavigationGrid"/>.
    ///
    /// Given a start node and an end node, finds the lowest-cost path between
    /// them through the grid, avoiding BLOCKED cells and preferring horizontal
    /// movement over vertical transitions.
    ///
    /// Movement costs:
    /// - Horizontal (same palier):         1.0
    /// - Vertical adjacent palier:         3.0  (CONNECTOR cells only)
    /// - Vertical direct (stacked rooms):  gap * 5.0
    ///
    /// Heuristic: 3D Manhattan distance with vertical penalty
    ///   h(n) = |dx| + |dz| + |dy| * 3.0
    ///
    /// The returned path is a list of <see cref="AStarNode"/> from start to end
    /// (inclusive), which is then consumed by <see cref="CorridorInstancer"/>
    /// to place corridor prefab segments.
    /// </summary>
    public class AStarPathfinder
    {
        // ── Movement costs ────────────────────────────────────────────

        /// <summary>Cost of moving to a horizontal neighbour on the same palier.</summary>
        private const float COST_HORIZONTAL = 1f;

        /// <summary>Cost of moving to an adjacent palier via a CONNECTOR cell.</summary>
        private const float COST_VERTICAL_ADJACENT = 3f;

        /// <summary>
        /// Cost multiplier per palier gap for direct vertical edges.
        /// Total cost = gap * COST_VERTICAL_DIRECT_PER_FLOOR.
        /// </summary>
        private const float COST_VERTICAL_DIRECT_PER_FLOOR = 5f;

        /// <summary>
        /// Vertical penalty multiplier used in the heuristic.
        /// Discourages unnecessary palier changes.
        /// </summary>
        private const float HEURISTIC_VERTICAL_MULTIPLIER = 3f;

        // ── Internal state ────────────────────────────────────────────

        private readonly NavigationGrid _grid;

        /// <summary>
        /// Horizontal neighbour offsets — 4 directions on the same palier.
        /// Y component is always 0 (same palier).
        /// </summary>
        private static readonly Vector3Int[] HorizontalDirections =
        {
            new Vector3Int( 1, 0,  0), // East
            new Vector3Int(-1, 0,  0), // West
            new Vector3Int( 0, 0,  1), // North
            new Vector3Int( 0, 0, -1)  // South
        };

        // ── Constructor ───────────────────────────────────────────────

        /// <summary>
        /// Creates a new pathfinder operating on the given navigation grid.
        /// </summary>
        /// <param name="grid">
        /// The <see cref="NavigationGrid"/> built from the placed rooms of the floor.
        /// </param>
        public AStarPathfinder(NavigationGrid grid)
        {
            _grid = grid;
        }

        // ── Entry point ───────────────────────────────────────────────

        /// <summary>
        /// Finds the shortest path between two nodes in the navigation grid.
        ///
        /// Returns a list of nodes from <paramref name="start"/> to
        /// <paramref name="end"/> (both inclusive), ordered start → end.
        /// Returns null if no path exists.
        /// </summary>
        /// <param name="start">The starting node (exit door of the source room).</param>
        /// <param name="end">The destination node (entrance door of the target room).</param>
        /// <returns>
        /// An ordered list of <see cref="AStarNode"/> representing the corridor path,
        /// or null if no path was found.
        /// </returns>
        public List<AStarNode> FindPath(AStarNode start, AStarNode end)
        {
            // Reset pathfinding state on all nodes before each run
            ResetGrid();

            List<AStarNode> openSet    = new List<AStarNode>();
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

            start.gCost = 0;
            start.hCost = Heuristic(start, end);
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                AStarNode current = GetLowestFCost(openSet);
                openSet.Remove(current);
                closedSet.Add(current);

                // Path found
                if (current == end)
                    return ReconstructPath(start, end);

                foreach (AStarNode neighbour in GetNeighbours(current))
                {
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                        continue;

                    float movementCost = GetMovementCost(current, neighbour);
                    float newGCost     = current.gCost + movementCost;

                    if (newGCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost  = newGCost;
                        neighbour.hCost  = Heuristic(neighbour, end);
                        neighbour.parent = current;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            // No path found
            Debug.LogWarning($"[AStarPathfinder] No path found from " +
                $"{start.gridPosition} to {end.gridPosition}");
            return null;
        }

        // ── Neighbours ────────────────────────────────────────────────

        /// <summary>
        /// Returns all valid neighbours of the given node.
        ///
        /// Neighbours include:
        /// - 4 horizontal cells on the same palier (always checked)
        /// - Adjacent palier cells (only if current node is a CONNECTOR)
        /// - Direct vertical edge targets (only at stacked room positions)
        /// </summary>
        private List<AStarNode> GetNeighbours(AStarNode node)
        {
            List<AStarNode> neighbours = new List<AStarNode>();
            Vector3Int pos = node.gridPosition;

            // 1. Horizontal neighbours (same palier)
            foreach (Vector3Int dir in HorizontalDirections)
            {
                AStarNode neighbour = _grid.GetNode(
                    pos.x + dir.x,
                    pos.y,
                    pos.z + dir.z
                );
                if (neighbour != null)
                    neighbours.Add(neighbour);
            }

            // 2. Adjacent vertical neighbours (only from CONNECTOR cells)
            if (node.type == NodeType.Connector)
            {
                // Check palier above
                AStarNode above = _grid.GetNode(pos.x, pos.y + 1, pos.z);
                if (above != null && above.type == NodeType.Connector)
                    neighbours.Add(above);

                // Check palier below
                AStarNode below = _grid.GetNode(pos.x, pos.y - 1, pos.z);
                if (below != null && below.type == NodeType.Connector)
                    neighbours.Add(below);
            }

            // 3. Direct vertical edges (stacked rooms)
            Vector2Int xz = new Vector2Int(pos.x, pos.z);
            List<DirectVerticalEdge> directEdges = _grid.GetDirectEdgesAt(xz);

            if (directEdges != null)
            {
                foreach (DirectVerticalEdge edge in directEdges)
                {
                    // Only follow this edge if we are at one of its endpoints
                    if (edge.fromFloorIndex == pos.y || edge.toFloorIndex == pos.y)
                    {
                        int targetFloor = edge.fromFloorIndex == pos.y
                            ? edge.toFloorIndex
                            : edge.fromFloorIndex;

                        AStarNode target = _grid.GetNode(pos.x, targetFloor, pos.z);
                        if (target != null && target.isWalkable)
                            neighbours.Add(target);
                    }
                }
            }

            return neighbours;
        }

        // ── Cost & heuristic ──────────────────────────────────────────

        /// <summary>
        /// Returns the movement cost of travelling from one node to a neighbour.
        ///
        /// - Same palier → COST_HORIZONTAL (1.0)
        /// - Adjacent palier → COST_VERTICAL_ADJACENT (3.0)
        /// - Direct vertical edge → gap * COST_VERTICAL_DIRECT_PER_FLOOR
        /// </summary>
        private float GetMovementCost(AStarNode from, AStarNode to)
        {
            int dy = Mathf.Abs(to.gridPosition.y - from.gridPosition.y);

            if (dy == 0)
                return COST_HORIZONTAL;

            if (dy == 1)
                return COST_VERTICAL_ADJACENT;

            // Direct vertical edge (stacked rooms)
            return dy * COST_VERTICAL_DIRECT_PER_FLOOR;
        }

        /// <summary>
        /// Estimates the remaining cost from a node to the destination.
        ///
        /// Uses 3D Manhattan distance with a vertical penalty:
        ///   h(n) = |dx| + |dz| + |dy| * HEURISTIC_VERTICAL_MULTIPLIER
        ///
        /// The vertical multiplier matches the adjacent vertical movement cost (3.0),
        /// ensuring the heuristic never overestimates (admissibility condition).
        /// </summary>
        private float Heuristic(AStarNode node, AStarNode target)
        {
            int dx = Mathf.Abs(node.gridPosition.x - target.gridPosition.x);
            int dy = Mathf.Abs(node.gridPosition.y - target.gridPosition.y);
            int dz = Mathf.Abs(node.gridPosition.z - target.gridPosition.z);

            return dx + dz + dy * HEURISTIC_VERTICAL_MULTIPLIER;
        }

        // ── Path reconstruction ───────────────────────────────────────

        /// <summary>
        /// Reconstructs the path from start to end by following parent references.
        /// Returns an ordered list from start (index 0) to end (last index).
        /// </summary>
        private List<AStarNode> ReconstructPath(AStarNode start, AStarNode end)
        {
            List<AStarNode> path = new List<AStarNode>();
            AStarNode current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        // ── Helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Returns the node with the lowest fCost from the open set.
        /// If two nodes have equal fCost, the one with the lower hCost is preferred.
        /// </summary>
        private AStarNode GetLowestFCost(List<AStarNode> openSet)
        {
            AStarNode lowest = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < lowest.fCost ||
                    (openSet[i].fCost == lowest.fCost &&
                     openSet[i].hCost < lowest.hCost))
                {
                    lowest = openSet[i];
                }
            }
            return lowest;
        }

        /// <summary>
        /// Resets the pathfinding state (gCost, hCost, parent) of all nodes
        /// in the grid before each FindPath call.
        /// Necessary to ensure previous path results do not affect the new search.
        /// </summary>
        private void ResetGrid()
        {
            // Iterate over all nodes and reset their pathfinding state
            for (int x = 0; x < _grid._width; x++)
            for (int f = 0; f < _grid._floorCount; f++)
            for (int z = 0; z < _grid._depth; z++)
            {
                AStarNode node = _grid.GetNode(x, f, z);
                if (node == null) continue;
                node.gCost  = float.MaxValue;
                node.hCost  = 0;
                node.parent = null;
            }
        }
    }
}