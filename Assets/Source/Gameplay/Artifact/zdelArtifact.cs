using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using Pathfinding;
using Pathfinding.RVO;
using Random = UnityEngine.Random;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents a single artifact.
    /// </summary>
    public class zdelArtifact : MonoBehaviour
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

        private zdelExhibit _exhibit01;
        private zdelExhibit _exhibit02;


        private Color validGhost;
        private Color invalidGhost;



        public Vector3 GetStandPoint(Vector3 currentPos)
        {
            Transform pointsParent =  upgraded ? _exhibit02.standPointsParent : _exhibit01.standPointsParent;
            pointsParent.transform.localPosition = new Vector3(0f, 0f, 0f);
            Vector3 closerStandPoint = pointsParent.GetChild(0).transform.position;
            float dis = 100000f;
            foreach (Transform standPos in pointsParent) {
                float temp = Vector3.Distance(currentPos, standPos.position);
                if (temp < dis) {
                    dis = temp;
                    closerStandPoint = standPos.position;
                }
            }
            return closerStandPoint;
        }

        public zdelExhibit GetExhibit()
        {
            return upgraded ? _exhibit02 : _exhibit01;
        }

        public ArtifactVisitorHandler GetVisitorHandler() => GetExhibit().GetComponent<ArtifactVisitorHandler>();
        

        /// <summary>
        /// "Factory" function, to fascilitate with the creation of this specific class.
        /// Since the function is inside the Artifact class, we can set private data without having to
        /// rely on setters.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="info"></param>
        /// <returns>The created artifact class</returns>
        public static zdelArtifact Create( GameObject owner, ArtifactInfo info )
        {
            var artifact = owner.AddComponent<zdelArtifact>();
            artifact.m_info = info;
            artifact.status = Status.Storage;
            artifact.condition = Random.Range(0.6f, 0.9f);            


            // Generate the exhibit cases/tables
            artifact._exhibit01 = zdelExhibit.Create(owner, info.exhibitPrefab01, info);
            artifact._exhibit01.gameObject.SetActive(false);

            artifact._exhibit02 = zdelExhibit.Create(owner, info.exhibitPrefab02, info);
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
        public static zdelArtifact Create( GameObject owner, ArtifactData saveData )
        {
            // Load position and rotation
            owner.transform.position = saveData.position;
            owner.transform.rotation = Quaternion.Euler(0, saveData.angle,0);

            // Load artifact info data
            var artifact = owner.AddComponent<zdelArtifact>();
            var infoId = saveData.infoId;
            var info = ArtifactInfoLibrary.Find(infoId);
            artifact.m_info = info;

            // Load other game data
            artifact.status = (Status)saveData.status;
            artifact.condition = saveData.condition;
            
            // Generate the exhibit cases/tables
            artifact._exhibit01 = zdelExhibit.Create(owner, info.exhibitPrefab01, info);
            artifact._exhibit01.gameObject.SetActive(false);

            artifact._exhibit02 = zdelExhibit.Create(owner, info.exhibitPrefab02, info);
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
        /// Returns a value that represents the impression this exhibit gives to visitors.
        /// 0 means no interest.
        /// 0.5 means it is worth looking at
        /// 1.0 means that it is a star attraction piece
        /// </summary>
        /// <returns></returns>
        public float GetAttraction()
        {
            // TODO: Get inherent coolness value from the artifact data
            float attraction = 1.0f;

            // The condition of the artifact will negatively affect it
            if (condition > 0.75f) attraction *= 1.0f;
            else if (condition > 0.5f) attraction *= 0.9f;
            else if (condition > 0.25f) attraction *= 0.7f;
            else if (condition > float.Epsilon) attraction *= 0.5f;
            else attraction *= 0.3f;


            // If the artifact is currently on display it will be more interesting for obvious reasons
            switch (status)
            {
                case zdelArtifact.Status.Exhibit:
                    attraction *= 1.0f;
                    break;
                case zdelArtifact.Status.Restoration:
                    attraction *= 0.9f;
                    break;
                default:
                    Debug.LogError("This shouldn't be called!");
                    attraction *= 0.0f;
                    break;
            }

            // Factor novelty into the equation
            attraction *= Mathf.Lerp(0.75f, 1.0f, Novelty);


            return attraction;
        }


        /// <summary>
        /// Damages the artifact by a small amount
        /// </summary>
        public void Damage(float amount)
        {
            zdelExhibit exhibit = GetExhibit();
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
                    _exhibit01.SetGhost(true, valid ? validGhost : invalidGhost);
                    _exhibit02.SetGhost(true, valid ? validGhost : invalidGhost);
                    _exhibit01.navCollider.gameObject.SetActive(false);
                    _exhibit02.navCollider.gameObject.SetActive(false);
                    break;
                case Status.Transit:
                    _exhibit01.SetGhost(true, validGhost);
                    _exhibit02.SetGhost(true, validGhost);
                    // Cut navmesh area around exhibit
                    break;
                default:
                    _exhibit01.SetGhost(false, validGhost);
                    _exhibit02.SetGhost(false, validGhost);
                    StartCoroutine(SetNavmeshAreaRadius(2.0f, 3.2f, 3f));
                    break;
            }

            // Step Three: Generate view points for exhibit
            if (status == Status.Exhibit)
            {
                var handler = GetVisitorHandler();
                handler.GenerateViewPoints();
            }

        }

        private IEnumerator SetNavmeshAreaRadius(float newRadiusNav, float newRadiusRVO, float waitTime)
        {
            // Wait some time before expand cut to allow keeper to leave
            yield return new WaitForSeconds(waitTime);
            
            NavmeshCut navCut = GetExhibit().GetComponent<NavmeshCut>();
            float lastRadiusNav = navCut.circleRadius;
            navCut.circleRadius = newRadiusNav;
            if (Mathf.Approximately(newRadiusNav, lastRadiusNav) == false)
                navCut.ForceUpdate();
            
            RVOCircleObstacle rvoObstacle = GetExhibit().GetComponent<RVOCircleObstacle>();
            rvoObstacle.size.x = newRadiusRVO;
            rvoObstacle.size.y = newRadiusRVO;
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
