using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Stores all the information relevant to the specific artifact.
    /// </summary>
    
    [CreateAssetMenu(fileName = "ArtifactData", menuName = "Data/Artifact", order = 1)]
    public class ArtifactData : ScriptableObject
    {
        [Header("General Information")]
        public string label;
        public Sprite icon;
        public enum Type { Statue, Scroll, Document, Pottery  }
        public Type type;
        
        public string description;



        [Header("Physical")]

        [Tooltip("The object containing the mesh filter and renderer")]
        public GameObject artifactPrefab;

        [Tooltip("The base version of the exhibit")]
        public GameObject exhibitPrefab01;

        [Tooltip("The upgraded version of the exhibit")]
        public GameObject exhibitPrefab02;
        
        [Tooltip("How much it costs to upgrade the exhibit case")]
        public int upgradePricing;
        

    }
}
