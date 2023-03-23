using System;
using System.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Gameplay.Management;

using Pathfinding;

namespace Cyens.ReInherit
{
    [DefaultExecutionOrder(-1000)]
    public class ArtifactManager : Singleton<ArtifactManager>
    {

        public List<ArtifactInfo> starterArtifacts;


        // Input related
        protected ReInheritControls input;
        private Vector2 mousePos;
        private new Camera camera;

        // Placement related
        private bool validPlacement;
        protected Plane groundPlane;
        protected Artifact targetArtifact;
        [SerializeField] LayerMask raycastIgnoreLayer;
        private colliding collidingScript;

        public enum Mode { None = 0, Placement = 1 }
        public Mode mode = Mode.None;



        private void Start()
        {
            if (input == null) input = new ReInheritControls();
            camera = Camera.main;


            // Ground plane to help with ray intersection calculations
            groundPlane = new Plane(Vector3.up, Vector3.zero);


            foreach ( var info in starterArtifacts)
                AddArtifact(info);
            
        }

        private void OnEnable()
        {
            if (input == null) input = new ReInheritControls();
            input.Enable();
            input.GamePlay.Select.performed += OnSelect;
            input.GamePlay.Point.performed += OnMove;
            input.GamePlay.Cancel.performed += OnCancel;
        }

        private void OnDisable()
        {
            input.Disable();
            input.GamePlay.Select.performed -= OnSelect;
            input.GamePlay.Point.performed -= OnMove;
            input.GamePlay.Cancel.performed -= OnCancel;
        }


        /// <summary>
        /// Returns an array of exhibits that are currently on display
        /// </summary>
        public Exhibit[] GetExhibits()
        {
            Exhibit[] exhibits = GetComponentsInChildren<Exhibit>(false);
            return exhibits;
        }


        public Artifact[] GetArtifactsByStatus( Artifact.Status status )
        {
            Artifact[] allArtifacts = GetComponentsInChildren<Artifact>(true);
            return Array.FindAll<Artifact>(allArtifacts, artifact => artifact.GetStatus() == status);
        }

        public ArtifactInfo[] GetArtifactInfo()
        {
            // Grab the artifacts contained within and get their data
            // Collect their data in hashsets to remove duplicates
            Artifact[] artifacts = GetComponentsInChildren<Artifact>(true);
            HashSet<ArtifactInfo> dataSet = new HashSet<ArtifactInfo>();
            foreach( var artifact in artifacts )
                dataSet.Add(artifact.GetInfo());
            
            // Convert the hashset to an array
            ArtifactInfo[] artifactInfo = new ArtifactInfo[dataSet.Count];
            dataSet.CopyTo(artifactInfo);
            return artifactInfo;
        }

        public void PlaceArtifact(Artifact artifact)
        {
            mode = Mode.Placement;
            artifact.SetStatus(Artifact.Status.Design);
            targetArtifact = artifact;

        }

        #region InputEventFunctions
        protected void OnSelect(InputAction.CallbackContext context)
        {
            switch (mode)
            {
                case Mode.Placement:
                    FinalizePlacement();
                    break;
            }


        }

        protected void OnMove(InputAction.CallbackContext context) => mousePos = context.ReadValue<Vector2>();

        protected void OnCancel(InputAction.CallbackContext context)
        {
        }
        #endregion


        public void AddArtifact(ArtifactInfo info)
        {

            // Step One: Create an empty child game object
            GameObject temp = new GameObject("Artifact");
            temp.transform.SetParent(transform);

            // Step Two: Add the 'Artifact' class to that object
            Artifact artifact = Artifact.Create(temp, info );
            temp.name = artifact.GetInfo().label;
        }


        protected Vector3 Snap(Vector3 position)
        {
            Vector3 snapped = position;
            snapped.x = Mathf.Round(position.x) + 0.5f;
            snapped.z = Mathf.Round(position.z) + 0.5f;
            return snapped;
        }


