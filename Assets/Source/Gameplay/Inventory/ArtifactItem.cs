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


        public void Refresh()
        {
            bool inStorage = artifact.GetStatus() == Artifact.Status.Storage;
            icon.color = inStorage ? Color.white : Color.gray;
        }
        
        // Ready to place artifact
        public void Place()
        {
            if (manager.mode != ArtifactManager.Mode.None) return;
            if (artifact.GetStatus() != Artifact.Status.Storage) return;


            manager.PlaceArtifact(artifact);
        }

    }
}
