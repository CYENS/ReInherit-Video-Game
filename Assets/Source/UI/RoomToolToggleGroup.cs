using Cyens.ReInherit.Architect;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit.UI
{
    [RequireComponent(typeof(ToggleGroup))]
    public class RoomToolToggleGroup : MonoBehaviour
    {
        private RoomToolToggle m_currentActive;
        private ToggleGroup m_group;

        private RoomTool noTool;
        private RoomToolToggle m_lastTool;
        private RoomToolToggle m_tempoaryActive;
        private RoomToolToggle[] m_toggles;

        private GridInputHandler m_inputHandler;

        public ToggleGroup Group => m_group;

        private void Awake()
        {
            // TODO: Find better way to get this
            m_inputHandler = FindObjectOfType<GridInputHandler>();

            m_group = GetComponent<ToggleGroup>();
            m_toggles = GetComponentsInChildren<RoomToolToggle>();
            foreach (var toggle in m_toggles) {
                toggle.Init(this);
            }

            noTool = m_toggles[0].Tool;
        }

        /// <summary>
        /// Removes room building functionality when this object is set inactive
        /// </summary>
        private void OnDisable()
        {
            m_inputHandler.SelectedTool = noTool;
        }

        /// <summary>
        /// Re-enables last functionality when the object is reactivated.
        /// </summary>
        private void OnEnable()
        {
            if( m_lastTool != null )
                m_inputHandler.SelectedTool = m_lastTool.Tool;
        }

        // private void Update()
        // {
        //     var selectedTool = m_inputHandler.SelectedTool;
        //     if (m_lastTool.Tool == selectedTool) {
        //         return;
        //     }
        //
        //     foreach (var toggle in m_toggles) {
        //         if (toggle.Tool == selectedTool) {
        //             toggle.Toggle.isOn = true;
        //             return;
        //         }
        //     }
        // }

        public void NotifyActivate(RoomToolToggle toggle)
        {
            m_lastTool = toggle;
            m_inputHandler.SelectedTool = toggle.Tool;
        }
    }
}