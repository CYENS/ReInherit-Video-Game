using Cyens.ReInherit.Extensions;
using UnityEngine;

namespace Cyens.ReInherit {
    public struct MouseDragHelper {
        private Vector2 m_position;
        private Vector2 m_startPosition;
        private Vector2 m_delta;
        private bool m_isMoving;
        private bool m_isValid;
        private int m_button;

        public void Begin() {
            m_startPosition = m_position = Input.mousePosition;
            m_isMoving = true;
            m_isValid = true;
        }

        public int ButtonId {
            get => m_button;
            set => m_button = value;
        }

        public void End() {
            m_isMoving = false;
        }

        public bool IsValid => m_isValid;

        public bool Update() {
            if (Input.GetMouseButton(m_button)) {
                if (m_isMoving) {
                    m_isValid = true;
                    m_delta = Input.mousePosition.xy() - m_position;
                    m_position = Input.mousePosition;
                    return true;
                }

                m_isValid = false;
            } else {
                m_isValid = true;
            }

            End();
            return false;
        }

        public Vector2 Delta => m_delta;

        public bool IsDragging => m_isMoving;

        public Vector2 StartPosition => m_startPosition;
    }
}