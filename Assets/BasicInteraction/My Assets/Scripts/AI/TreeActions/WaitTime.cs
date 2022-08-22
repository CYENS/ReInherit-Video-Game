using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class WaitTime : ActionNode
    {
        [SerializeField] private float m_duration = 1.0f;
        [SerializeField] private bool m_randomDuration = false;
        private float m_startTime;

        private float GenerateRandomDuration()
        {
            return UnityEngine.Random.Range(0f, 5f);
        }
        
        protected override void OnStart() {
            m_startTime = Time.time;
            if (m_randomDuration) m_duration = GenerateRandomDuration();
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (Time.time - m_startTime > m_duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
