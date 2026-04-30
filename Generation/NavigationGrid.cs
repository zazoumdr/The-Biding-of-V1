using System.Collections.Generic;
using UnityEngine;

namespace TheBindingOfV1.Generation
{
    // ── Node types ────────────────────────────────────────────────────

    /// <summary>
    /// Defines the state of a cell in the <see cref="NavigationGrid"/>.
    /// </summary>
    public enum NodeType
    {
        /// <summary>Empty space — a corridor segment can be placed here.</summary>
        Free,

        /// <summary>
        /// Occupied by a room — corridors cannot pass through this cell.
        /// Set during grid construction for all cells inside room bounding boxes.
        /// </summary>
        Blocked,

        /// <summary>
        /// A vertical transition point — A* can move between paliers here.
        /// Used for adjacent palier transitions (e.g. vent, wall jump, jump pad).
        /// </summary>
        Connector
    }

    // ── A* Node ───────────────────────────────────────────────────────

    /// <summary>
    /// Represents a single cell in the <see cref="NavigationGrid"/>.
    /// Used by <see cref="AStarPathfinder"/> to store pathfinding state.
    ///
    /// Grid position is stored as (x, palierIndex, z) where palierIndex
    /// maps to a world Y height via <see cref="NavigationGrid.FloorHeights"/>.
    /// </summary>
    public class AStarNode
    {
        /// <summary>
        /// Position of this cell in grid space.
        /// X and Z are horizontal indices. Y is the palier index (not world Y).
        /// Use <see cref="NavigationGrid.FloorToIndex"/> and
        /// <see cref="NavigationGrid.IndexToFloor"/> to convert.
        /// </summary>
        public Vector3Int gridPosition;

        /// <summary>Accumulated movement cost from the start node to this node.</summary>
        public float gCost;

        /// <summary>
        /// Estimated remaining cost from this node to the destination (heuristic).
        /// Computed using 3D Manhattan distance with a vertical penalty.
        /// </summary>
        public float hCost;

        /// <summary>Total estimated cost. A* always expands the node with the lowest fCost.</summary>
        public float fCost => gCost + hCost;

        /// <summary>
        /// The node this node was reached from. Used to reconstruct the path
        /// once the destination is found.
        /// </summary>
        public AStarNode parent;

        /// <summary>Whether A* can route a corridor through this cell.</summary>
        public bool isWalkable;

        /// <summary>The functional type of this cell.</summary>
        public NodeType type;
    }

    // ── Navigation Grid ───────────────────────────────────────────────

    /// <summary>
    /// A 3D discrete navigation grid built from the placed rooms of a floor.
    ///
    /// The grid discretizes the world XZ plane into cells of size
    /// <see cref="CELL_SIZE"/> Unity units, with one layer per palier.
    /// Each cell is either FREE (corridor can pass), BLOCKED (inside a room),
    /// or CONNECTOR (vertical transition point).
    ///
    /// Grid construction:
    /// 1. The world bounding box of all placed rooms is computed.
    /// 2. A padding margin is added so corridors can route around rooms.
    /// 3. All cells are initialized as FREE.
    /// 4. Cells occupied by rooms are marked BLOCKED.
    ///
    /// Vertical connections:
    /// - Adjacent palier connectors are set via <see cref="SetConnector"/>.
    /// - Direct vertical edges between stacked rooms (gap >= 2) are registered
    ///   via <see cref="AddDirectVerticalEdge"/> and stored in
    ///   <see cref="_directVerticalEdges"/> for use by <see cref="AStarPathfinder"/>.
    ///
    /// Coordinate systems:
    /// - World space: Unity units (float X, Y, Z)
    /// - Grid space: cell indices (int x, palierIndex, z)
    /// - Palier index: 0 = palier -3, 3 = palier 0, 6 = palier +3
    ///
    /// Use <see cref="WorldToNode"/> to convert a world position to a grid node,
    /// and <see cref="GridToWorldX"/> / <see cref="GridToWorldZ"/> to convert back.
    /// </summary>
    public class NavigationGrid
    {
        // ── Constants ─────────────────────────────────────────────────

        /// <summary>
        /// Size of one grid cell in Unity units.
        /// Must match the width and length of corridor prefab segments.
        /// </summary>
        public const float CELL_SIZE = 8f;

        /// <summary>
        /// Padding added around the room bounds when sizing the grid.
        /// Ensures corridors have room to route around the outermost rooms.
        /// </summary>
        public const float GRID_PADDING = 5 * CELL_SIZE; // 40 units

