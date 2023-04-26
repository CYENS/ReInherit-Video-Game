using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A script component, attached to a game object in order to embed various useful 
    /// information. Such as icons, descriptions, labels, etc.
    /// </summary>
    public class MetaData : MonoBehaviour
    {

        public Sprite icon;

        public string label;

        public string description;
        
        public Mesh mesh;

        public int siblingIndex => transform.GetSiblingIndex();
        
    }
}
