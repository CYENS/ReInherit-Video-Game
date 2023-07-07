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
        [SerializeField] private Renderer m_garbageRenderer;
        
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

        public void Clean()
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        
        IEnumerator FadeOutAndDestroy()
        {
            float fadeOutTime = 2.0f;
            if (m_garbageRenderer == null) yield break;
            
            Material material = m_garbageRenderer.material;
            float fadeRate = 1.0f / fadeOutTime;
            for (float t = 0.0f; t <= 1.0f; t += fadeRate * Time.deltaTime)
            {
                float alpha = 1.0f - t;
                material.SetFloat("_Opacity", alpha);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