        /// <summary>
        /// Maps a palier index to its world Y height in Unity units.
        /// Shared with <see cref="RoomPlacer"/> to ensure consistency.
        /// </summary>
        public static readonly Dictionary<int, float> FloorHeights =
            new Dictionary<int, float>
        {
            { -3, -48f },
            { -2, -32f },
            { -1, -16f },
            {  0,   0f },
            {  1,  16f },
            {  2,  32f },
            {  3,  48f }
        };

        // ── Internal state ────────────────────────────────────────────

        private AStarNode[,,] _grid;
        private int _width;
        private int _depth;
        private int _floorCount;
        private Vector3 _origin; // world position of grid cell (0, 0, 0)

        /// <summary>
        /// Direct vertical edges between stacked rooms (palier gap >= 2).
        /// Keyed by XZ grid position. Used by AStarPathfinder to allow
        /// direct multi-palier jumps at stacked room positions.
        /// </summary>
        private Dictionary<Vector2Int, List<DirectVerticalEdge>> _directVerticalEdges
            = new Dictionary<Vector2Int, List<DirectVerticalEdge>>();

        // ── Constructor ───────────────────────────────────────────────

        /// <summary>
        /// Builds the navigation grid from a list of placed rooms.
        /// All room cells are marked BLOCKED. All other cells are FREE.
        /// Direct vertical edges are registered for stacked room pairs.
        /// </summary>
        /// <param name="placedRooms">
        /// All rooms placed in this floor by <see cref="RoomPlacer"/>.
        /// </param>
        public NavigationGrid(List<PlacedRoom> placedRooms)
        {
            BuildFromRooms(placedRooms);
            RegisterDirectVerticalEdges(placedRooms);
        }

        // ── Grid construction ─────────────────────────────────────────

        private void BuildFromRooms(List<PlacedRoom> rooms)
        {
            // 1. Compute world bounds of all rooms
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (PlacedRoom room in rooms)
            {
                minX = Mathf.Min(minX, room.bounds.min.x);
                maxX = Mathf.Max(maxX, room.bounds.max.x);
                minZ = Mathf.Min(minZ, room.bounds.min.z);
                maxZ = Mathf.Max(maxZ, room.bounds.max.z);
            }

            // 2. Add padding
            minX -= GRID_PADDING; maxX += GRID_PADDING;
            minZ -= GRID_PADDING; maxZ += GRID_PADDING;

            // 3. Compute grid dimensions
            _origin     = new Vector3(minX, 0f, minZ);
            _width      = Mathf.CeilToInt((maxX - minX) / CELL_SIZE);
            _depth      = Mathf.CeilToInt((maxZ - minZ) / CELL_SIZE);
            _floorCount = FloorHeights.Count; // 7 paliers (-3 to +3)

            // 4. Initialize all cells as FREE
            _grid = new AStarNode[_width, _floorCount, _depth];

            for (int x = 0; x < _width; x++)
            for (int f = 0; f < _floorCount; f++)
            for (int z = 0; z < _depth; z++)
            {
                _grid[x, f, z] = new AStarNode
                {
                    gridPosition = new Vector3Int(x, f, z),
                    isWalkable   = true,
                    type         = NodeType.Free
                };
            }

            // 5. Block cells occupied by rooms
            foreach (PlacedRoom room in rooms)
                BlockRoom(room);
        }

        /// <summary>
        /// Marks all grid cells overlapping a room's bounding box as BLOCKED.
        /// All paliers occupied by the room are blocked.
        /// </summary>
        private void BlockRoom(PlacedRoom room)
        {
            int xMin = WorldToGridX(room.bounds.min.x);
            int xMax = WorldToGridX(room.bounds.max.x);
            int zMin = WorldToGridZ(room.bounds.min.z);
            int zMax = WorldToGridZ(room.bounds.max.z);

            foreach (int floor in room.room.occupiedFloors)
            {
                int f = FloorToIndex(floor);
                for (int x = xMin; x <= xMax; x++)
                for (int z = zMin; z <= zMax; z++)
                {
                    if (!InBounds(x, f, z)) continue;
                    _grid[x, f, z].isWalkable = false;
                    _grid[x, f, z].type       = NodeType.Blocked;
                }
            }
        }

        /// <summary>
        /// Registers direct vertical edges for all stacked room pairs.
        /// A stacked pair is two rooms at the same XZ position with a palier gap >= 2.
        /// These edges allow A* to route a straight vertical shaft between them.
        /// </summary>
        private void RegisterDirectVerticalEdges(List<PlacedRoom> rooms)
        {
            foreach (PlacedRoom room in rooms)
            {
                foreach (PlacedRoom stacked in room.stackedAbove)
                    AddDirectVerticalEdge(room, stacked);
            }
        }

        // ── Public API ────────────────────────────────────────────────

