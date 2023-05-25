using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{

    /// <summary>
    /// A utility for finding object references at runtime.
    /// Has a similar purpose to Unity's Tag system, but doesn't use the tag list,
    /// as it can get messy and crowded with various tags.
    /// </summary>
    public class Reference : MonoBehaviour
    {

        [SerializeField] private string m_label;


        public static GameObject Find( string label )
        {
            var allReferences = Object.FindObjectsOfType<Reference>();
            foreach( var reference in allReferences )
            {
                if( reference.m_label.Equals(label) )
                {
                    return reference.gameObject;
                }

            }
            return null;
        }

    }
}
