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
        private Transform m_exhibitsParent;
        private List<Vector3> m_exhibits;
        private Vector3 m_checkpoint;
        [SerializeField] private bool m_rotateLeft = false;
        [SerializeField] private bool m_rotateRight = false;
        [SerializeField] private bool m_rotate180 = false;
        
        public List<Vector3> GetExhibits()
        {
            return m_exhibits;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            if(transform.Find("Sign") != null)
                m_sign = transform.Find("Sign").gameObject;
            if (transform.Find("Exhibits") != null)
                m_exhibitsParent = transform.Find("Exhibits");
            GetCheckpoints();
            AssignCheckpoint();
            AssignExhibits();
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

        private void AssignExhibits()
        {
            if (m_exhibitsParent == null) {
                m_exhibits = null;
                return;
            }
            
            m_exhibits = new List<Vector3>();
            m_exhibits.Add(m_exhibitsParent.GetChild(0).position);

            int nextIndex = 0;
            while(m_exhibits.Count < m_exhibitsParent.childCount){
                float dis = Single.PositiveInfinity;
                int index = 0;
                for (int i = 0; i < m_exhibitsParent.childCount; i++) {
                    if (nextIndex != i && !m_exhibits.Contains(m_exhibitsParent.GetChild(i).position)) {
                        float disTemp = Vector3.Distance(m_exhibitsParent.GetChild(nextIndex).position,
                            m_exhibitsParent.GetChild(i).position);
                        if (disTemp < dis) {
                            dis = disTemp;
                            index = i;
                        }
                    }
                }
                m_exhibits.Add(m_exhibitsParent.GetChild(index).position);
                nextIndex = index;
            }

        }
    }
}
