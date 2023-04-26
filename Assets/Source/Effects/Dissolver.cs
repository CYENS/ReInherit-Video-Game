using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A script that controls the dissolve shader effect.
    /// Once the dissolve effect is done, it disables the object.
    /// </summary>
    public class Dissolver : MonoBehaviour
    {

        private MeshRenderer m_renderer;
        private Material m_material;

        [Header("Parameters")]
        [SerializeField]
        [Tooltip("")]
        private string m_dissolveParamName = "_Dissolve_Amount";

        [SerializeField]
        [Tooltip("The speed that the shader is dissolving with")]
        private float m_dissolveSpeed = 5.0f;

        [Header("Variables")]
        [SerializeField]
        [Range(0,1)]
        [Tooltip("")]
        private float m_dissolve = 1.0f;



        // Start is called before the first frame update
        void Awake()
        {
            m_renderer = GetComponent<MeshRenderer>();
            
            // Create material instance
            m_material = m_renderer.material;
        }


        private void OnEnable() 
        {
            m_dissolve = 1.0f;
            Refresh();
        }

        /// <summary>
        /// Update shader uniform/parameters
        /// </summary>
        private void Refresh()
        {
            m_material.SetFloat( m_dissolveParamName, m_dissolve );
        }

        // Update is called once per frame
        void Update()
        {
            m_dissolve -= Time.deltaTime * m_dissolveSpeed;
            m_dissolve = Mathf.Max( m_dissolve, 0.0f );
            Refresh();

            if( m_dissolve < float.Epsilon )
            {
                m_dissolve = 0.0f;
                Refresh();
                gameObject.SetActive(false);
            }

        }
    }
}
