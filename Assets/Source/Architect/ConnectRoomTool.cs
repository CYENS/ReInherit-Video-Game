namespace Cyens.ReInherit.Architect
{
    public class ConnectRoomTool : RoomTool
    {
        private Index m_index1;
        private Index m_index2;

        private bool m_isValidStart;

        private bool m_isSelectionValid;

        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            InitMove(in roomData, in data);
        }

        private void InitMove(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = true;

            m_index2 = m_index1 = data.index;
            m_isValidStart = roomData.graph[m_index1] != null;

            m_isSelectionValid = false;
            SetIndicatorSingleTile(roomData, data.index);
        }

        private void SetIndicatorSingleTile(in RoomData roomData, Index index)
        {
            roomData.indicator.Clear(index);
            roomData.indicator.SetArea(index);
            roomData.indicator.Color = m_isValidStart ? roomData.colorBlue : roomData.colorOff;
            m_index1 = m_index2 = index;
            m_isSelectionValid = false;
        }
        
        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (!data.WasIndexChanged) {
                return;
            }

            if (m_index1 == data.index) {
                m_isSelectionValid = false;
                SetIndicatorSingleTile(roomData, data.index);
            } else {
                if (Index.AreAdjacent(m_index1, data.index)) {
                    m_index2 = data.index;
                    roomData.indicator.SetArea(new IndexBounds(m_index1, m_index2));
                }

                // CheckConnectionValidity(in roomData, in data);
                m_isSelectionValid = roomData.graph.CanLink(m_index1, m_index2);
            }

            roomData.indicator.Color = m_isSelectionValid ? roomData.colorOn : roomData.colorOff;
        }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.Clear();
            roomData.indicator.IsDrawing = false;

            if (!m_isSelectionValid) {
                return;
            }

            roomData.graph.Link(m_index1, m_index2);
        }

        public override void OnSubtractStarted(in RoomData roomData, in EventData data)
        {
            InitMove(in roomData, in data);
            
            roomData.indicator.Color = roomData.colorOff;
        }

        public override void OnSubtractUpdate(in RoomData roomData, in EventData data)
        {
            if (!data.WasIndexChanged) {
                return;
            }

            if (m_index1 == data.index) {
                m_isSelectionValid = false;
                SetIndicatorSingleTile(roomData, data.index);
            } else {
                if (Index.AreAdjacent(m_index1, data.index)) {
                    m_index2 = data.index;
                    roomData.indicator.SetArea(new IndexBounds(m_index1, m_index2));
                }

                m_isSelectionValid = roomData.graph.AreLinked(m_index1, m_index2);
            }

            roomData.indicator.Color = m_isSelectionValid ? roomData.colorOn : roomData.colorOff;
        }

        public override void OnSubtractEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.Clear();
            roomData.indicator.IsDrawing = false;
            
            if (!m_isSelectionValid) {
                return;
            }
            
            roomData.graph.Unlink(m_index1, m_index2);
        }

        public override bool OnAddCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            return true;
        }

        public override bool OnSubtractCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;

            return true;
        }


        public override void OnActivate(in RoomData roomData) { }

        public override void OnDeactivate(in RoomData roomData) { }
    }
}