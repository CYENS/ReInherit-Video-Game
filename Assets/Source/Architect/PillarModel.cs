using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class PillarModel : MonoBehaviour
    {
        [SerializeField] private Direction dir1;
        [SerializeField] private Direction dir2;

        private bool m_enable1;
        private bool m_enable2;

        public void Clear()
        {
            m_enable1 = false;
            m_enable2 = false;
            gameObject.SetActive(false);
        }

        public void NotifyWallTypeChanged(Direction direction, WallModel.WallType type)
        {
            if (direction == dir1) { m_enable1 = type != WallModel.WallType.None; }

            if (direction == dir2) { m_enable2 = type != WallModel.WallType.None; }

            gameObject.SetActive(m_enable1 || m_enable2);
        }
    }
}