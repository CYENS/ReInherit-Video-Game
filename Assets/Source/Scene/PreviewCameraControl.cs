using System.Net.NetworkInformation;
using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public class PreviewCameraControl : MonoBehaviour
    {
        [SerializeField] private float speed = 100;
        [SerializeField] private float scrollSpeed = 100;

        
        private TopdownCamera m_camera;
        private float m_anglex;
        private float m_angley;
        private float m_zoom;
        private Vector3 m_lastPos;

        private bool m_isMouseDown;

        private void Awake()
        {
            m_camera = GetComponent<TopdownCamera>();
        }

        private void OnEnable()
        {
            m_camera.MinDistance = 2;
            m_camera.MaxDistance = 10;
            m_camera.Distance = 5;
            m_camera.AutoWarpOrbit = false;
        }

        private void Update()
        {
            // Inputs
            if( Input.GetMouseButtonDown(1) )
            {
                m_lastPos = Input.mousePosition;
            }
            if( Input.GetMouseButton(1) )
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 mouseDelta = m_lastPos - mousePos;
                m_lastPos = mousePos;

                m_camera.OrbitAngle += mouseDelta.x * speed * Time.deltaTime;
                m_camera.Angle += mouseDelta.y * speed * Time.deltaTime;
            } else if (Input.GetMouseButtonUp(1)) {
                m_camera.SanitizeOrbitAngle();
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            if( Mathf.Abs(scrollDelta) > float.Epsilon )
            {
                m_camera.Distance += scrollDelta * Time.deltaTime * scrollSpeed;
            }
        }
    }
}
