using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Exhibit : MonoBehaviour
    {
        public Transform placementPoint;
        public Artifact artifact;

        private void Awake()
        {
            
        }

        public void AddArtifact(Artifact artifact)
        {
            this.artifact = artifact;
            artifact.transform.position = placementPoint.position;

        }
    }
}
