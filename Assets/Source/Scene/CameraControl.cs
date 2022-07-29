using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    [RequireComponent(typeof(TopdownCamera))]
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        private TopdownCamera m_camera;

        private void Awake()
        {
            m_camera = GetComponent<TopdownCamera>();
        }

        private void Update()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var z = Input.GetAxisRaw("Vertical");

            if (x == 0 && z == 0) {
                return;
            }

            var distance = speed * Time.deltaTime;
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
    }
}