using System.Collections.Generic;
using Cyens.ReInherit.Extensions;
using Cyens.ReInherit.Pooling;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class RoomGraph : MonoBehaviour
    {
        public const int MaxSizeX = 60;
        public const int MaxSizeY = 40;

        private readonly Block[,] m_blocks = new Block[MaxSizeX, MaxSizeY];
        private readonly Room[,] m_rooms = new Room[MaxSizeX, MaxSizeY];
        private readonly Direction[,] m_connections = new Direction[MaxSizeX, MaxSizeY];

        static RoomGraph()
        {
            Bounds = new IndexBounds(
                ToWorldIndex(new Index(0, 0)),
                ToWorldIndex(new Index(MaxSizeX - 1, MaxSizeY - 1))
            );
        }

        public static IndexBounds Bounds { get; }

        private List<Block> m_purchasedBlocks = new();

        private void Awake()
        {
            Debug.Log(ClassPool<Block>.Spawn().Index);
        }

        private static Index ToRawIndex(Index index)
        {
            index.x += MaxSizeX / 2;
            return index;
        }

        public static Index ToWorldIndex(Index index)
        {
            index.x -= MaxSizeX / 2;
            return index;
        }

        public Block GetBlockAtIndex(Index index)
        {
            index = ToRawIndex(index);
            return GetBlockAtRawIndex(index);
        }

        private Block GetBlockAtRawIndex(Index index)
        {
            if (index.x is < 0 or >= MaxSizeX || index.y is < 0 or >= MaxSizeY) {
                return null;
            }

            return m_blocks[index.x, index.y];
        }

        private Block GetBlockAt(Vector3 worldCoordinate)
        {
            var index = Index.FromWorld(worldCoordinate);
            return GetBlockAtIndex(index);
        }

        public void RemoveBlock(Index index) { }

        public void ConnectBlocks(Block lhs, Block rhs) { }

        public void UniteBlocks(Block lhs, Block rhs) { }

        public void CreateRoom(Index index) { }

        private readonly bool[,] m_flagBuffer = new bool[MaxSizeX, MaxSizeY];

        private readonly HashSet<Room> cacheSet = new(32);

        public int GetRoomsAt(IndexBounds bounds, List<Room> buffer)
        {
            buffer.Clear();
            cacheSet.Clear();

            var min = ToRawIndex(bounds.min);
            var max = ToRawIndex(bounds.max);

            for (var x = min.x; x < max.x; ++x) {
                for (var y = min.y; y < max.y; ++y) {
                    var block = GetBlockAtRawIndex(new Index(x, y));
                    if (block != null) {
                        var room = block.Parent;
                        cacheSet.Add(room);
                    }
                }
            }

            buffer.ReserveCapacity(cacheSet.Count);
            foreach (var room in cacheSet) {
                buffer.Add(room);
            }

            return buffer.Count;
        }

        public bool RoomExistsAt(IndexBounds bounds)
        {
            bounds = bounds.Readjusted;
            var min = ToRawIndex(bounds.min);
            var max = ToRawIndex(bounds.max);

            for (var x = min.x; x < max.x; ++x) {
                for (var y = min.y; y < max.y; ++y) {
                    if (GetBlockAtRawIndex(new Index(x, y)) != null) {
                        return true;
                    }
                }
            }

            return false;
        }


        public Room GetRoomAt(Index index)
        {
            var block = GetBlockAtIndex(index);
            return block != null ? block.Parent : null;
        }


        public bool IsWithinBounds(Index index)
        {
            index = ToRawIndex(index);
            return index.x is >= 0 and < MaxSizeX && index.y is >= 0 and < MaxSizeY;
        }

        private bool TryFlagIndex(Index index)
        {
            if (IsWithinBounds(index) && m_flagBuffer[index.x, index.y]) {
                m_flagBuffer[index.x, index.y] = true;
                return true;
            }

            return false;
        }

        /// Fills a buffer with a list of blocks that are available for puchase
        public void GetAvailableBlocks(List<Index> buffer)
        {
            buffer.Clear();

            if (m_purchasedBlocks.Count == 0) {
                buffer.Add(Index.Zero);
            }

            foreach (var block in m_purchasedBlocks) {
                var index = ToRawIndex(block.Index);

                if (TryFlagIndex(index.West)) {
                    buffer.Add(ToWorldIndex(index.West));
                }

                if (TryFlagIndex(index.East)) {
                    buffer.Add(ToWorldIndex(index.East));
                }

                if (TryFlagIndex(index.North)) {
                    buffer.Add(ToWorldIndex(index.North));
                }

                if (TryFlagIndex(index.South)) {
                    buffer.Add(ToWorldIndex(index.South));
                }
            }

            // Clear flag buffer for reuse
            foreach (var index in buffer) {
                var raw = ToRawIndex(index);
                m_flagBuffer[raw.x, raw.y] = false;
            }
        }

        public void AddBlock(Block block)
        {
            if (block.Graph != this) {
                return;
            }

            var raw = ToRawIndex(block.Index);
            
            var existing = m_blocks[raw.x, raw.y];
            if (existing != null) {
                Destroy(existing);
            }
            m_blocks[raw.x, raw.y] = block;
        }
    }
}