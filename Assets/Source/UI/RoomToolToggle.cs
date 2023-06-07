using Cyens.ReInherit.Architect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit.UI
{
    public class RoomToolToggle : MonoBehaviour
    {
        private RoomToolToggleGroup m_parent;

        private Toggle m_toggle;
        private RoomTool m_tool;
        private TextMeshProUGUI m_text;

        public Toggle Toggle => m_toggle == null ? m_toggle = GetComponentInChildren<Toggle>() : m_toggle;

        public RoomTool Tool => m_tool == null ? m_tool = GetComponentInChildren<RoomTool>() : m_tool;

        private void Awake()
        {
            Toggle.onValueChanged.AddListener(ToggleCallback);
        }

        private void Start()
        {
            m_text = GetComponentInChildren<TextMeshProUGUI>();
            ToggleCallback(m_toggle.isOn);
        }

        private void ToggleCallback(bool value)
        {
            if (value) {
                m_text.color = Color.black;
                m_parent.NotifyActivate(this);
            }
            else {
                m_text.color = Color.white;
            }
                
        }

        public void Init(RoomToolToggleGroup parentGroup)
        {
            m_parent = parentGroup;
            Toggle.group = parentGroup.Group;
        }
    }
}