using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;
using Pathfinding;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents a single artifact.
    /// </summary>
    public class Artifact : MonoBehaviour
    {
        [SerializeField]
        private ArtifactInfo m_info;

        public enum Status { Storage = 0, Design = 1, Transit = 2, Exhibit = 3, Restoration = 4  }

        [SerializeField]
        private Status status;


        public Status GetStatus() => status;

        public ArtifactInfo GetInfo() => m_info;


        public float Novelty => m_info.Novelty;        
        public string GetLabel() => m_info.label;


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
        /// <param name="info"></param>
        /// <returns>The created artifact class</returns>
        public static Artifact Create( GameObject owner, ArtifactInfo info )
        {
            var artifact = owner.AddComponent<Artifact>();
            artifact.m_info = info;
            artifact.status = Status.Storage;
            artifact.condition = Random.Range(0.6f, 0.9f);

            // Generate the exhibit cases/tables
            artifact._exhibit01 = Exhibit.Create(owner, info.exhibitPrefab01, info);
            artifact._exhibit01.gameObject.SetActive(false);

            artifact._exhibit02 = Exhibit.Create(owner, info.exhibitPrefab02, info);
            artifact._exhibit02.gameObject.SetActive(false);


            return artifact;
        }

        /// <summary>
        /// An alternate version of the artifact factory, which instead created an artifact
        /// based on some save data.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="saveData"></param>
        /// <returns>The created artifact class</returns>
        public static Artifact Create( GameObject owner, ArtifactData saveData )
        {
            // Load position and rotation
            owner.transform.position = saveData.position;
            owner.transform.rotation = Quaternion.Euler(0, saveData.angle,0);

            // Load artifact info data
            var artifact = owner.AddComponent<Artifact>();
            var infoId = saveData.infoId;
            var info = ArtifactInfoLibrary.Find(infoId);
            artifact.m_info = info;

            // Load other game data
            artifact.status = (Status)saveData.status;
            artifact.condition = saveData.condition;
            
            // Generate the exhibit cases/tables
            artifact._exhibit01 = Exhibit.Create(owner, info.exhibitPrefab01, info);
            artifact._exhibit01.gameObject.SetActive(false);

            artifact._exhibit02 = Exhibit.Create(owner, info.exhibitPrefab02, info);
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


        /// <summary>
        /// Damages the artifact by a small amount
        /// </summary>
        public void Damage(float amount)
        {
            Exhibit exhibit = GetExhibit();
            float protection = exhibit.Protection;

            // Damage is reduced when placed in a protective case
            float damage = amount;
            damage -= amount * protection;

            // Affect the 'health' of the exhibit
            condition = Mathf.Clamp(condition - damage, 0.0f, 1.0f);
            // TODO: If zero or close to zero do something!
        }


        public void Restore( float amount ) => condition = Mathf.Clamp(condition + amount, 0.0f, 1.0f);
        

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
            _exhibit01.navCollider.gameObject.SetActive(true);
            _exhibit02.navCollider.gameObject.SetActive(true);
            switch (status)
            {
                case Status.Design:
                    _exhibit01.SetGhost(true, valid? validGhost : invalidGhost );
                    _exhibit02.SetGhost(true, valid ? validGhost : invalidGhost);
                    _exhibit01.navCollider.gameObject.SetActive(false);
                    _exhibit02.navCollider.gameObject.SetActive(false);
                    break;
                case Status.Transit:
                    _exhibit01.SetGhost(true, validGhost);
                    _exhibit02.SetGhost(true, validGhost);
                    // Cut navmesh area around exhibit
                    CutNavmeshArea(true);
                    break;
                default:
                    _exhibit01.SetGhost(false, validGhost);
                    _exhibit02.SetGhost(false, validGhost);
                    break;
            }

        }

        // Enable/Disable NavMeshCut component
        private void CutNavmeshArea(bool enable)
        {
            if (enable) {
                NavmeshCut navCut = gameObject.AddComponent<NavmeshCut>();
                navCut.circleRadius = 1f;
                navCut.type = NavmeshCut.MeshType.Circle;
            }
            else {
                if(gameObject.GetComponent<NavmeshCut>() != null)
                    Destroy(GetComponent<NavmeshCut>());
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



            // Send keeper to upgrade the exhibit display case
            KeeperManager.Instance.AddUpgradeTask(this);


            Refresh(true);
        }

        

        /// <summary>
        /// Returns a serializable data object containing
        /// all the important info for this artifact
        /// </summary>
        /// <returns></returns>
        public ArtifactData GetData()
        {
            ArtifactData data = new ArtifactData();
            data.position = transform.position;
            data.angle = transform.rotation.eulerAngles.y;

            data.infoId = m_info.myid;
            data.condition = condition;
            data.upgraded = upgraded;
            switch(status)
            {
                case Status.Design: data.status = (int)Status.Storage; break;
                case Status.Transit: data.status = (int)Status.Exhibit; break;
                default: data.status = (int)status; break;
            }

            return data;
        }

    }
}
