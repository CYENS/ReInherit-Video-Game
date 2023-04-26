using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Attached to an object that represents the artifact.
    /// Useful in cases where a collision event hits a collider that doesn't have the artifact attached to it
    /// </summary>
    public class ArtifactProxy : MonoBehaviour
    {

        private zdelArtifact m_artifact;

        // Start is called before the first frame update
        void Awake()
        {
            m_artifact = GetComponentInParent<zdelArtifact>();
        }
    }
}
