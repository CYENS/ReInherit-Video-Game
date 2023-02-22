using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

namespace Cyens.ReInherit.Architect
{
    public class Room : ScriptableObject
    {
        private List<Block> m_tiles;
        private RoomGraph m_parent;
        
        public RoomGraph Graph => m_parent;

        public static Room Create(RoomGraph graph)
        {
            var room = CreateInstance<Room>();
            room.m_tiles = new List<Block>(4);
            room.m_parent = graph;
            
            return room;
        }
        
        public void Add(Block block)
        {
            if (block.Parent != this) {
                return;
            }
            
            m_tiles.Add(block);
        }

        private void OnDestroy()
        {
            foreach (var block in m_tiles) {
                Destroy(block);
            }
        }

        public void RefreshAll()
        {
            foreach (var tile in m_tiles) {
                if (m_parent.GetRoomAt(tile.Index.East) == this) {
                    tile.Model.EastType = WallModel.WallType.None;
                } else {
                    tile.Model.EastType = WallModel.WallType.Wall;
                }
                
                if (m_parent.GetRoomAt(tile.Index.West) == this) {
                    tile.Model.WestType = WallModel.WallType.None;
                } else {
                    tile.Model.WestType = WallModel.WallType.Wall;
                }
                
                if (m_parent.GetRoomAt(tile.Index.North) == this) {
                    tile.Model.NorthType = WallModel.WallType.None;
                } else {
                    tile.Model.NorthType = WallModel.WallType.Wall;
                }
                
                if (m_parent.GetRoomAt(tile.Index.South) == this) {
                    tile.Model.SouthType = WallModel.WallType.None;
                } else {
                    tile.Model.SouthType = WallModel.WallType.Wall;
                }
                
            }
        }
    }
}