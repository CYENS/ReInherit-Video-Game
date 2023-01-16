using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class Exhibit : MonoBehaviour
    {


        private ArtifactData data;


        private bool ghost = false;


        [Header("References")]
        public GameObject placeholder;
        private Animator animator;
        private GameObject artifact;
        private Selectable selectable;
        private Artifact owner;


        [Tooltip("Where people stand to look at the exhibit label")]
        public Transform standPoint;

        private Ghostify[] ghosties;
        private Collider[] colliders;


        /// <summary>
        /// "Factory" function that generates exhibit cases based on a prefab,
        /// Useful for setting up the artifact data without using setters.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="prefab"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Exhibit Create(GameObject owner, GameObject prefab, ArtifactData data)
        {
            GameObject temp = GameObject.Instantiate(prefab, owner.transform);
            temp.transform.localPosition = Vector3.zero;
            Exhibit exhibit = temp.GetComponent<Exhibit>();
            exhibit.data = data;
            return exhibit;
        }

        public void Place()
        {
            animator.Play("Place");
        }


        public void SetGhost( bool value, Color color )
        {
            // Wait for the artifact to spawn first
            // Place the artifact
            if (artifact == null)
                artifact = CreateArtifact();

            if (ghosties == null)
                ghosties = GetComponentsInChildren<Ghostify>(true);

            if (colliders == null)
                colliders = GetComponentsInChildren<Collider>(true);


            ghost = value;
            foreach(var ghostie in ghosties)
            {
                ghostie.enabled = value;
                if (value) ghostie.SetColor(color);
            }

            // Ghosts should be passable!
            //foreach(var collider in colliders )
            //    collider.isTrigger = value;

            if(selectable == null)
                selectable = GetComponent<Selectable>();

            selectable.enabled = !value;
        }


        public void Upgrade()
        {
            if (owner.upgraded)
                return;

            int funds = GameManager.GetFunds();
            int price = data.upgradePricing;

            if( funds < price )
            {
                // Cannot buy. TODO: Play sounds or something
                return;
            }

            GameManager.SetFunds(funds - price);
            owner.Upgrade();
        }

        private GameObject CreateArtifact()
        {
            artifact = GameObject.Instantiate(data.artifactPrefab, placeholder.transform.position, Quaternion.identity);
            artifact.transform.SetParent(placeholder.transform.parent);

            // Remove the placeholder as it is not needed
            Destroy(placeholder);

            return artifact;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();

            // Place the artifact
            if( artifact == null ) 
                artifact = CreateArtifact();

            owner = GetComponentInParent<Artifact>();
        }


    }
}
