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
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            m_camera.Target += new Vector3(x, 0, y) * (speed * Time.deltaTime);
        }
    }
}