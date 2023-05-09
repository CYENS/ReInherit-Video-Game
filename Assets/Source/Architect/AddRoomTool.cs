using System.Collections.Generic;
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
            
            if (!data.graph.UsableBounds.Contains(area)) {
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
            m_startRoom = roomData.graph[data.index];

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
                roomData.graph.AddArea(roomData.indicator.InclusiveBounds);
            }
            
            roomData.indicator.Clear();
        }

        public override bool OnAddCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            return true;
        }
    }
}