using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Stores all the information relevant to the specific artifact.
    /// </summary>
    
    [CreateAssetMenu(fileName = "ArtifactInfo", menuName = "Data/Artifact", order = 1)]
    public class ArtifactInfo : ScriptableObject
    {
        [Header("General Information")]
        public string label;
        public Sprite icon;
        public enum Type { Statue, Scroll, Document, Pottery  }
        public Type type;
        
        public string description;

        [Header("Runtime variables")]
        [Tooltip("How novel the specific artifact is")]
        [SerializeField]
        private float novelty;

        public float Novelty
        {
            get => novelty;
            set => novelty = Mathf.Clamp(value,0.0f,1.0f);
        }


        [Header("Physical")]

        [Tooltip("The object containing the mesh filter and renderer")]
        public GameObject artifactPrefab;

        [Tooltip("The base version of the exhibit")]
        public GameObject exhibitPrefab01;

        [Tooltip("The upgraded version of the exhibit")]
        public GameObject exhibitPrefab02;
        
        [Tooltip("How much it costs to upgrade the exhibit case")]
        public int upgradePricing;

        private void Awake()
        {
            novelty = 1.0f;
        }
    }
}
