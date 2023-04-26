using Cyens.ReInherit.Managers;
using Pathfinding;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class BlockModel : MonoBehaviour
    {
        [SerializeField] private DirectionList<WallModel> wallModels;

        [SerializeField] private PillarModel northEastPillar;
        [SerializeField] private PillarModel northWestPillar;
        [SerializeField] private PillarModel southEastPillar;
        [SerializeField] private PillarModel southWestPillar;

        [SerializeField] private bool moveToIndex;
        [SerializeField] private Index startIndex;

        public void Clear()
        {
            foreach (var dir in Direction.Values) {
                wallModels[dir].Type = WallModel.WallType.None;
            }

            northEastPillar.Clear();
            northWestPillar.Clear();
            southEastPillar.Clear();
            southWestPillar.Clear();
        }

        public WallModel.WallType GetModelType(Direction direction)
        {
            return wallModels[direction].Type;
        }

        public void SetModelType(Direction direction, WallModel.WallType type)
        {
            if (direction.Id == DirectionId.South) {
                var index = Index.FromWorld(transform.position);
                var entranceIndex1 = new Index(Index.MaxSizeX / 2, 0);
                var entranceIndex2 = entranceIndex1.East;

                if (index == entranceIndex1 || index == entranceIndex2) {
                    type = WallModel.WallType.None;
                }
            }

            var model = wallModels[direction];
            if (model != null) {
                model.Type = type;
            }

            northEastPillar.NotifyWallTypeChanged(direction, type);
            northWestPillar.NotifyWallTypeChanged(direction, type);
            southEastPillar.NotifyWallTypeChanged(direction, type);
            southWestPillar.NotifyWallTypeChanged(direction, type);
        }

        public WallModel.WallType EastType
        {
            get => wallModels[Direction.East].Type;
            set => wallModels[Direction.East].Type = value;
        }

        public WallModel.WallType WestType
        {
            get => wallModels[Direction.West].Type;
            set => wallModels[Direction.West].Type = value;
        }

        public WallModel.WallType NorthType
        {
            get => wallModels[Direction.North].Type;
            set => wallModels[Direction.North].Type = value;
        }

        public WallModel.WallType SouthType
        {
            get => wallModels[Direction.South].Type;
            set => wallModels[Direction.South].Type = value;
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
            // TODO: Fix this
            transform.Find("Floor").GetComponent<Collider>().enabled = false;
            // GetComponent<GraphUpdateScene>().Apply();
        }

        public void RecreateNavMesh()
        {
            // TODO: Fix this
            transform.Find("Floor").GetComponent<Collider>().enabled = true;
            // GetComponent<GraphUpdateScene>().Apply();
        }

        private void OnValidate()
        {
            if (moveToIndex) {
                MoveToIndex(startIndex);
            }
        }
    }
}