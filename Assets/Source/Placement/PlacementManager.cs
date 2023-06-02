using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit.Managers
{   
    public class PlacementManager : Singleton<PlacementManager>
    {

        public enum Status {
            Inactive = 0,
            Exhibit = 1
        }

        [SerializeField]
        protected Status m_status;


        [Header("References")]

        [SerializeField]
        [Tooltip("References the placement marker object")]
        protected Transform m_marker;

        [SerializeField]
        protected Material m_modelMat;

        [SerializeField]
        [Tooltip("A reference to the mesh filter, so that we can switch the model at will")]
        protected MeshFilter m_meshFilter;


        [SerializeField]
        protected Material m_markerMat;


        [Header("Grid Settings")]

        [SerializeField]
        protected float m_gridSize = 5f;


        [Header("Ray Cast Settings")]
        [SerializeField] 
        [Tooltip("Layers that the raycaster must be able to detect")]
        protected LayerMask m_layers;

        [SerializeField]
        [Tooltip("Layers that the raycaster should consider obstacles")]
        protected LayerMask m_obstacles;


        [Header("Color settings")]

        [SerializeField]
        protected Color m_validCol;

        [SerializeField]
        protected Color m_invalidCol;

        protected string m_colorName = "_BaseColor";


        // The ground plane
        protected Plane m_ground;

        
        protected GameObject m_notifyTarget;


        // Start is called before the first frame update
        void Start()
        {
            // Ground plane to help with ray intersection calculations
            m_ground = new Plane(Vector3.up, Vector3.zero);
        }


        public static bool IsActive() => Instance.m_status != Status.Inactive;


        public static void PlaceExhibit( GameObject notifyTarget, Mesh mesh ) 
        {
            Instance.m_notifyTarget = notifyTarget;
            Instance.m_status = Status.Exhibit;
            Instance.m_meshFilter.sharedMesh = mesh;

            // Deselect everything while placing
            SelectManager.Clear();
        }

        public static void Cancel() 
        {
            Instance.m_status = Status.Inactive;
        }


        /// <summary>
        /// Utility function that gives us the the position which the mouse cursor points to
        /// on the infinite ground plane.
        /// </summary>
        /// <param name="intersection"></param>
        /// <returns></returns>
        protected bool GetIntersection( ref Vector3 intersection )
        {
            // Get Mouse cursor position on the screen
            Vector2 mousePos = Input.mousePosition;

            // Get camera
            Camera camera = Camera.main;

            // Find cursor position in world space
            Vector3 screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));
            Vector3 cameraPos = camera.transform.position;

            // Find intersection of screen ray and the ground plane
            Ray screenRay = new Ray(camera.transform.position, screenPos - cameraPos);
            float t = 0.0f;
            if (m_ground.Raycast(screenRay, out t))
            {
                intersection = screenRay.GetPoint(t);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Snaps the point to a grid
        /// </summary>
        /// <param name="point"></param>
        protected void SnapToGrid( ref Vector3 point )
        {
            Vector3 snapped = point;
            float half = m_gridSize/2.0f;
            snapped.x = Mathf.Round((point.x-half)/m_gridSize)*m_gridSize + half;
            snapped.z = Mathf.Round((point.z-half)/m_gridSize)*m_gridSize + half;
            point = snapped;
        }


        /// <summary>
        /// Checks to see if the layer of the given object is considered an obstacle
        /// to avoid. Performs a simple bitwise and against an obstacle layer mask.
        /// </summary>
        /// <param name="target">The target game object</param>
        /// <returns></returns>
        protected bool IsObstacle( GameObject target )
        {

            // Bitwise and
            int mask = 1 << target.layer;
            return ( mask & m_obstacles ) != 0;
        }

        /// <summary>
        /// Checks the availability of the current point.
        /// It ensures that there is solid ground to place the object, and 
        /// it also ensures that there is no obstacle in the way.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected bool CheckAvailability(Vector3 point)
        {
            
            if( GameUI.Instance.isPointerOverUIElement )
            {
                return false;
            }

            // A ray that drops down
            Ray dropRay = new Ray(point + Vector3.up * 100.0f, Vector3.down);
            RaycastHit hit;

            // TODO: Check for obstacles, or if there is floor
            if (Physics.Raycast(dropRay, out hit, 1000f, m_layers))
            {
                if( IsObstacle(hit.collider.gameObject) )
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        protected void UpdatePlacement()
        {
            bool valid = false;

            // Cancel placement with right click
            if( Input.GetMouseButtonDown(1) )
            {
                Cancel();
                return;
            }

            Vector3 markerPos = m_marker.position;
            if( GetIntersection(ref markerPos) )
            {
                // Snap
                SnapToGrid(ref markerPos);

                m_marker.position = markerPos;

                // Check if there's solid ground below
                if( CheckAvailability( markerPos ) )
                {
                    valid = true;
                }
            }


            // Change color based on placement validity.
            m_markerMat.SetColor( m_colorName, valid ? m_validCol : m_invalidCol );
            m_modelMat.SetColor(  m_colorName, valid ? m_validCol : m_invalidCol );


            // On Click
            if( Input.GetMouseButtonDown(0) && valid )
            {
                if(m_notifyTarget != null )
                {
                    m_notifyTarget.SendMessage("Place", m_marker.position );
                    m_status = Status.Inactive;
                }
            }
        }

        

        // Update is called once per frame
        void Update()
        {
            
            switch(m_status)
            {
                case Status.Exhibit:
                    UpdatePlacement();
                break;
            }
            

            m_marker.gameObject.SetActive( m_status != Status.Inactive );
        }
    }
}
