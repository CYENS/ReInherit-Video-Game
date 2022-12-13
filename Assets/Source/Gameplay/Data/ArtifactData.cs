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
        public string label;

        public Sprite icon;
        public enum Type { Statue, Scroll, Document, Pottery  }
        public Type type;
        
        public string description;
    }
}
