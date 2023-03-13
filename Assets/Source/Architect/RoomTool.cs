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
            public Color colorBlue;
        }

        public struct EventData
        {
            public bool isValid;

            public Index lastIndex;
            public Index index;
            public Vector2 fractional;
            public Vector2 lastFractional;
            public Vector3 lastPoint;
            public Vector3 point;

            public bool WasIndexChanged => index != lastIndex;
        }

        protected bool LineScan(in RoomData roomData, in EventData data, ScanCallback callback)
        {
            var min = new Index(Mathf.Min(data.index.x, data.lastIndex.x), Mathf.Min(data.index.y, data.lastIndex.y));
            var max = new Index(Mathf.Max(data.index.x, data.lastIndex.x), Mathf.Max(data.index.y, data.lastIndex.y));
            
            var increment = (data.fractional - data.lastFractional).normalized;

            // We don't add lastIndex because we assume it was added last time
            var current = data.lastIndex;
            var currentFrac = data.lastFractional;

            var dataChanged = false;

            while (true) {
                var index = new Index(Mathf.RoundToInt(currentFrac.x), Mathf.RoundToInt(currentFrac.y));
                
                if (index.x < min.x || index.x > max.x || index.y < min.y || index.y > max.y) {
                    break;
                }

                if (index != current) {
                    dataChanged |= callback.Invoke(roomData, index);
                    current = index;
                }

                currentFrac += increment;
            }

            if (current != data.index) {
                // This can happen due to fractional errors
                dataChanged |= callback.Invoke(roomData, data.index);
            }

            return dataChanged;
        }

        protected delegate bool ScanCallback(in RoomData roomData, Index index);
    }
}