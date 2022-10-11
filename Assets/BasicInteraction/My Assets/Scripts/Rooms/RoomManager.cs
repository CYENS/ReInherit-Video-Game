using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class RoomManager : MonoBehaviour
    {
        private GameObject m_sign = null;
        private List<Collider> m_checkpoints;
        private Vector3 m_checkpoint;
        [SerializeField] private bool m_rotateLeft = false;
        [SerializeField] private bool m_rotateRight = false;
        [SerializeField] private bool m_rotate180 = false;
        
        // Start is called before the first frame update
        void Start()
        {
            if(transform.Find("Sign") != null)
                m_sign = transform.Find("Sign").gameObject;
            GetCheckpoints();
            AssignCheckpoint();
        }

        private void Update()
        {
            if (m_rotateLeft) {
                RotateSign(-90f);
                m_rotateLeft = false;
            }else if(m_rotateRight) {
                RotateSign(90f);
                m_rotateRight = false;
            }else if(m_rotate180) {
                RotateSign(180f);
                m_rotate180 = false;
            }
        }

        private void RotateSign(float degrees)
        {
            m_sign.transform.Rotate(0,degrees,0);
            AssignCheckpoint();
        }

        public Vector3 Checkpoint()
        {
            return m_checkpoint;
        }

        private void GetCheckpoints()
        {
            Transform parent = transform.Find("CheckPointColliders");
            if (parent == null)
                return;
            m_checkpoints = new List<Collider>(parent.childCount);
            foreach (Transform child in parent) {
                m_checkpoints.Add(child.GetComponent<BoxCollider>());
            }
        }

        private void AssignCheckpoint()
        {
            if(m_checkpoints == null || m_checkpoints.Count == 0 || m_sign == null)
                return;
            foreach (var ch in m_checkpoints) {
                Vector3 heading = ch.transform.position - m_sign.transform.position;
                float dot = Vector3.Dot(heading, m_sign.transform.forward);
                if (dot > 0.5f) {
                    m_checkpoint = ch.transform.position;
                    return;
                }
            }
        }
    }
}
