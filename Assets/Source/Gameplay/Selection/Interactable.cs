using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Interactable : MonoBehaviour
    {

        public enum Type
        {
            None = 0,
            Elevator = 1
        }

        [Tooltip("The type of interaction")]
        private Type type;

        public Type GetInteractionType() => type;
        
        
        private HighlightController[] m_highlightControllers;
        
        
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
