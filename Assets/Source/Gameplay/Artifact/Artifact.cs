using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents a single artifact.
    /// </summary>
    public class Artifact : MonoBehaviour
    {
        [SerializeField]
        private ArtifactData data;


        public enum Status { Storage = 0, Design = 1, Transit = 2, Exhibit = 3, Restoration = 4  }

        [SerializeField]
        private Status status;


        public Status GetStatus() => status;

        public ArtifactData GetData() => data;
        
        
        //public Mesh GetMesh() => data.mesh;
        
        public string GetLabel() => data.label;


        [Header("Game Data")]
        public bool upgraded = false;
        public float condition = 1.0f;


        [Header("References")]

        private Exhibit _exhibit01;
        private Exhibit _exhibit02;


        private Color validGhost;
        private Color invalidGhost;



        public Vector3 GetStandPoint()
        {
            return upgraded ? _exhibit02.standPoint.position : _exhibit01.standPoint.position;
        }

        public Exhibit GetExhibit()
        {
            return upgraded ? _exhibit02 : _exhibit01;
        }

        /// <summary>
        /// "Factory" function, to fascilitate with the creation of this specific class.
        /// Since the function is inside the Artifact class, we can set private data without having to
        /// rely on setters.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="data"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Artifact Create( GameObject owner, ArtifactData data )
        {
            var artifact = owner.AddComponent<Artifact>();
            artifact.data = data;
            artifact.status = Status.Storage;
            artifact.condition = Random.Range(0.6f, 0.9f);

            // Generate the exhibit cases/tables
            artifact._exhibit01 = Exhibit.Create(owner, data.exhibitPrefab01, data);
            artifact._exhibit01.gameObject.SetActive(false);

            artifact._exhibit02 = Exhibit.Create(owner, data.exhibitPrefab02, data);
            artifact._exhibit02.gameObject.SetActive(false);


            return artifact;
        }

        private void Awake()
        {
            validGhost = new Color(0, 1, 0, 0.5f);
            invalidGhost = new Color(1, 0, 0, 0.5f);

        }


        public void SetStatus( Status newStatus )
        {
            status = newStatus;
            Refresh();
        }


        public void Damage(float amount)
        {
            condition = Mathf.Clamp(condition - amount, 0.0f, 1.0f);
            // TODO: If zero or close to zero do something!
        }

        public void Refresh(bool valid = true)
        {

            // Step One: Activate / Deactivate the exhibit objects
            switch (status)
            {
                case Status.Storage:
                    _exhibit01.gameObject.SetActive(false);
                    _exhibit02.gameObject.SetActive(false);
                    break;

                case Status.Design:
                case Status.Transit:
                case Status.Exhibit:
                case Status.Restoration:
                    _exhibit01.gameObject.SetActive(!upgraded);
                    _exhibit02.gameObject.SetActive(upgraded);
                    break;
            }

            // Step Two: Set transparency based on state
            switch (status)
            {
                case Status.Design:
                    _exhibit01.SetGhost(true, valid? validGhost : invalidGhost );
                    _exhibit02.SetGhost(true, valid ? validGhost : invalidGhost);
                    break;
                case Status.Transit:
                    _exhibit01.SetGhost(true, validGhost);
                    _exhibit02.SetGhost(true, validGhost);
                    break;

                default:
                    _exhibit01.SetGhost(false, validGhost);
                    _exhibit02.SetGhost(false, validGhost);
                    break;
            }

        }


        public void FinalizeUpgrade()
        {
            SetStatus(Status.Exhibit);
            Refresh();
        }


        /// <summary>
        /// Upgrades the exhibit
        /// </summary>
        public void Upgrade()
        {
            if (upgraded)
                return;

            upgraded = true;
            SetStatus(Status.Transit);

            // Rebake the navmesh (just in case)
            AstarPath.active.Scan();

            // Send keeper to upgrade the exhibit display case
            KeeperManager.Instance.AddUpgradeTask(this);


            Refresh(true);
        }

        private void Update()
        {
            
        }


    }
}
