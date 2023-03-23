using System;
using Cyens.ReInherit.Gameplay.Management;
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

        public void Clear()
        {
            eastWall.Type = WallModel.WallType.None;   
            westWall.Type = WallModel.WallType.None;   
            northWall.Type = WallModel.WallType.None;   
            southWall.Type = WallModel.WallType.None;   
        }
        
        public WallModel.WallType GetModelType(Direction direction)
        {
            return direction.Id switch {
                DirectionId.East => EastType,
                DirectionId.West => WestType,
                DirectionId.North => NorthType,
                DirectionId.South => SouthType,
                _ => WallModel.WallType.None,
            };
        }
        
        public void SetModelType(Direction direction, WallModel.WallType type)
        {
            // TODO use DirectionList
            var model = direction.Id switch {
                DirectionId.East => eastWall,
                DirectionId.West => westWall,
                DirectionId.North => northWall,
                DirectionId.South => southWall,
                _ => null,
            };

            // If this room is connected to stage, remove south walls to merge them
            Vector2 entryRoom = GameManager.Instance.entryRoomCoordinates;
            if (Mathf.Approximately(transform.position.x, entryRoom.x) 
                && Mathf.Approximately(transform.position.z, entryRoom.y) 
                && direction.Id == DirectionId.South) { 
                model.Type = WallModel.WallType.None;
                    return;
            }

            if (model != null) {
                model.Type = type;
            }
        }

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