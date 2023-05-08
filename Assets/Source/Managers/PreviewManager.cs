using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Allows players to enter / exit preview mode for specific artifacts.
    /// </summary>
    public class PreviewManager : Singleton<PreviewManager>
    {

        [Header("Parameters")]

        [SerializeField]
        private int m_cameraIndex = 1;

        [Header("Variables")]
        [SerializeField]
        private bool m_active;

        public bool Active => m_active;


        [SerializeField]
        private Vector3 m_center;

        public Vector3 Center => m_center;




        private void SetActive(bool active)
        {
            Instance.m_active = active;
        }

        private void SetCenter(Vector3 point)
        {
            m_center = point;
        }


        public static void Preview( Vector3 point )
        {
            Instance.SetActive(true);
            Instance.SetCenter(point);
            CameraManager.SetCamera(Instance.m_cameraIndex);
        }

        public static void Cancel()
        {
            Instance.SetActive(false);
        }



        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
