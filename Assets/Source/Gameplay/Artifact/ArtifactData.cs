using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{

    /// <summary>
    /// A serializable data object used to save data for a specific artifact.
    /// </summary>
    [System.Serializable]
    public class ArtifactData
    {
        // Where the artifact exhibit case is located
        public Vector3 position;

        // How the artifact exhibit case is oriented (Y-axis rotation)
        public float angle;

        // Reference if to the info object
        public string infoId;

        // The current status of the object
        // (enum casted as int)
        public int status;

        // The upgrade status of the artifact exhibit case
        public bool upgraded;

        // The current condition of the artifact
        public float condition;
    }
}
