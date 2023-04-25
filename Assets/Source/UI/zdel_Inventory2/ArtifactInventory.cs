using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ArtifactInventory : MonoBehaviour
    {

        public GameObject itemPrefab;

        protected zdelArtifact[] artifacts;
        private ArtifactManager manager;


        [Header("References")]
        public Transform container;
        public GameObject contents;

        // Start is called before the first frame update
        void Start()
        {
            manager = ArtifactManager.Instance;
        }

        private void OnEnable()
        {
            foreach(Transform child in container)
                GameObject.Destroy(child.gameObject);

            artifacts = ArtifactManager.Instance.GetComponentsInChildren<zdelArtifact>(true);

            foreach ( zdelArtifact artifact in artifacts )
            {
                GameObject temp = GameObject.Instantiate(itemPrefab, container);
                temp.name = artifact.GetLabel();
                var item = temp.GetComponent<ArtifactItem>();
                item.SetArtifact(artifact);
            }
        }

     


        private void OnDisable()
        {
            // TODO: Make sure artifact placements are properly cancelled!
        }


        // Update is called once per frame
        void Update()
        {
            contents.SetActive(manager.mode == ArtifactManager.Mode.None );
        }
    }
}
