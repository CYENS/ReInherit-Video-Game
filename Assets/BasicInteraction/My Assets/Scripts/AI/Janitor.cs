using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Janitor : MonoBehaviour
    {
        [SerializeField] private List<Vector3> m_movingPoints;

        private void Awake()
        {
            transform.Find("blank_body_Sphere").GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

        public void SetMovingPoints(List<Vector3> points)
        {
            m_movingPoints = new List<Vector3>(points);
        }

        public List<Vector3> GetMovingPoints()
        {
            return m_movingPoints;
        }
    }
}
