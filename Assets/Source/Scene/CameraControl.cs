using Cyens.ReInherit.Architect;
using Cyens.ReInherit.Extensions;
using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    [RequireComponent(typeof(TopdownCamera))]
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float zoomSpeed = 30f;
        [SerializeField] private float rotateSpeed = 180f;
        [SerializeField] private float minDistance = 30;
        [SerializeField] private float maxDistance = 180;
        [SerializeField] private float xPadding = 0;
        [SerializeField] private float zPadding = 0;

        private TopdownCamera m_camera;
        private Vector3 m_lastPanPoint;
        private Vector3 m_lastPlaneHit;

        private float m_storedDistance = -1;
        private RoomGraph m_graph;

        private void OnDisable()
        {
            m_storedDistance = m_camera.Distance;
        }

        private void OnEnable()
        {
            m_camera.MinDistance = minDistance;
            m_camera.MaxDistance = maxDistance;
            m_camera.AutoWarpOrbit = true;

            if (m_storedDistance > 0) {
                m_camera.Distance = m_storedDistance;
            }
        }

        private void Awake()
        {
            m_camera = GetComponent<TopdownCamera>();
            m_graph = FindObjectOfType<RoomGraph>();
        }

        private void Pan(float x, float z)
        {
            var distance = panSpeed * Time.deltaTime;
            var camTransform = m_camera.transform;

            // Convert forward and right to be parallel to the ground
            var forward = camTransform.forward;
            forward.y = 0;
            forward.Normalize();

            var right = camTransform.right;
            right.y = 0;
            right.Normalize();

            var direction = (x * right + z * forward).normalized;
            SetTargetClamped(m_camera.Target + direction * distance);
        }

        public void SetTargetClamped(Vector3 target)
        {
            var min = new Vector2(m_graph.MinWorldX, m_graph.MinWorldY);
            var max = new Vector2(m_graph.MaxWorldX, m_graph.MaxWorldX);
            target.x = Mathf.Clamp(target.x, min.x - xPadding, max.x + xPadding);
            target.z = Mathf.Clamp(target.z, min.y - zPadding, max.y + zPadding);
            m_camera.Target = target;
        }

        private void Zoom(float z)
        {
            m_camera.Distance -= z * zoomSpeed * Time.deltaTime;
        }

        private void Rotate(float x, float z)
        {
            m_camera.Angle += z * rotateSpeed * Time.deltaTime;
            m_camera.OrbitAngle -= x * rotateSpeed * Time.deltaTime;
        }

        private void Update()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var z = Input.GetAxisRaw("Vertical");

            if (Input.mouseScrollDelta.y != 0) {
                Zoom(Input.mouseScrollDelta.y * zoomSpeed);
            }

            if (Input.GetMouseButtonDown(2)) {
                m_lastPanPoint = Input.mousePosition;
                // m_camera.PlaneCast(out m_lastPlaneHit);
            } else if (Input.GetMouseButton(2)) {
                var cameraTransform = m_camera.transform;

                Vector3 p1;
                Vector3 p2;

                if (m_camera.GroundCast(Input.mousePosition, out p1) && m_camera.GroundCast(m_lastPanPoint, out p2)) {
                    // Natural "grab" panning mode in regards to the ground
                    SetTargetClamped(m_camera.Target + (p2 - p1));
                } else {
                    // Less natural but failsafe panning mode for when the user doesn't drag along the ground.

                    var ray1 = m_camera.Raycast(Input.mousePosition);
                    var ray2 = m_camera.Raycast(m_lastPanPoint);
                    var forward = cameraTransform.forward;

                    Picker.TryPlaneIntersect(ray1, forward, m_camera.Target, out p1);
                    Picker.TryPlaneIntersect(ray2, forward, m_camera.Target, out p2);

                    var delta = p2 - p1;

                    var forward2d = forward.xz();
                    var side2d = cameraTransform.right.xz();
                    var delta2d = (delta.x * side2d + delta.y * forward2d).normalized;

                    SetTargetClamped(m_camera.Target + delta2d.x0z());
                }

                m_lastPanPoint = Input.mousePosition;
            }

            if (x == 0 && z == 0) {
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                Zoom(z);
                Pan(x, 0);
            } else if (Input.GetMouseButton(2)) {
                Rotate(x, z);
            } else {
                Pan(x, z);
            }
        }
    }
}
