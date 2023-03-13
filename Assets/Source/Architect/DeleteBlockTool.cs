namespace Cyens.ReInherit.Architect
{
    public class DeleteBlockTool : RoomTool
    {
        private bool DoRemove(in RoomData roomData, Index index)
        {
            // var graph = roomData.graph;
            // var block = graph.GetBlockAtIndex(index);
            // if (block == null) {
            //     return false;
            // }
            //
            // if (block.Parent.CanDelete(block)) {
            //     graph.RemoveBlock(index);
            //     return true;
            // }
            //
            // return false;
            return false;
        }

        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = true;
            roomData.indicator.Clear(data.index);

            roomData.indicator.SetArea(data.index);

            DoRemove(in roomData, data.index);

            roomData.indicator.Color = roomData.colorOn;
        }

        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (!data.WasIndexChanged) {
                return;
            }

            roomData.indicator.Clear(data.index);
            // DoRemove(in roomData, data.index);

            LineScan(roomData, data, DoRemove);
       }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            // roomData.graph.GetRoomsAt(roomData.indicator.InclusiveBounds, m_buffer);
            //
            // foreach (var room in m_buffer) {
            //     Destroy(room);
            // }
        }
    }
}