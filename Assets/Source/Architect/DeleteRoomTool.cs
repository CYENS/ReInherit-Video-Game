using System.Collections.Generic;

namespace Cyens.ReInherit.Architect
{
    public class DeleteRoomTool : RoomTool
    {
        private Index m_firstIndex;

        // private bool m_isValid;

        private Room startRoom;

        private readonly List<Room> m_buffer = new(16);
        private Index m_currentIndex;

        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            m_buffer.Clear();
            startRoom = null;

            roomData.indicator.IsDrawing = true;
            roomData.indicator.Clear(data.index);

            roomData.indicator.SetArea(data.index);
            startRoom = roomData.graph.GetRoomAt(data.index);

            roomData.indicator.Color = roomData.colorOn;
        }

        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (data.WasIndexChanged) {
                m_currentIndex = data.index;
                roomData.indicator.SetArea(data.index);
            }
        }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            roomData.graph.GetRoomsAt(roomData.indicator.InclusiveBounds, m_buffer);

            foreach (var room in m_buffer) {
                Destroy(room);
            }
        }
    }
}