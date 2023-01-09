using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents a single artifact.
    /// </summary>
    public class Artifact : MonoBehaviour
    {
        [SerializeField]
        private ArtifactData data;


        public enum Status { Storage = 0, Transit = 1, Exhibit = 2, Restoration = 3  }

        [SerializeField]
        private Status status;


        public void SetStatus( Status newStatus )
        {
            status = newStatus;
            switch(status)
            {
                case Status.Storage:
                case Status.Restoration:
                    gameObject.SetActive(false); 
                break;
                case Status.Transit:
                case Status.Exhibit:
                    gameObject.SetActive(true);
                break;
            }

            // TODO: Switch between ghost and opaque material

        }

        // Start is called before the first frame update
        void Awake()
        {
            // Use data to create the object
            // Add the mesh
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = data.mesh;

            // Add the material
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = data.material;

            // Just a little hack until we get proper 3d models
            transform.rotation = Quaternion.Euler(-90, 0, 0);

            // Disable it?
        }



    }
}