        /// <summary>
        /// Updates the novelty of each artifact based on how long it was 
        /// on display. Should be called at the end of a round
        /// </summary>
        public void UpdateNovelty()
        {

            // Get All artifact data
            var artifactInfo = GetArtifactInfo();

            // Get artifacts that are on display, and their data
            var displayedArtifacts = GetArtifactsByStatus(Artifact.Status.Exhibit);
            HashSet<ArtifactInfo> displayedArtifactData = new HashSet<ArtifactInfo>();
            foreach( var artifact in displayedArtifacts )
                displayedArtifactData.Add(artifact.GetInfo());

            // Get artifacts that are in the Conservation area
            var restorationArtifacts = GetArtifactsByStatus(Artifact.Status.Restoration);
            HashSet<ArtifactInfo> restorationArtifactData = new HashSet<ArtifactInfo>();
            foreach (var artifact in restorationArtifacts)
                restorationArtifactData.Add(artifact.GetInfo());

            // Add / subtract points accordingly
            foreach( var data in artifactInfo )
            {
                if (displayedArtifactData.Contains(data)) data.Novelty -= 0.1f;
                else if (restorationArtifactData.Contains(data)) data.Novelty += 0.0f;
                else data.Novelty += 0.5f;
            }

        }

        /// <summary>
        /// Applies damage to each artifact on display.
        /// Should be called at the end of a round.
        /// </summary>
        public void ApplyDamage()
        {
            var artifacts = GetArtifactsByStatus(Artifact.Status.Exhibit);
            foreach( var artifact in artifacts )
                artifact.Damage(0.1f);
        }

        /// <summary>
        /// Fixes all the artifacts in the restoration room.
        /// Then returns them to their exhibits after they're fixed
        /// </summary>
        public void FixArtifacts()
        {
            var artifacts = GetArtifactsByStatus(Artifact.Status.Restoration);
            foreach( var artifact in artifacts )
            {
                artifact.Restore(0.3f);
                if (artifact.condition >= 1.0f - float.Epsilon)
                {
                    artifact.SetStatus(Artifact.Status.Exhibit);
                    Debug.Log("" + gameObject + " artifact is fully restored!");
                }
            }
        }

        private void UpdatePlacement()
        {
            validPlacement = false;
            // Find cursor position in world space
            Vector3 screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));
            Vector3 cameraPos = camera.transform.position;

            // Find intersection of screen ray and the ground plane
            Ray screenRay = new Ray(camera.transform.position, screenPos - cameraPos);
            RaycastHit hit;
            float t = 0.0f;
            if (groundPlane.Raycast(screenRay, out t))
            {
                Vector3 intersection = screenRay.GetPoint(t);
                targetArtifact.transform.position = intersection;
                
                // Snap to center of a 1x1 cell


                //// Snap to grid
                targetArtifact.transform.position = Snap(intersection);


                Ray dropRay = new Ray(targetArtifact.transform.position + Vector3.up * 100.0f, Vector3.down);

                // TODO: Check for obstacles, or if there is floor
                if (Physics.Raycast(dropRay, out hit, 1000f, ~raycastIgnoreLayer))
                {
                    if (hit.transform.tag == "Floor" && targetArtifact.transform.GetComponentInChildren<colliding>().isColliding == false)
                    {
                        
                        validPlacement = true;
                    }
                       
                    else
                        validPlacement = false;
                }
                Debug.Log(targetArtifact.transform.GetComponentInChildren<colliding>().isColliding);
                // Check to see if placement is valid
                //validPlacement = true;
                //if (IsGrounded(ghost.position) == false) validPlacement = false;
            }
            //Debug.Log(targetArtifact.transform.GetChild(0).ToString());
            targetArtifact.Refresh(validPlacement);

        }

        private void FinalizePlacement()
        {
            if (validPlacement == false) return;

            mode = Mode.None;
            targetArtifact.SetStatus(Artifact.Status.Transit);

            // Rebake the navmesh
            //AstarPath.active.Scan();

            KeeperManager.Instance.AddPlaceTask(targetArtifact);
        }


        private void Update()
        {
            
            switch( mode )
            {
                case Mode.Placement:
                    UpdatePlacement();
                    break;
            }

        }

    }
}
