using Cyens.ReInherit.Managers;
using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public class PreviewCamera : MonoBehaviour
    {

        private PreviewManager m_previewManager;

        [Header("Parameters")]

        [SerializeField]
        private float m_speed = 100.0f;

        [Header("Variables")]

        [SerializeField]
        private float m_anglex;

        [SerializeField]
        private float m_angley;

        [SerializeField]
        [Range(1.0f,4.0f)]
        private float m_zoom;


        [SerializeField]
        private Vector3 m_lastPos;


        // Start is called before the first frame update
        void Start()
        {
            m_previewManager = PreviewManager.Instance;
        }

        // Update is called once per frame
        void Update()
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

                m_anglex += mouseDelta.x * Time.deltaTime * m_speed;
                m_angley += mouseDelta.y * Time.deltaTime * m_speed;
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            if( Mathf.Abs(scrollDelta) > float.Epsilon )
            {
                m_zoom += scrollDelta * Time.deltaTime * 10.0f;
                m_zoom = Mathf.Clamp(m_zoom, 1.0f, 4.0f);
            }



            Vector3 center = m_previewManager.Center;

            Vector3 offset = Vector3.forward * m_zoom;
            offset = Quaternion.Euler(Vector3.right * m_angley) * offset;
            offset = Quaternion.Euler(Vector3.up * m_anglex) * offset;

            Vector3 pos = center + offset;
            transform.position = pos;


            transform.rotation = Quaternion.LookRotation(center - pos);
            
        }
    }
}
