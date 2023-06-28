using Cyens.ReInherit.Managers;
using Unity.VisualScripting;
using UnityEngine;

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
            Debug.Log(roomData.indicator);
            if (KeeperManager.Instance.GetActiveKeepers() > 0) {
                ErrorMessage.Instance.CreateErrorMessage("Cannot delete room",
                    "Cannot delete rooms while keepers moving.");
            }
            else {
                roomData.graph.RemoveArea(bounds);
            }
        }
        
        
        public override bool OnAddCanceling(in RoomData roomData, in EventData data)
        {
            roomData.indicator.IsDrawing = false;
            return true;
        }
    }
}