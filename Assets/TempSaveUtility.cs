using Cyens.ReInherit.Architect;
using Cyens.ReInherit.Serialization;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class TempSaveUtility : MonoBehaviour
    {
        // This is used just for demonstrating the save/load capabilities.
        
        private RoomGraph m_roomGraph;

        private void Start()
        {
            m_roomGraph = FindObjectOfType<RoomGraph>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) {
                GameSerializer.Save(m_roomGraph);
            } else if (Input.GetKeyDown(KeyCode.F2)) {
                GameSerializer.Load(m_roomGraph);
            }
        }
    }
}