namespace Cyens.ReInherit.Architect
{
    public class DeleteRoomTool : RoomTool
    {
        public override void OnAddStarted(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = true;
            roomData.indicator.Clear(data.index);
            roomData.indicator.Color = roomData.colorOn;
        }

        public override void OnAddUpdate(in RoomData roomData, in EventData data)
        {
            if (data.WasIndexChanged) {
                roomData.indicator.SetArea(data.index);
            }
        }

        public override void OnAddEnded(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            var bounds = roomData.indicator.InclusiveBounds;
            
            roomData.graph.RemoveArea(bounds);
        }
        
        
        public override bool OnAddCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            return true;
        }
    }
}