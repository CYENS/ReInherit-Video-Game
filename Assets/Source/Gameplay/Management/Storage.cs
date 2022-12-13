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
        private List<Artifact> storedArtifacts;
        
        [SerializeField]
        private List<Artifact> borrowedArtifacts;

        
        
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
