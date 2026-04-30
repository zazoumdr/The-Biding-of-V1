using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TheBindingOfV1.Generation
{
    /// <summary>
    /// Responsible for assigning 3D positions to room nodes in the floor graph.
    ///
    /// The placement algorithm runs in two modes per room:
    ///
    /// 1. Vertical Stack (25% chance) — places the room at the same XZ position
    ///    as an existing room but on a different palier (gap >= 2). This creates
    ///    a direct vertical connection (free fall, elevator, etc.) between the
    ///    two rooms. Stacking relationships are stored bidirectionally in
    ///    <see cref="PlacedRoom.stackedAbove"/> and <see cref="PlacedRoom.stackedBelow"/>.
    ///
    /// 2. Random Placement — picks a candidate position randomly within an annulus
    ///    (ring) between <see cref="MIN_DISTANCE"/> and <see cref="MAX_DISTANCE"/>
    ///    around the previous placed room. The position is validated for bounding
    ///    box overlap and minimum distance before being accepted.
    ///
    /// Placement order:
    /// - Start room is always placed at the origin (0, 0, 0).
    /// - Boss room is placed far away from Start to enforce the "furthest room" constraint.
    /// - All other rooms are placed randomly between Start and Boss.
    ///
    /// After all rooms are placed, the resulting <see cref="PlacedRoom"/> list is
    /// passed to <see cref="NavigationGrid"/> for grid construction, and then to
    /// <see cref="AStarPathfinder"/> for corridor routing.
    /// </summary>
    public static class RoomPlacer
    {
        // ── Distance constraints ──────────────────────────────────────

        /// <summary>Minimum XZ distance between any two rooms.</summary>
        public const float MIN_DISTANCE = 20f;

        /// <summary>Maximum XZ distance from the reference room when placing randomly.</summary>
        public const float MAX_DISTANCE = 80f;

        /// <summary>
        /// XZ tolerance within which two rooms are considered to be at the same
        /// horizontal position (i.e. stacked). Used to detect and prevent duplicate
        /// stacks at the same position.
        /// </summary>
        public const float STACK_TOLERANCE = 4f;

        /// <summary>Maximum number of placement attempts before skipping a room.</summary>
        public const int MAX_ATTEMPTS = 50;

        // ── Probability ───────────────────────────────────────────────

        /// <summary>
        /// Probability (0-1) of attempting a vertical stack placement before
        /// falling back to random placement.
        /// </summary>
        public const float VERTICAL_STACK_CHANCE = 0.25f;

        // ── Palier heights ────────────────────────────────────────────

        /// <summary>
        /// Maps a palier index to its world Y height in Unity units.
        /// Palier 0 is the run start level. Positive paliers are above, negative below.
        /// Will change for shure but for now its the standar
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

        // ── Entry point ──────────────────────────────────────────────

        /// <summary>
        /// Attempts to place a room in 3D space.
        /// First tries a vertical stack (25% chance), then falls back to random placement.
        /// </summary>
        /// <param name="room">The RoomData of the room to place.</param>
        /// <param name="targetFloor">The palier index to place the room on.</param>
        /// <param name="placedRooms">All rooms already placed in this floor.</param>
        /// <returns>
        /// A <see cref="PlacedRoom"/> with position, floor, bounds, and stacking
        /// relationships filled in, or null if placement failed after all attempts.
        /// </returns>
        public static PlacedRoom TryPlaceRoom(
            RoomData room,
            int targetFloor,
            List<PlacedRoom> placedRooms)
        {
            // Attempt vertical stack first
            if (placedRooms.Count > 0 && Random.value < VERTICAL_STACK_CHANCE)
            {
                PlacedRoom stackResult = TryStackRoom(room, targetFloor, placedRooms);
                if (stackResult != null)
                    return stackResult;
            }

            // Fall back to random placement
            return TryPlaceRoomRandom(room, targetFloor, placedRooms);
        }

        /// <summary>
        /// Places the Start room at the world origin (0, 0, 0) on palier 0.
        /// Always succeeds.
        /// </summary>
        public static PlacedRoom PlaceStartRoom(RoomData room)
        {
            Vector3 position = new Vector3(0f, FloorHeights[0], 0f);
            return new PlacedRoom
            {
                room     = room,
                position = position,
                floor    = 0,
                bounds   = CalculateBounds(position, room)
            };
        }

        /// <summary>
        /// Places the Boss room as far as possible from the Start room.
        /// Picks a random direction from the origin and places the Boss near
        /// <see cref="MAX_DISTANCE"/> in that direction, on a random palier.
        /// </summary>
        /// <param name="room">The Boss room RoomData.</param>
        /// <param name="placedRooms">All rooms already placed (must include Start).</param>
        /// <returns>A PlacedRoom for the Boss, or null if placement failed.</returns>
        public static PlacedRoom PlaceBossRoom(RoomData room, List<PlacedRoom> placedRooms)
        {
            int[] availableFloors = { -3, -2, -1, 0, 1, 2, 3 };
            int targetFloor = availableFloors[Random.Range(0, availableFloors.Length)];

            // Place Boss far from Start in a random direction
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 bossPosition = new Vector3(
                randomDir.x * MAX_DISTANCE * 0.9f,
                FloorHeights[targetFloor],
                randomDir.y * MAX_DISTANCE * 0.9f
            );

            Bounds bossBounds = CalculateBounds(bossPosition, room);

            if (HasOverlap(bossBounds, placedRooms))
                return TryPlaceRoomRandom(room, targetFloor, placedRooms);

            return new PlacedRoom
            {
                room     = room,
                position = bossPosition,
                floor    = targetFloor,
                bounds   = bossBounds
            };
        }

        // ── Placement modes ───────────────────────────────────────────

        /// <summary>
        /// Attempts to place the room directly above or below an existing room
        /// at the same XZ position (vertical stack).
        /// </summary>
        /// <param name="room">The room to place.</param>
        /// <param name="targetFloor">The target palier index.</param>
        /// <param name="placedRooms">All rooms already placed.</param>
        /// <returns>A PlacedRoom if a valid stack position was found, otherwise null.</returns>
        private static PlacedRoom TryStackRoom(
            RoomData room,
            int targetFloor,
            List<PlacedRoom> placedRooms)
        {
            // Find rooms on a non-adjacent palier that don't already have a stack
            // at the target floor
            List<PlacedRoom> candidates = placedRooms.FindAll(p =>
                Mathf.Abs(p.floor - targetFloor) >= 2 &&
                !HasStackAt(p.position, targetFloor, placedRooms)
            );

            if (candidates.Count == 0)
                return null;

            // Pick a random candidate
            PlacedRoom reference = candidates[Random.Range(0, candidates.Count)];

            Vector3 stackPosition = new Vector3(
                reference.position.x,
                FloorHeights[targetFloor],
                reference.position.z
            );

            Bounds stackBounds = CalculateBounds(stackPosition, room);

            if (HasOverlap(stackBounds, placedRooms))
                return null;

            PlacedRoom placed = new PlacedRoom
            {
                room     = room,
                position = stackPosition,
                floor    = targetFloor,
                bounds   = stackBounds
            };

            // Register bidirectional stacking relationship
            reference.stackedAbove.Add(placed);
            placed.stackedBelow.Add(reference);

            return placed;
        }

        /// <summary>
        /// Places the room at a random position within an annulus around the
        /// last placed room, validating overlap and minimum distance constraints.
        /// </summary>
        /// <param name="room">The room to place.</param>
        /// <param name="targetFloor">The target palier index.</param>
        /// <param name="placedRooms">All rooms already placed.</param>
        /// <returns>A PlacedRoom if a valid position was found, otherwise null.</returns>
        private static PlacedRoom TryPlaceRoomRandom(
            RoomData room,
            int targetFloor,
            List<PlacedRoom> placedRooms)
        {
            // Reference position = last placed room (or origin if none)
            Vector3 reference = placedRooms.Count > 0
                ? placedRooms[placedRooms.Count - 1].position
                : Vector3.zero;

            float floorY = FloorHeights[targetFloor];

            for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
            {
                // Pick a random position inside an annulus
                Vector2 randomXZ = Random.insideUnitCircle.normalized
                    * Random.Range(MIN_DISTANCE, MAX_DISTANCE);

                Vector3 candidate = new Vector3(
                    reference.x + randomXZ.x,
                    floorY,
                    reference.z + randomXZ.y
                );

                Bounds candidateBounds = CalculateBounds(candidate, room);

                if (!HasOverlap(candidateBounds, placedRooms) &&
                    IsDistanceValid(candidate, placedRooms))
                {
                    return new PlacedRoom
                    {
                        room     = room,
                        position = candidate,
                        floor    = targetFloor,
                        bounds   = candidateBounds
                    };
                }
            }

            return null;
        }

        // ── Validation helpers ────────────────────────────────────────

        /// <summary>
        /// Returns true if there is already a room at approximately the same XZ
        /// position on the given palier.
        /// </summary>
        private static bool HasStackAt(
            Vector3 xzReference,
            int floor,
            List<PlacedRoom> placedRooms)
        {
            foreach (PlacedRoom placed in placedRooms)
            {
                if (placed.floor != floor) continue;

                float xzDistance = Vector2.Distance(
                    new Vector2(placed.position.x, placed.position.z),
                    new Vector2(xzReference.x, xzReference.z)
                );

                if (xzDistance <= STACK_TOLERANCE)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the candidate bounds overlap with any already placed room.
        /// </summary>
        private static bool HasOverlap(Bounds candidate, List<PlacedRoom> placedRooms)
        {
            foreach (PlacedRoom placed in placedRooms)
            {
                if (candidate.Intersects(placed.bounds))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the candidate position respects the minimum XZ distance
        /// from all already placed rooms.
        /// </summary>
        private static bool IsDistanceValid(Vector3 candidate, List<PlacedRoom> placedRooms)
        {
            foreach (PlacedRoom placed in placedRooms)
            {
                float xzDistance = Vector2.Distance(
                    new Vector2(candidate.x, candidate.z),
                    new Vector2(placed.position.x, placed.position.z)
                );

                if (xzDistance < MIN_DISTANCE)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Calculates the world-space bounding box of a room at a given position.
        /// </summary>
        private static Bounds CalculateBounds(Vector3 position, RoomData room)
        {
            return new Bounds(position, room.dimensions);
        }
    }
}