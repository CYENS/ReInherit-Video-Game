using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Coin : MonoBehaviour
    {
        private Vector3 m_startPos;
        private Vector3 m_middlePos;
        private Vector3 m_targetPos;
        private Vector3 m_originalScale;
        private Vector3 m_targetScale;
        float count = 0.0f;

        public void AnimateCoin(Vector3 target)
        {
            m_startPos = transform.position;
            m_middlePos = m_startPos + (m_targetPos - m_startPos)/2 + Vector3.right * 20.0f;
            m_targetPos = target;
        }
        
        // Start is called before the first frame update
        void Awake()
        {
            m_originalScale = transform.localScale;
            m_targetScale = new Vector3(0.5f, transform.localScale.y, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
            if (count < 1.0f) {
                count += 0.75f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(m_startPos, m_middlePos, count);
                Vector3 m2 = Vector3.Lerp(m_middlePos, m_targetPos, count);
                transform.position = Vector3.Lerp(m1, m2, count);
                transform.localScale = Vector3.Lerp(transform.localScale, m_targetScale, count);
            }

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, m_targetPos) < 0.001f) {
                transform.localScale = m_originalScale;
                CoinManager.Instance.CoinOnDestination(gameObject);
            }
        }
    }
}
