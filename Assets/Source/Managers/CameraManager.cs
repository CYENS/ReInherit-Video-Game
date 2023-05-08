using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class CameraManager : CollectionManager<CameraManager, Camera>
    {


        protected Camera[] m_cameras;


        protected new void Start()
        {
            base.Start();
            m_cameras = m_collection.Get<Camera>();
        }

        public static void SetCamera( int index )
        {
            index = Mathf.Clamp(index, 0, Instance.m_cameras.Length - 1);
            Debug.Log("Set Camera " + index);

            for ( int i=0; i<Instance.m_cameras.Length; i++ )
            {
                Camera camera = Instance.m_cameras[i];
                camera.gameObject.SetActive(i == index);
            }
            
        }

    }
}
