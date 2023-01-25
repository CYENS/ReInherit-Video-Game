using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [SelectionBase]
    public class WallModel : MonoBehaviour
    {
        [Serializable]
        public enum WallType
        {
            None,
            Wall,
            Door,
        }

        [SerializeField] private GameObject wall;
        [SerializeField] private GameObject door;

        [SerializeField] private WallType wallType;


        private void Awake()
        {
            Refresh();
        }

        private void OnValidate()
        {
            if (wall == null || door == null) {
                return;
            }

            Refresh();
        }

        public WallType Type
        {
            get => wallType;
            set {
                wallType = value;
                Refresh();
            }
        }

        private void Refresh()
        {
            switch (wallType) {
                case WallType.Door:
                    wall.SetActive(false);
                    door.SetActive(true);
                    break;
                case WallType.Wall:
                    wall.SetActive(true);
                    door.SetActive(false);
                    break;
                case WallType.None:
                default:
                    wall.SetActive(false);
                    door.SetActive(false);
                    break;
            }
        }
    }
}