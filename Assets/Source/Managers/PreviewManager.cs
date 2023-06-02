using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Scene;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Allows players to enter / exit preview mode for specific artifacts.
    /// </summary>
    public class PreviewManager : Singleton<PreviewManager>
    {
        [SerializeField] private TopdownCamera topdownCamera;
        [SerializeField] private CameraControl gameCamera;
        [SerializeField] private PreviewCameraControl previewCamera;


        public Exhibit m_exhibit;



        public static bool IsActive() => Instance.previewCamera.enabled;

        private void OnValidate()
        {
            topdownCamera = FindObjectOfType<TopdownCamera>(true);
            gameCamera = topdownCamera.GetComponent<CameraControl>();
            previewCamera = topdownCamera.GetComponent<PreviewCameraControl>();
        }
        
        public static void Preview( Vector3 point, Exhibit exhibit )
        {
            Instance.topdownCamera.Target = point;
            
            // Note: The enable order matters a little:
            //       The gameCamera stores the current distance from the target so that it can restore it later
            Instance.gameCamera.enabled = false;
            Instance.previewCamera.enabled = true;

            Instance.m_exhibit = exhibit;

            SelectManager.Clear();
        }
        
        public Vector3 Center => topdownCamera.Target;
        
        public static void Cancel()
        {
            Instance.previewCamera.enabled = false;
            Instance.gameCamera.enabled = true;
        }
    }
}
