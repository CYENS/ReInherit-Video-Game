using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Ghostify : MonoBehaviour
    {

        [SerializeField]
        private Color color;

        [Header("References")]

        [SerializeField] [Tooltip("The base Ghost material effect to be used")]
        private Material m_ghost;
        private Material m_ghostInstance;

        private Renderer m_renderer;

        private Material[] m_originals;
        private Material[] m_ghosts;



        private void Awake()
        {
            // Attempt to grab the renderer component
            m_renderer = GetComponent<MeshRenderer>();
            if(m_renderer == null)
            {
                m_renderer = GetComponent<SkinnedMeshRenderer>();
                if(m_renderer == null)
                {
                    Debug.LogError("Cannot activate ghostify effect script: No renderer component is present in the gameobject");
                    return;
                }
            }

            // Keep a copy of the original materials
            m_originals = GetComponent<Renderer>().sharedMaterials;

            // Create instance of ghost material
            GetComponent<Renderer>().material = m_ghost;
            m_ghostInstance = GetComponent<Renderer>().material;
            GetComponent<Renderer>().sharedMaterial = m_originals[0];

            // Assign the array with the ghost material instance
            m_ghosts = new Material[m_originals.Length];
            for (int i = 0; i < m_ghosts.Length; i++)
                m_ghosts[i] = m_ghostInstance;

        }

        private void OnEnable()
        {
            GetComponent<Renderer>().sharedMaterials = m_ghosts;
            m_ghostInstance.color = color;
        }

        private void OnDisable()
        {
            GetComponent<Renderer>().sharedMaterials = m_originals;
        }


        public void SetColor( Color color )
        {
            this.color = color;
            m_ghostInstance.color = color;
        }

        public void SetAlpha( float alpha )
        {
            var newColor = this.color;
            newColor.a = Mathf.Clamp(alpha,0.0f,1.0f);
            SetColor(newColor);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //if(Application.isPlaying)
            //   SetColor(color);
        }
#endif

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
