using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public abstract class RoomTool : MonoBehaviour
    {
        public virtual void OnAddStarted(in RoomData roomData, in EventData data) { }
        public virtual void OnAddUpdate(in RoomData roomData, in EventData data) { }
        public virtual void OnAddEnded(in RoomData roomData, in EventData data) { }
        public virtual bool OnAddCanceling(in RoomData roomData, in EventData data) => false;

        public virtual void OnSubtractStarted(in RoomData roomData, in EventData data) { }
        public virtual void OnSubtractUpdate(in RoomData roomData, in EventData data) { }
        public virtual void OnSubtractEnded(in RoomData roomData, in EventData data) { }
        public virtual bool OnSubtractCanceling(in RoomData roomData, in EventData data) => false;

        public virtual void OnActivate(in RoomData roomData) { }

        public virtual void OnDeactivate(in RoomData roomData) { }

        public struct RoomData
        {
            public RoomGraph graph;
            public GridIndicator indicator;
            public Color colorOff;
            public Color colorOn;
        }
        
        public struct EventData
        {
            public bool isValid;

            public Index lastIndex;
            public Index index;
            public Vector3 lastPoint;
            public Vector3 point;

            public bool WasIndexChanged => index != lastIndex;
        }
    }
}