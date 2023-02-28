using Pathfinding;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class BlockModel : MonoBehaviour
    {
        [SerializeField] private WallModel eastWall;
        [SerializeField] private WallModel westWall;
        [SerializeField] private WallModel northWall;
        [SerializeField] private WallModel southWall;

        [SerializeField] private bool moveToIndex;
        [SerializeField] private Index startIndex;

        public WallModel.WallType EastType 
        {
            get => eastWall.Type;
            set => eastWall.Type = value;
        }

        public WallModel.WallType WestType
        {
            get => westWall.Type;
            set => westWall.Type = value;
        }

        public WallModel.WallType NorthType
        {
            get => northWall.Type;
            set => northWall.Type = value;
        }

        public WallModel.WallType SouthType
        {
            get => southWall.Type;
            set => southWall.Type = value;
        }

        private void MoveToIndex(Index index)
        {
            var position = index.WorldCenter;
            var myTransform = transform;

            position.y = -0.125f;
            myTransform.position = position;
        }

        public void DisableFloorAndRemoveNavMesh()
        {
            transform.Find("Floor").GetComponent<Collider>().enabled = false;
            GetComponent<GraphUpdateScene>().Apply();
        }

        public void RecreateNavMesh()
        {
            transform.Find("Floor").GetComponent<Collider>().enabled = true;
            GetComponent<GraphUpdateScene>().Apply();
        }

        private void OnValidate()
        {
            if (moveToIndex) {
                MoveToIndex(startIndex);
            }
        }
    }
}