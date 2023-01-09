using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Cyens.ReInherit.Patterns;
using Cyens.ReInherit;

namespace Cyens.ReInherit
{
    public class PlacementManager : Singleton<PlacementManager>
    {

        public bool active = false;

        protected ReInheritControls input;

        protected float clickBuffer;


        protected Vector2 mousePos;
        protected new Camera camera;


        protected Plane groundPlane;


        public GameObject caller;

        [Header("Grid")]
        public Vector2 cellSize;


        protected Color green;
        protected Color red;
        protected Color invisible;

        [Header("Collision Detection")]
        public LayerMask floorMask;
        public LayerMask obstacleMask;

        public bool validPlacement;


        [Header("Results")]
        public GameObject prefab;

        [Header("References")]

        public Transform ghost;
        private MeshFilter ghostFilter;
        private MeshFilter ghostShadow;
        private Material ghostMaterial;
        private Color ghostColor;
        



        // Start is called before the first frame update
        void Start()
        {
            if( input == null ) input = new ReInheritControls();

            camera = Camera.main;

            
            ghostFilter = ghost.GetComponent<MeshFilter>();
            ghostMaterial = ghost.GetComponent<MeshRenderer>().sharedMaterial;

            // Transparent objects don't cast shadows. So use shadow caster object instead!
            ghostShadow = ghost.GetComponentsInChildren<MeshFilter>()[1];

            groundPlane = new Plane(Vector3.up, Vector3.zero);

            // Setup colors
            green = new Color(0, 1, 0, 0.5f);
            red = new Color(1, 0, 0, 0.5f);
            invisible = new Color(0, 0, 0, 0);

        }

        private void OnEnable()
        {
            if (input == null) input = new ReInheritControls();
            input.Enable();
            input.GamePlay.Select.performed += OnPlace;
            input.GamePlay.Point.performed += OnMove;
            input.GamePlay.Cancel.performed += OnCancel;
            //input.GamePlay.Target.performed += OnTarget;
        }

        private void OnDisable()
        {
            input.Disable();
            input.GamePlay.Select.performed -= OnPlace;
            input.GamePlay.Point.performed -= OnMove;
            input.GamePlay.Cancel.performed -= OnCancel;
        }

        public void Begin( GameObject caller, Mesh mesh, Vector3 scale, GameObject prefab, Vector2 cellSize )
        {
            if (active) return;

            this.caller = caller;

            // To prevent "instant double click" type glitches
            clickBuffer = 0.15f;
            validPlacement = false;
            active = true;

            this.prefab = prefab;
            this.cellSize = cellSize;

            this.ghostFilter.sharedMesh = mesh;
            this.ghostShadow.sharedMesh = mesh;
            ghost.localScale = scale;
        }

        protected void OnMove(InputAction.CallbackContext context)
        {
            mousePos = context.ReadValue<Vector2>();
        }

        protected void OnCancel(InputAction.CallbackContext context)
        {
            if (active == false) return;
            if (clickBuffer > float.Epsilon) return;

            active = false;

            if (caller != null)
            {
                caller.SendMessage("Cancel","cancelled");
                caller = null;
            }
        }

        protected void OnPlace(InputAction.CallbackContext context)
        {
            if (active == false) return;
            if (clickBuffer > float.Epsilon) return;

            if( validPlacement == false )
            {
                // TODO: Error sound
                return;
            }

            // Generate object at position
            GameObject instance = GameObject.Instantiate(prefab, ghost.position, Quaternion.identity);
            instance.transform.localScale = ghost.localScale;

            // deactivate placer
            active = false;
        }

        protected Vector3 Snap(Vector3 position)
        {
            Vector3 snapped = position;
            snapped.x = Mathf.Round(position.x / cellSize.x) * cellSize.x;
            snapped.z = Mathf.Round(position.z / cellSize.y) * cellSize.y;
            return snapped;
        }


        protected bool IsGrounded(Vector3 position)
        {
            // If even one ray is OOB then consider spot invalid
            Vector3 origin = position + Vector3.up;
            Ray dropRay = new Ray(position + Vector3.up, Vector3.down);
            if (Physics.Raycast(origin, Vector3.down, 2.0f, obstacleMask) == false) return false;
            if (Physics.Raycast(origin + Vector3.left * cellSize.x * 0.5f , Vector3.down, 2.0f, obstacleMask) == false) return false;
            if (Physics.Raycast(origin + Vector3.right * cellSize.x * 0.5f, Vector3.down, 2.0f, obstacleMask) == false) return false;
            if (Physics.Raycast(origin + Vector3.forward * cellSize.y * 0.5f, Vector3.down, 2.0f, obstacleMask) == false) return false;
            if (Physics.Raycast(origin + Vector3.back * cellSize.y * 0.5f, Vector3.down, 2.0f, obstacleMask) == false) return false;

            return true;
        }

        // Update is called once per frame
        void Update()
        {
            if( active == false )
            {
                ghostMaterial.color = invisible;
                ghostShadow.gameObject.SetActive(false);
                validPlacement = false;
                return;
            }

            clickBuffer = Mathf.Max(clickBuffer - Time.deltaTime, 0.0f);


            // Find cursor position in world space
            Vector3 screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane) );
            Vector3 cameraPos = camera.transform.position;
            //Debug.Log(screenPos);
            Ray screenRay = new Ray(camera.transform.position, screenPos - cameraPos);

            float t = 0.0f;
            if( groundPlane.Raycast(screenRay, out t) )
            {
                Vector3 intersection = screenRay.GetPoint(t);
                ghost.position = intersection;

                // Snap to position
                if (cellSize.magnitude > float.Epsilon) 
                    ghost.position = Snap(ghost.position);

                // Check to see if placement is valid
                validPlacement = true;
                if (IsGrounded(ghost.position) == false) validPlacement = false;
            }


            // Change color
            ghostShadow.gameObject.SetActive(true);
            ghostMaterial.color = validPlacement ? green : red;
        }
    }
}
