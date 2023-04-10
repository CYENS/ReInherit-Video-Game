using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class ArtifactItem : MonoBehaviour
    {

        public zdelArtifact artifact;
        protected ArtifactInfo info;

        [Header("References")]
        public Image icon;
        public Image stripesExhibition;
        public Image stripesRestoration;


        private ArtifactInventory inventory;
        private ArtifactManager manager;
            
        public void SetArtifact(zdelArtifact artifact)
        {
            this.artifact = artifact;
            info = artifact.GetInfo();

            icon.sprite = info.icon;

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
            bool inStorage = artifact.GetStatus() == zdelArtifact.Status.Storage;
            bool inConservation = artifact.GetStatus() == zdelArtifact.Status.Restoration;
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
                case zdelArtifact.Status.Storage:
                    manager.PlaceArtifact(artifact);
                    break;

                case zdelArtifact.Status.Transit:
                case zdelArtifact.Status.Exhibit:
                case zdelArtifact.Status.Design:
                    // TODO: Move the camera to the spot where the artifact is placed.
                    Camera camera = Camera.main;
                    camera.transform.position = artifact.transform.position + Vector3.up * 5 + Vector3.forward * 3 + Vector3.left * 3;
                    camera.transform.LookAt(artifact.transform);
                    break;

                case zdelArtifact.Status.Restoration:
                    // TODO: Take players to the Conservation tab and highlight the artifact
                    break;
                    
            }

            

            
        }

    }
}
