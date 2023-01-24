using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class ArtifactItem : MonoBehaviour
    {

        public Artifact artifact;
        protected ArtifactData data;

        [Header("References")]
        public Image icon;
        public Image stripesExhibition;
        public Image stripesRestoration;


        private ArtifactInventory inventory;
        private ArtifactManager manager;
            
        public void SetArtifact(Artifact artifact)
        {
            this.artifact = artifact;
            data = artifact.GetData();

            icon.sprite = data.icon;

            Refresh();

            inventory = GetComponentInParent<ArtifactInventory>(true);
            manager = ArtifactManager.Instance;
        }

        private void OnEnable()
        {
            if (artifact == null) return;
            Refresh();
        }

        public void Refresh()
        {
            // Note: Moved code in Update. It's less efficient, but it works correctly without a lot of complications.
        }

        private void Update()
        {
            bool inStorage = artifact.GetStatus() == Artifact.Status.Storage;
            bool inConservation = artifact.GetStatus() == Artifact.Status.Restoration;
            bool inExhibition = !inStorage && !inConservation;

            icon.color = inStorage ? Color.white : Color.gray;
            stripesRestoration.gameObject.SetActive(inConservation);
            stripesExhibition.gameObject.SetActive(inExhibition);
        }


        // Ready to place artifact
        public void Place()
        {
            if (manager.mode != ArtifactManager.Mode.None) return;

            switch ( artifact.GetStatus() )
            {
                case Artifact.Status.Storage:
                    manager.PlaceArtifact(artifact);
                    break;

                case Artifact.Status.Transit:
                case Artifact.Status.Exhibit:
                case Artifact.Status.Design:
                    // TODO: Move the camera to the spot where the artifact is placed.
                    Camera camera = Camera.main;
                    camera.transform.position = artifact.transform.position + Vector3.up * 5 + Vector3.forward * 3 + Vector3.left * 3;
                    camera.transform.LookAt(artifact.transform);
                    break;

                case Artifact.Status.Restoration:
                    // TODO: Take players to the Conservation tab and highlight the artifact
                    break;
                    
            }

            

            
        }

    }
}