        /// <summary>
        /// Marks a cell as a CONNECTOR, allowing A* to transition between
        /// adjacent paliers at this position.
        /// </summary>
        public void SetConnector(int x, int floorIndex, int z)
        {
            if (!InBounds(x, floorIndex, z)) return;
            _grid[x, floorIndex, z].type       = NodeType.Connector;
            _grid[x, floorIndex, z].isWalkable  = true;
        }

        /// <summary>
        /// Registers a direct vertical edge between two stacked rooms.
        /// A* can use this edge to jump directly between the two paliers
        /// without passing through intermediate paliers.
        /// </summary>
        public void AddDirectVerticalEdge(PlacedRoom from, PlacedRoom to)
        {
            int x = WorldToGridX(from.position.x);
            int z = WorldToGridZ(from.position.z);
            Vector2Int key = new Vector2Int(x, z);

            if (!_directVerticalEdges.ContainsKey(key))
                _directVerticalEdges[key] = new List<DirectVerticalEdge>();

            _directVerticalEdges[key].Add(new DirectVerticalEdge
            {
                fromFloorIndex = FloorToIndex(from.floor),
                toFloorIndex   = FloorToIndex(to.floor),
                gridX          = x,
                gridZ          = z
            });
        }

        /// <summary>
        /// Returns all direct vertical edges at a given XZ grid position.
        /// Returns null if no edges exist at this position.
        /// </summary>
        public List<DirectVerticalEdge> GetDirectEdgesAt(Vector2Int xz)
        {
            _directVerticalEdges.TryGetValue(xz, out List<DirectVerticalEdge> edges);
            return edges;
        }

        /// <summary>
        /// Returns the node at the given grid coordinates, or null if out of bounds.
        /// </summary>
        public AStarNode GetNode(int x, int floorIndex, int z)
        {
            if (!InBounds(x, floorIndex, z)) return null;
            return _grid[x, floorIndex, z];
        }

        /// <summary>
        /// Converts a world position and palier to the corresponding grid node.
        /// Returns null if the position is outside the grid bounds.
        /// </summary>
        public AStarNode WorldToNode(Vector3 worldPos, int floor)
        {
            int x = WorldToGridX(worldPos.x);
            int z = WorldToGridZ(worldPos.z);
            int f = FloorToIndex(floor);
            return GetNode(x, f, z);
        }

        /// <summary>Returns true if the given grid coordinates are within bounds.</summary>
        public bool InBounds(int x, int f, int z)
        {
            return x >= 0 && x < _width  &&
                   f >= 0 && f < _floorCount &&
                   z >= 0 && z < _depth;
        }

        // ── Coordinate helpers ────────────────────────────────────────

        /// <summary>Converts a world X coordinate to a grid column index.</summary>
        public int WorldToGridX(float worldX) =>
            Mathf.FloorToInt((worldX - _origin.x) / CELL_SIZE);

        /// <summary>Converts a world Z coordinate to a grid row index.</summary>
        public int WorldToGridZ(float worldZ) =>
            Mathf.FloorToInt((worldZ - _origin.z) / CELL_SIZE);

        /// <summary>Converts a grid column index to a world X coordinate.</summary>
        public float GridToWorldX(int x) => _origin.x + x * CELL_SIZE;

        /// <summary>Converts a grid row index to a world Z coordinate.</summary>
        public float GridToWorldZ(int z) => _origin.z + z * CELL_SIZE;

        /// <summary>
        /// Converts a palier number (-3 to +3) to a palier array index (0 to 6).
        /// </summary>
        public static int FloorToIndex(int floor) => floor + 3;

        /// <summary>
        /// Converts a palier array index (0 to 6) back to a palier number (-3 to +3).
        /// </summary>
        public static int IndexToFloor(int index) => index - 3;
    }

    // ── Direct Vertical Edge ──────────────────────────────────────────

    /// <summary>
    /// Represents a direct vertical connection between two stacked rooms.
    /// Used by <see cref="AStarPathfinder"/> to allow multi-palier jumps
    /// at the exact XZ position of stacked rooms.
    /// </summary>
    public class DirectVerticalEdge
    {
        /// <summary>Palier index of the lower room.</summary>
        public int fromFloorIndex;

        /// <summary>Palier index of the upper room.</summary>
        public int toFloorIndex;

        /// <summary>Grid X position of the shaft.</summary>
        public int gridX;

        /// <summary>Grid Z position of the shaft.</summary>
        public int gridZ;

        /// <summary>
        /// Movement cost for using this edge.
        /// Higher gap = higher cost to discourage unnecessary vertical jumps.
        /// </summary>
        public float Cost => Mathf.Abs(toFloorIndex - fromFloorIndex) * 5f;
    }
}