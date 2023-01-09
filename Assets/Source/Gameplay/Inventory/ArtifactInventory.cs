using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ArtifactInventory : MonoBehaviour
    {

        public GameObject itemPrefab;

        protected Artifact[] artifacts;



        [Header("References")]
        public Transform container;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void OnEnable()
        {

            Debug.Log("Artifact Inventory Enabled");
            foreach(Transform child in container)
                GameObject.Destroy(child.gameObject);

            artifacts = ArtifactManager.Instance.GetComponentsInChildren<Artifact>(true);

            foreach( Artifact artifact in artifacts )
            {
                GameObject temp = GameObject.Instantiate(itemPrefab, container);
                temp.name = artifact.GetLabel();
                var item = temp.GetComponent<ArtifactItem>();
                item.SetArtifact(artifact);
                
            }

        }


        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
