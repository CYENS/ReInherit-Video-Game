using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class Exhibit : MonoBehaviour
    {


        private ArtifactInfo m_info;


        private bool ghost = false;
        private float timer = 0;

        [Header("Game Data")]
        [Range(0.1f,0.9f)]
        [SerializeField]
        [Tooltip("How well protected the artifact is")]
        private float protection = 0.1f;

        public float Protection
        {
            get => protection;
        }


        [Header("References")]
        public Collider navCollider;
        public GameObject placeholder;
        private Animator animator;
        private GameObject prop;
        private Selectable selectable;
        private Artifact artifact;


        [SerializeField]
        private GameObject m_BoxDissolve;


        [Header("UI Element References")]
        public Transform healthMeter;
        public Transform healthMeter2;
        public GameObject uiExhibit;
        public GameObject uiRestore;


        [Tooltip("Where people stand to look at the exhibit label")]
        public Transform standPoint;

        private Ghostify[] ghosties;
        private Collider[] colliders;



        public GameObject GetBoxDissolve() => m_BoxDissolve;

        public Artifact GetArtifact() => artifact;

        /// <summary>
        /// "Factory" function that generates exhibit cases based on a prefab,
        /// Useful for setting up the artifact data without using setters.
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="prefab"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Exhibit Create(GameObject owner, GameObject prefab, ArtifactInfo info)
        {
            GameObject temp = GameObject.Instantiate(prefab, owner.transform);
            temp.transform.localPosition = Vector3.zero;
            Exhibit exhibit = temp.GetComponent<Exhibit>();
            exhibit.m_info = info;
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
            if (prop == null)
                prop = CreateProp();

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
            foreach(var collider in colliders )
                collider.isTrigger = value;

            if(selectable == null)
                selectable = GetComponent<Selectable>();

            selectable.enabled = !value;
        }




        public void Upgrade()
        {
            if (artifact.upgraded)
                return;

            int funds = GameManager.Funds;
            int price = m_info.upgradePricing;

            if( funds < price )
            {
                // Cannot buy. TODO: Play sounds or something
                return;
            }

            GameManager.Funds -= price;
            artifact.Upgrade();
        }

        /// <summary>
        /// Sends the exhibit to be fixed / restored
        /// </summary>
        public void Restore()
        {
            if (artifact.condition > 1.0f - float.Epsilon)
                return;

            int funds = GameManager.Funds;
            int price = 100;

            if( funds < price )
            {
                ErrorMessage.Instance.CreateErrorMessage("Cannot restore artifact", "Not enough funds to afford it.");
                return;
            }

            GameManager.Funds -= price;
            artifact.SetStatus(Artifact.Status.Restoration);
        }

        private GameObject CreateProp()
        {
            prop = GameObject.Instantiate(m_info.artifactPrefab, placeholder.transform.position, Quaternion.identity);
            prop.transform.SetParent(placeholder.transform.parent);

            // Remove the placeholder as it is not needed
            Destroy(placeholder);

            return prop;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();

            // Place the artifact
            if(prop == null )
                prop = CreateProp();

            artifact = GetComponentInParent<Artifact>();
        }


        private void Update()
        {
            // Make artifact in display case invisible if it is in restoration room.
            bool inRestoration = (artifact.GetStatus() == Artifact.Status.Restoration);
            prop.SetActive(!inRestoration);


            // --- UI element management ---

            uiExhibit.SetActive(!inRestoration);
            uiRestore.SetActive(inRestoration);

            // Update meter
            healthMeter.localScale = new Vector3(artifact.condition, 1.0f, 1.0f);
            healthMeter2.localScale = healthMeter.localScale;
        }

    }
}
