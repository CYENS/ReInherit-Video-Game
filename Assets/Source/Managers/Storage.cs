using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Keeps track of all the artifacts the museum has.
    /// 
    /// </summary>
    public class Storage : MonoBehaviour
    {

        [SerializeField]
        private List<zdelArtifact> storedArtifacts;
        
        [SerializeField]
        private List<zdelArtifact> borrowedArtifacts;

        
        
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
