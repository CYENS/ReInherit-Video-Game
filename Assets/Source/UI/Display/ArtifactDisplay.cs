using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{
    public class ArtifactDisplay : MonoBehaviour
    {

        public enum Source { Current = 0, Reference = 1 }

        [SerializeField] private Source m_source;


        [Header("References")]

        [SerializeField] private ExhibitInfo m_info;
        private ExhibitManager m_exhibitManager;

        [SerializeField] private Image m_image;
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_description;



        // Start is called before the first frame update
        void Start()
        {
            m_exhibitManager = ExhibitManager.Instance;

            switch(m_source)
            {
                case Source.Current:
                    m_info = m_exhibitManager.NextInfo();
                break;
            }


            if( m_image != null )
            {
                m_image.sprite = m_info.m_thumb;
            }
            if( m_title != null )
            {
                m_title.text = m_info.label;
            }
            if( m_description != null )
            {
                m_description.text = m_info.description;
            }
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
