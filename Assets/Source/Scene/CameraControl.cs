using Cyens.ReInherit.Extensions;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Cyens.ReInherit.Scene
{
    [RequireComponent(typeof(TopdownCamera))]
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float zoomSpeed = 30f;
        [SerializeField] private float rotateSpeed = 180f;
        [SerializeField] private float mousePanSpeed = 0.05f;

        private TopdownCamera m_camera;
        private Vector3 m_lastPanPoint;
        private Vector3 m_lastPlaneHit;

        private void Awake()
        {
            m_camera = GetComponent<TopdownCamera>();
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
            m_camera.Target += direction * distance;
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
                m_camera.PlaneCast(out m_lastPlaneHit);
            } else if (Input.GetMouseButton(2)) {
                if (m_camera.PlaneCast(out var pointNow)) {

                }
                // var mouseDelta = m_lastPanPoint - Input.mousePosition;
                // m_lastPanPoint = Input.mousePosition;
                //
                // var cameraTransform = m_camera.transform;
                // var forward2d = cameraTransform.forward.xz();
                // var side2d = cameraTransform.right.xz();
                //
                // var delta2d = (mouseDelta.x * side2d + mouseDelta.y * forward2d).normalized;
                //
                // m_camera.Target += delta2d.x0z() * mousePanSpeed;
            }

            // TODO: Add controls for mouse panning and rotating

            if (x == 0 && z == 0) {
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                Zoom(z);
                Pan(x, 0);
            } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                Rotate(x, z);
            } else {
                Pan(x, z);
            }
        }
    }
}