using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    [DefaultExecutionOrder(-1000)]
    public class ArtifactManager : Singleton<ArtifactManager>
    {

        public List<ArtifactData> starterArtifacts;


        // Input related
        protected ReInheritControls input;
        private Vector2 mousePos;
        private new Camera camera;

        // Placement related
        private bool validPlacement;
        protected Plane groundPlane;
        protected Artifact targetArtifact;


        public enum Mode { None = 0, Placement = 1 }
        public Mode mode = Mode.None;



        private void Start()
        {
            if (input == null) input = new ReInheritControls();
            camera = Camera.main;


            // Ground plane to help with ray intersection calculations
            groundPlane = new Plane(Vector3.up, Vector3.zero);


            foreach ( var data in starterArtifacts)
                AddArtifact(data);
            
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


        public void AddArtifact(ArtifactData data)
        {

            // Step One: Create an empty child game object
            GameObject temp = new GameObject("Artifact");
            temp.transform.SetParent(transform);

            // Step Two: Add the 'Artifact' class to that object
            Artifact artifact = Artifact.Create(temp, data );
            temp.name = artifact.GetData().label;

        }


        protected Vector3 Snap(Vector3 position)
        {
            Vector3 snapped = position;
            snapped.x = Mathf.Round(position.x) + 0.5f;
            snapped.z = Mathf.Round(position.z) + 0.5f;
            return snapped;
        }


        private void UpdatePlacement()
        {
            // Find cursor position in world space
            Vector3 screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));
            Vector3 cameraPos = camera.transform.position;

            // Find intersection of screen ray and the ground plane
            Ray screenRay = new Ray(camera.transform.position, screenPos - cameraPos);
            float t = 0.0f;
            if (groundPlane.Raycast(screenRay, out t))
            {
                Vector3 intersection = screenRay.GetPoint(t);
                targetArtifact.transform.position = intersection;

                // Snap to center of a 1x1 cell

                //// Snap to grid
                targetArtifact.transform.position = Snap(intersection);

                // Check to see if placement is valid
                validPlacement = true;
                //if (IsGrounded(ghost.position) == false) validPlacement = false;
            }

            targetArtifact.Refresh(validPlacement);

        }

        private void FinalizePlacement()
        {
            if (validPlacement == false) return;

            mode = Mode.None;
            targetArtifact.SetStatus(Artifact.Status.Transit);

            KeeperManager.Instance.AddNewTask(targetArtifact.transform.position);
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
