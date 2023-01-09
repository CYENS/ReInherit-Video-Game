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

        public void SetArtifact(Artifact artifact)
        {
            this.artifact = artifact;
            data = artifact.GetData();

            icon.sprite = data.icon;

            Refresh();
        }


        public void Refresh()
        {
            bool inStorage = artifact.GetStatus() == Artifact.Status.Storage;
            icon.color = inStorage ? Color.white : Color.gray;
        }
        
        // Ready to place artifact
        public void Place()
        {
            if (PlacementManager.Instance.active) return;
            if (artifact.GetStatus() != Artifact.Status.Storage) return;


            //PlacementManager.Instance.Begin( gameObject, data.mesh, Vector3.one, artifact, )
        }

    }
}
