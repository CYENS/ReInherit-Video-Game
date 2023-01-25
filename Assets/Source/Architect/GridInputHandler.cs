using Cyens.ReInherit.Extensions;
using Cyens.ReInherit.Scene;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class GridInputHandler : MonoBehaviour
    {
        private bool m_isCanceled;

        private int m_lastButtonMode;

        private Index m_lastIndex;
        private Vector2 m_lastMousePosition;

        private RoomTool m_lastTileTool;

        private MouseDragHelper m_leftDragHelper;
        private MouseDragHelper m_middleDragHelper;
        private MouseDragHelper m_rightDragHelper;

        [SerializeField] private GridIndicator indicator;

        private TopdownCamera m_camera;

        public RoomGraph Graph
        {
            get => graph;
            set => graph = value;
        }

        public RoomTool SelectedTool { get; set; }

        private Vector3 m_lastGroundHit;

        public bool IsInUse { get; private set; }

        [SerializeField] private RoomGraph graph;

        [SerializeField] private Color indicatorEnabledColor = Color.green.WithAlpha(0.15f);

        [SerializeField] private Color indicatorDisabledColor = Color.red.WithAlpha(0.15f);

        private void Awake()
        {
            // TODO: Get from elsewhere
            m_camera = FindObjectOfType<TopdownCamera>();

            m_leftDragHelper.ButtonId = 0;
            m_rightDragHelper.ButtonId = 1;
            m_middleDragHelper.ButtonId = 2;
        }

        public void Update()
        {
            var roomData = new RoomTool.RoomData {
                graph = Graph,
                indicator = indicator,
                colorOn = indicatorEnabledColor,
                colorOff = indicatorDisabledColor,
            };

            var isLeftStarted = false;
            var isRightStarted = false;

            var isSelectionOnGround = m_camera.GroundCast(out var groundHit);

            var index = Index.FromWorld(groundHit);
            
            // Debug.Log(index);

            var data = new RoomTool.EventData {
                index = index,
                lastIndex = m_lastIndex,
                point = groundHit,
                lastPoint = m_lastGroundHit
            };
            var mousePosition = m_lastMousePosition;
            
            m_lastMousePosition = mousePosition;
            m_lastIndex = index;
            m_lastGroundHit = groundHit;

            if (m_camera.IsMouseWithinGame && isSelectionOnGround) {
                if (Input.GetMouseButtonDown(m_leftDragHelper.ButtonId) && m_lastButtonMode == -1) {
                    m_leftDragHelper.Begin();
                    data.lastIndex = data.index;
                    isLeftStarted = true;
                    m_lastButtonMode = m_leftDragHelper.ButtonId;
                    IsInUse = true;
                }

                if (Input.GetMouseButtonDown(m_middleDragHelper.ButtonId)) {
                    m_middleDragHelper.Begin();
                }

                //
                if (Input.GetMouseButtonDown(m_rightDragHelper.ButtonId) && m_lastButtonMode == -1) {
                    m_rightDragHelper.Begin();
                    data.lastIndex = data.index;
                    isRightStarted = true;
                    m_lastButtonMode = m_rightDragHelper.ButtonId;
                    IsInUse = true;
                }
            }
            
            var isLeftDown = Input.GetMouseButtonDown(m_leftDragHelper.ButtonId);
            var isRightDown = Input.GetMouseButtonDown(m_rightDragHelper.ButtonId);

            // Difference between is[?]Down and is[?]Started is that isStarted only triggers
            // When the user initiates on the view.

            // They always need to be updated
            var isLeftHeld = m_leftDragHelper.Update();
            var isMiddleHeld = m_middleDragHelper.Update();
            var isRightHeld = m_rightDragHelper.Update();
            
            if (!isLeftHeld && m_lastButtonMode == m_leftDragHelper.ButtonId) {
                // Whether to end "add" mode input
                if (SelectedTool != null && !m_isCanceled) {
                    SelectedTool.OnAddEnded(in roomData, in data);
                }

                m_lastButtonMode = -1;
            }

            if (!isRightHeld && m_lastButtonMode == m_rightDragHelper.ButtonId) {
                // Whether to end "subtract" mode input
                if (SelectedTool != null && !m_isCanceled) {
                    SelectedTool.OnSubtractEnded(in roomData, in data);
                }

                m_lastButtonMode = -1;
            }

            if (!isLeftHeld && !isRightHeld) {
                // Input ended for both buttons, reset state
                m_lastButtonMode = -1;
                m_isCanceled = false;
                IsInUse = false;
            }

            // Left and right buttons are analogous and work the same way, but:
            // - left uses "add" mode, right uses "substract" mode.
            // - left cancels right ("subtract") mode and right cancels left ("add") mode

            // The middle button works differently, it just does panning.
            // The wheel does zooming.

            if (isLeftHeld && !m_isCanceled && SelectedTool != null) {
                if (isLeftStarted) {
                    // TODO: Push undo history
                    SelectedTool.OnAddStarted(in roomData, in data);
                } else if (isRightDown) {
                    m_isCanceled = SelectedTool.OnAddCanceling(in roomData, in data);
                } else if (isSelectionOnGround) {
                    SelectedTool.OnAddUpdate(in roomData, in data);
                }
            } else if (isRightHeld && !m_isCanceled && SelectedTool != null) {
                if (isRightStarted) {
                    // TODO: Push undo history
                    SelectedTool.OnSubtractStarted(in roomData, in data);
                } else if (isLeftDown) {
                    m_isCanceled = SelectedTool.OnSubtractCanceling(in roomData, in data);
                } else if (isSelectionOnGround) {
                    SelectedTool.OnSubtractUpdate(in roomData, in data);
                }
            }
        }
    }
}