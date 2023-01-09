using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ExhibitSlot : MonoBehaviour
    {
        protected MeshFilter filter;
        protected new MeshRenderer renderer;

        public Artifact artifact;

        // Start is called before the first frame update
        void Start()
        {
            filter = GetComponentInChildren<MeshFilter>();
            renderer = GetComponentInChildren<MeshRenderer>();

        }

        public bool Place( Artifact artifact )
        {
            if (this.artifact != null) return false;

            this.artifact = artifact;
            filter.sharedMesh = artifact.GetMesh();
            
            return true;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
