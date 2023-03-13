namespace Cyens.ReInherit.Architect
{
    public class MergeRoomTool : RoomTool
    {
        private Index m_index1;
        private Index m_index2;

        private bool m_isValidStart;
        
        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            InitMove(in roomData, in data);
        }

        private void InitMove(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = true;

            m_index2 = m_index1 = data.index;
            m_isValidStart = roomData.graph[m_index1] != null;
            
            SetIndicatorSingleTile(roomData, data.index);
        }

        private void SetIndicatorSingleTile(in RoomData roomData, Index index)
        {
            roomData.indicator.Clear(index);
            roomData.indicator.SetArea(index);
            roomData.indicator.Color = m_isValidStart ? roomData.colorBlue : roomData.colorOff;
            m_index1 = m_index2 = index;
        }

        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (!data.WasIndexChanged) {
                return;
            }

            if (m_index1 == data.index) {
                SetIndicatorSingleTile(roomData, data.index);
            } else {
                if (Index.AreAdjacent(m_index1, data.index)) {
                    m_index2 = data.index;
                    roomData.indicator.SetArea(new IndexBounds(m_index1, m_index2));
                }
            }

            roomData.indicator.Color = CheckValidity(roomData) ? roomData.colorOn : roomData.colorOff;
        }

        private bool CheckValidity(in RoomData roomData)
        {
            if (!Index.AreAdjacent(m_index1, m_index2) || !m_index1.IsValid || !m_index2.IsValid) {
                return false;
            }

            var graph = roomData.graph;
            var room1 = graph[m_index1];
            var room2 = graph[m_index2];

            return room1 != null && room2 != null && room1 != room2;
        }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;

            if (!CheckValidity(roomData)) {
                return;
            }

            roomData.graph.MergeArea(roomData.indicator.InclusiveBounds);
            roomData.indicator.Clear();
        }

        public override bool OnAddCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            return true;
        }

        public override void OnActivate(in RoomData roomData) { }

        public override void OnDeactivate(in RoomData roomData) { }
    }
}