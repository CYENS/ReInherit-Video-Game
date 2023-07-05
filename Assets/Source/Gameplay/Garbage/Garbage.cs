using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Managers;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Garbage : MonoBehaviour
    {
        public Vector3 position;
        public float timeInScene;

        private void Awake()
        {
            timeInScene = 0;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            timeInScene += Time.deltaTime;
        }

        public void GarbageSelected()
        {
            position = transform.position;
            JanitorManager.Instance.AddCleanTask( this );
        }

        public void CleanAndDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}
