using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class ExhibitManager : Singleton<ExhibitManager>
    {

        public Exhibit[] exhibits;

        [Header("Debugging")]
        public bool dbgTest = false;
        public Artifact dbgArtifact;
        public Transform dbgPoint;


        private void OnValidate()
        {
            if(dbgTest)
            {
                dbgTest = false;
                PlaceExhibit(dbgPoint.position, dbgArtifact);
                KeeperManager.Instance.AddNewTask(dbgPoint.position);
            }
        }


        public bool PlaceExhibit(Vector3 point, Artifact artifact)
        {
            // Find available exhibit case.
            Exhibit exhibit = exhibits.FirstOrDefault(candidate => candidate.gameObject.activeSelf == false);
            if (exhibit == null) return false;


            exhibit.gameObject.SetActive(true);
            exhibit.transform.position = point;
            //exhibit.AddArtifact(artifact);

            artifact.SetStatus(Artifact.Status.Exhibit);
            return true;
        }

        private void Start()
        {
            exhibits = GetComponentsInChildren<Exhibit>(true);          
        }
    }
}
