using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class AddRoomTool : RoomTool
    {
        private Index m_firstIndex;
        
        // Used when extending a room
        private Room m_startRoom;

        private readonly List<Room> m_buffer = new(16);

        private bool CheckValid(in RoomData data)
        {
            m_buffer.Clear();
            var area = data.indicator.InclusiveBounds;
            if (!RoomGraph.Bounds.Contains(area)) {
                return false;
            }

            data.graph.GetRoomsAt(area, m_buffer);

            return m_buffer.Count == 0 || m_buffer.Count == 1 && m_buffer[0] == m_startRoom;
        }

        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            m_buffer.Clear();
            m_startRoom = null;

            roomData.indicator.IsDrawing = true;
            roomData.indicator.Clear(data.index);

            roomData.indicator.SetArea(data.index);
            m_startRoom = roomData.graph.GetRoomAt(data.index);

            var color = CheckValid(in roomData) ? roomData.colorOn : roomData.colorOff;

            if (m_startRoom != null) {
                color = roomData.colorOn;
            }

            roomData.indicator.Color = color;
        }

        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (data.WasIndexChanged) {
                roomData.indicator.SetArea(data.index);
                var color = CheckValid(in roomData) ? roomData.colorOn : roomData.colorOff;
                roomData.indicator.Color = color;
            }
        }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;

            if (CheckValid(roomData)) {
                AddRoom(roomData.graph, roomData.indicator.InclusiveBounds);
            }
        }

        private void AddRoom(RoomGraph graph, IndexBounds bounds)
        {
            var room = m_startRoom != null ? m_startRoom : Room.Create(graph);

            for (var x = bounds.min.x; x < bounds.max.x; ++x) {
                for (var y = bounds.min.y; y < bounds.max.y; ++y) {
                    var index = new Index(x, y);
                    if (graph.GetBlockAtIndex(index) == null) {
                        
                        // TODO: Should we do something with the return value of Block.Create?
                        var block = Block.Create(room, index);
                    }
                }
            }

            // TODO: Only refresh blocks that are necessary.
            room.RefreshAll();
        }
    }
}