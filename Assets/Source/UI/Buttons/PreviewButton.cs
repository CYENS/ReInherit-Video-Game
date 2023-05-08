using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Exhibition;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class PreviewButton : MonoBehaviour
    {

        [Header("References")]

        [SerializeField]
        private ExhibitCase m_case;


        public void Preview()
        {
            Vector3 point = m_case.Placement;
            PreviewManager.Preview(point);
        }

        // Start is called before the first frame update
        void Start()
        {
            m_case = GetComponentInParent<ExhibitCase>();
            
        }

    }
}
