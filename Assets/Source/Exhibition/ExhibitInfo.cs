using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cyens.ReInherit.Exhibition
{
    /// <summary>
    /// Holds information about the appearance of the exhibit, the artifact model inside of it,
    /// as well as game related parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "ExhibitInfo", menuName = "Data/ExhibitInfo", order = 0)]
    public class ExhibitInfo : ScriptableObject 
    {
        
        [Header("Artifact")]

        [Tooltip("The model of the artifact that will be placed in the exhibit case")]
        public GameObject m_artifact;

        [Tooltip("Artifact thumbnail / icon")]
        public Sprite m_thumb;


        [Tooltip("A mesh used for visualization purposes")]
        public Mesh m_mesh;

        [Header("Gameplay")]

        public float test;


        #if UNITY_EDITOR
        private void OnValidate() 
        {

            if( m_artifact == null )
            {
                Debug.LogWarning("Exhibit info "+this+" does not have an artifact model reference");
                return;
            }
            
            var artifact = m_artifact.GetComponent<Artifact>();
            if( artifact == null )
            {
                Debug.LogWarning("Given game object "+m_artifact+" does not have an Artifact script");
                m_artifact = null;
                return;
            }
        }
        #endif

    }

}
