using System;
using System.Collections.Generic;
using Cyens.ReInherit.Extensions;
using Cyens.ReInherit.Pooling;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public partial class RoomGraph : MonoBehaviour
    {
        private readonly GridData<Block> m_blocks = new();

        public Room this[Index index] => m_blocks.TryGet(index, out var blockData) ? blockData.room : null;

        [SerializeField] private List<InnerRoom> rooms = new();

        private enum VisitState
        {
            None,
            Visited,
            MarkedForDeletion,
        }

        private void Awake()
        {
            foreach (var index in Iterate(IndexBounds.FullBounds)) {
                m_blocks[index.x, index.y] = new Block(index);
            }
        }

        public int GetRoomsAt(in IndexBounds area, List<Room> result = null)
        {
            // Mark underlying rooms as visited and then return the visited rooms.
            // Marking them as visited removes the need of using a set.

            foreach (var index in Iterate(area)) {
                var room = m_blocks[index.x, index.y].room;
                if (room == null) {
                    continue;
                }

                room.state = VisitState.Visited;
            }

            result?.Clear();

            var count = 0;
            foreach (var room in rooms) {
                if (room.state == VisitState.Visited) {
                    room.state = VisitState.None;
                    result?.Add(room);
                    ++count;
                }
            }

            return count;
        }

        private InnerRoom SplitRoom(InnerRoom room)
        {
            bool GetFirstTileFromRoom(out Index result)
            {
                result = Index.Zero;
                foreach (var tile in room.tiles) {
                    if (m_blocks[tile].state != VisitState.None) {
                        continue;
                    }

                    result = tile;
                    return true;
                }

                return false;
            }

            void MarkAccessibleTilesFromIndex(Index firstTile)
            {
                using var bufferStack = TempList<Index>.Get();

                // Enqueue first tile
                m_blocks[firstTile].state = VisitState.Visited;
                bufferStack.Add(firstTile);

                while (bufferStack.Count > 0) {
                    var current = bufferStack.Pop();

                    // Add each of the 4 adjacent tiles as long as they're valid
                    // (i.e. within bounds, with the same parent, and not already visited)
                    foreach (var dir in Direction.Values) {
                        var next = current.Towards(dir);
                        if (!next.IsValid) {
                            continue;
                        }

                        var block = m_blocks[next];
                        if (block.room != room || block.state != VisitState.None) {
                            continue;
                        }

                        m_blocks[next].state = VisitState.Visited;
                        bufferStack.Add(next);
                    }
                }
            }

            if (!GetFirstTileFromRoom(out var firstIndex)) {
                room.state = VisitState.MarkedForDeletion;
                return null;
            }

            MarkAccessibleTilesFromIndex(firstIndex);

            using var strayIndicesBuffer = TempList<Index>.Get();

            // Get unmarked tiles
            foreach (var tile in room.tiles) {
                var state = m_blocks[tile].state;
                if (state == VisitState.None) {
                    strayIndicesBuffer.Add(tile);
                    // Will be deleted from this room (but not the graph)
                    m_blocks[tile].state = VisitState.MarkedForDeletion;
                } else if (state == VisitState.Visited) {
                    m_blocks[tile].state = VisitState.None;
                }
            }

            // Actually remove blocks that are marked for deletion.
            // This will only remove them from the tile list of the room.
            room.RemoveMarkedForDeletion();

            if (strayIndicesBuffer.Count == 0) {
                // Room was not split into multiple subrooms
                return null;
            }

            // Add stray (unvisited) blocks to a new room.
            var newRoom = InnerRoom.Create(this);
            foreach (var tile in strayIndicesBuffer) {
                newRoom.tiles.Add(tile);
                
                var block = m_blocks[tile];
                block.state = VisitState.None;
                block.RemapRoomAndLinks(newRoom);
            }

            return newRoom;
        }

        /// Remove all blocks within an area.
        /// If any rooms are split in multiple parts, new rooms are created.
        /// If any rooms lose all their blocks they get destroyed.
        public void RemoveArea(IndexBounds area)
        {
            area = area.Clipped;

            foreach (var index in Iterate(area)) {
                var block = m_blocks[index];
                if (block.room != null) {
                    block.state = VisitState.MarkedForDeletion;
                    block.room.state = VisitState.Visited;
                }
            }

            using var newRoomsBuffer = TempList<InnerRoom>.Get();
            foreach (var room in rooms) {
                if (room.state != VisitState.Visited) {
                    continue;
                }

                var roomToSplit = room;
                while (true) {
                    // A deletion can split a room in more than 2 parts. Each time we split a room we check if the
                    // new room can be split further, until it can't be split anymore.
                    var newRoom = SplitRoom(roomToSplit);
                    if (newRoom == null) {
                        break;
                    }

                    roomToSplit = newRoom;
                    newRoomsBuffer.Add(newRoom);
                }
            }

            rooms.AddRange(newRoomsBuffer);

            // Wipe all the blocks that are included in the removal area.
            foreach (var index in Iterate(area)) {
                m_blocks[index].ClearBlockAndGatherModel();
            }

            // Refresh the blocks around (but not inside!) the deleted area
            foreach (var direction in Direction.Values) {
                foreach (var index in IterateSide(direction, area)) {
                    if (!m_blocks.TryGet(index.Towards(direction), out var block)) {
                        continue;
                    }

                    block.Links[direction.Opposite] = null;
                    block.RefreshModel();
                }
            }

            // Delete rooms when necessary, reset room state otherwise
            rooms.RemoveAll(r => {
                if (r.state == VisitState.MarkedForDeletion) {
                    // Destroying a room normally clears tiles one by one, but by now they're
                    // already cleared, so including them with the destroyed room would be unecessary.
                    r.tiles.Clear();
                    Destroy(r);
                    return true;
                }

                r.state = VisitState.None;
                return false;
            });
        }

        private void ExtendRoom(in IndexBounds area, InnerRoom room)
        {
            foreach (var index in Iterate(area)) {
                room.AddIfNew(index);
                m_blocks[index].ResetModelToFlat();
            }

            // Refresh blocks on the edges of the area and also those adjacent to them
            foreach (var direction in Direction.Values) {
                foreach (var index in IterateSide(direction, area)) {
                    m_blocks[index].RefreshModel();
                    if (m_blocks.TryGet(index.Towards(direction), out var adjacentBlock)) {
                        adjacentBlock.RefreshModel();
                    }
                }
            }
        }

        public Room MergeArea(in IndexBounds area)
        {
            using var roomList = TempList<Room>.Get();

            var count = GetRoomsAt(area, roomList);
            if (count < 2) {
                return null;
            }

            // All other rooms will be destroyed
            var room = (InnerRoom)roomList[0];

            // Remap blocks that already exist
            for (var i = 1; i < roomList.Count; ++i) {
                var otherRoom = (InnerRoom)roomList[i];
                foreach (var tile in otherRoom.tiles) {
                    room.tiles.Add(tile);

                    m_blocks[tile].RemapRoomAndLinks(room);
                }

                otherRoom.tiles.Clear();
                otherRoom.state = VisitState.MarkedForDeletion;
            }

            // Remove destroyed rooms
            rooms.RemoveAll(r => {
                if (r.state == VisitState.MarkedForDeletion) {
                    Destroy(r);
                    return true;
                }

                return false;
            });

            // Add any other remaining blocks in the area
            foreach (var index in Iterate(area)) {
                room.AddIfNew(index);
            }

            // All blocks need to be refreshed
            foreach (var tile in room.tiles) {
                m_blocks[tile].RefreshModel();
            }

            return room;
        }

        /// Add an area (non-inclusive) to the graph. This operation will only succeed if there are
        /// zero or exactly one rooms under the specified area. If there are no rooms, a new room
        /// is created. If there is ane existing room, that room is extended. Method returns the
        /// new or existing room that was modified. If the method failed, the null value is returned.
        public Room AddArea(in IndexBounds area)
        {
            using var roomsAtBuffer = TempList<Room>.Get();

            var count = GetRoomsAt(area, roomsAtBuffer);
            switch (count) {
                case 0:
                    return CreateRoom(area);
                case 1: {
                    var room = (InnerRoom)roomsAtBuffer[0];
                    ExtendRoom(area, room);
                    return room;
                }
                default:
                    return null;
            }
        }


        private Room CreateRoom(IndexBounds bounds)
        {
            bounds = bounds.Clipped;

            if (bounds.Area == 0) {
                return null;
            }

            var roomOut = InnerRoom.Create(this);
            foreach (var index in Iterate(bounds)) {
                roomOut.AddIfNew(index);
                m_blocks[index].ResetModelToFlat();
            }

            foreach (var direction in Direction.Values) {
                foreach (var index in IterateSide(direction, bounds)) {
                    m_blocks[index].model.SetModelType(direction, WallModel.WallType.Wall);
                }
            }

            rooms.Add(roomOut);

            return null;
        }

        /// Get the room the index connects towards a given direction
        public Room GetConnection(Index index, Direction direction)
        {
            return m_blocks.TryGet(index, out var block) ? block.Links[direction] : null;
        }

        /// Get if the index is accessible from the adjacent index at the specified direction.
        /// An index is accessible only if the adjacent index belongs in the same room, or
        /// there's a door connecting them.
        public bool IsAdjacentAccessible(Index index, Direction direction)
        {
            if (m_blocks.TryGet(index, out var block)) {
                if (m_blocks.TryGet(index.Towards(direction), out var blockTowards)) {
                    if (block.room == blockTowards.room || block.Links[direction] != null) {
                        return true;
                    }
                }
            }

            return false;
        }

        public Room RoomAt(Vector3 position) => this[Index.FromWorld(position)];

        [Serializable]
        private class Block
        {
            [SerializeField] public InnerRoom room;
            [SerializeField] public BlockModel model;
            [SerializeField] public VisitState state;
            [SerializeField] private Index index;
            [SerializeField] private DirectionList<Room> links;

            public DirectionList<Room> Links => links;

            public Block(Index index)
            {
                links = new();
                this.index = index;
            }

            public void ResetModelToFlat()
            {
                if (room == null) {
                    return;
                }

                if (model == null) {
                    model.RecreateNavMesh();
                    model = ArchitectLibrary.RoomPrefabs.Spawn(room.Graph.transform, index.WorldCenter);
                }

                model.Clear();
            }

            public void RefreshModel()
            {
                if (room == null) {
                    ClearBlockAndGatherModel();
                    return;
                }

                if (model == null) {
                    ResetModelToFlat();
                }

                var graph = room.Graph;

                foreach (var direction in Direction.Values) {
                    var indexTowards = index.Towards(direction);
                    if (!indexTowards.IsValid) {
                        // Outside bounds
                        model.SetModelType(direction, WallModel.WallType.Wall);
                        continue;
                    }

                    var other = graph.m_blocks[indexTowards];
                    var link = Links[direction];

                    WallModel.WallType wallType;
                    if (other.room == room) {
                        wallType = WallModel.WallType.None;
                    } else if (other.room == null || link != other.room || other.Links[direction.Opposite] != room) {
                        wallType = WallModel.WallType.Wall;
                    } else {
                        wallType = WallModel.WallType.Door;
                    }

                #if UNITY_EDITOR
                    if (wallType != WallModel.WallType.Door) {
                        if (Links[direction] != null && other.Links[direction.Opposite] != null) {
                            Debug.LogError("Invalid link detected between blocks");
                        }
                    }
                #endif

                    model.SetModelType(direction, wallType);
                }
            }

            public void ClearBlockAndGatherModel()
            {
                if (model != null) {
                    model.DisableFloorAndRemoveNavMesh();
                    ArchitectLibrary.RoomPrefabs.Gather(model);
                }

                model = null;
                room = null;
                state = VisitState.None;
                Links.Clear();
            }

            /// Moves and remaps to a different room and updates both tile lists
            public void MoveToDifferentRoom(InnerRoom newRoom)
            {
                if (room == newRoom) {
                    return;
                }

                if (room != null) {
                    room.tiles.Remove(index);
                }

                newRoom.tiles.Add(index);
                RemapRoomAndLinks(newRoom);
            }

            /// Sets parent and updates links without updating tile lists of rooms.
            public void RemapRoomAndLinks(InnerRoom newRoom)
            {
                if (room == null) {
                    Links.Clear();
                    room = newRoom;
                    return;
                }

                foreach (var dir in Direction.Values) {
                    var link = Links[dir];
                    if (link == null) {
                        continue;
                    }

                    var indexTowards = index.Towards(dir);
                    var blockTowards = newRoom.Graph.m_blocks[indexTowards];

                    if (blockTowards.room == newRoom) {
                        // Remove link
                        Links[dir] = blockTowards.Links[dir.Opposite] = null;
                    } else if (link == blockTowards.room && blockTowards.Links[dir.Opposite] == room) {
                        // Remap connection
                        blockTowards.Links[dir.Opposite] = newRoom;
                    }
                }

                room = newRoom;
            }
        }

        private void RefreshAroundIndex(Index index)
        {
            var block = m_blocks[index];
            block.RefreshModel();

            foreach (var direction in Direction.Values) {
                var indexTowards = index.Towards(direction);
                if (!indexTowards.IsValid) {
                    continue;
                }

                m_blocks[indexTowards].RefreshModel();
            }
        }

        public bool AreLinked(in Index from, in Index to)
        {
            GetAdjacency(from, to, out var block1, out var block2, out var dir);
            return block1.Links[dir] == block2.room && block2.Links[dir.Opposite] == block1.room;
        }

        private bool GetAdjacency(in Index from, in Index to, out Block block1, out Block block2, out Direction dir)
        {
            block1 = null;
            block2 = null;
            dir = default;

            if (!Index.AreAdjacent(from, to)) {
                return false;
            }

            if (!m_blocks.TryGet(from, out block1) || !m_blocks.TryGet(to, out block2)) {
                return false;
            }

            if (block1.room == null || block2.room == null || block1.room == block2.room) {
                return false;
            }

            dir = Direction.FromIndices(from, to);
            return true;
        }


        public bool CanLink(Index from, Index to)
        {
            return CanLink(from, to, out _, out _, out _);
        }

        private bool CanLink(Index from, Index to, out Block block1, out Block block2, out Direction dir)
        {
            if (!GetAdjacency(from, to, out block1, out block2, out dir)) {
                return false;
            }

            return block1.Links[dir] != block2.room || block2.Links[dir.Opposite] != block1.room;
        }

        public void Link(Index from, Index to)
        {
            if (!CanLink(from, to, out var block1, out var block2, out var direction)) {
                return;
            }

            block1.Links[direction] = block2.room;
            block2.Links[direction.Opposite] = block1.room;
            block1.RefreshModel();
            block2.RefreshModel();
        }

        public void Unlink(Index from, Index to)
        {
            if (!AreLinked(from, to)) {
                return;
            }

            var direction = Direction.FromIndices(to, from);

            var block1 = m_blocks[from];
            var block2 = m_blocks[to];

            block1.Links[direction] = null;
            block2.Links[direction.Opposite] = null;

            block1.RefreshModel();
            block2.RefreshModel();
        }

        private class InnerRoom : Room
        {
            [SerializeField] private RoomGraph graph;

            public VisitState state = VisitState.None;
            public List<Index> tiles;

            private Predicate<Index> m_deleteCallbackPredicate;

            public override RoomGraph Graph => graph;
            public override IReadOnlyList<Index> Tiles => tiles;
            public override bool Contains(Index index) => graph[index] == this;

            public static InnerRoom Create(RoomGraph graph)
            {
                var room = CreateInstance<InnerRoom>();
                room.graph = graph;
                room.tiles = new List<Index>(8);

                // Create a callback to capture references to avoid doing that repeatedly later.
                room.m_deleteCallbackPredicate = room.DeleteIndexCallback;
                return room;
            }

            public void AddIfNew(Index index)
            {
                if (!graph.m_blocks.TryGet(index, out var block) || block.room == this) {
                    return;
                }

                if (block.room != null && block.room != this) {
                    Debug.LogError("Tried to add block that belongs in another room");
                    return;
                }

                tiles.Add(index);
                block.room = this;
            }

            private bool DeleteIndexCallback(Index index)
            {
                return graph.m_blocks[index].state == VisitState.MarkedForDeletion;
            }

            public void RemoveMarkedForDeletion()
            {
                tiles.RemoveAll(m_deleteCallbackPredicate);
            }

            protected void OnDestroy()
            {
                if (ArchitectLibrary.IsDisposing) {
                    return;
                }

                foreach (var tile in tiles) {
                    var block = graph.m_blocks[tile];
                    if (block.room != this) {
                        // Block was moved to a different room
                        continue;
                    }

                    block.ClearBlockAndGatherModel();
                    graph.RefreshAroundIndex(tile);
                }
            }
        }
    }
}