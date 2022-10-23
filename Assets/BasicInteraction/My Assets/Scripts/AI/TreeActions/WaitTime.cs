using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class WaitTime : ActionNode
    {
        private float m_duration = 1.0f;
        [SerializeField] private float m_lowRange;
        [SerializeField] private float m_highRange;
        private float m_startTime;

        private float GenerateRandomDuration()
        {
            return UnityEngine.Random.Range(m_lowRange, m_highRange);
        }
        
        protected override void OnStart() {
            m_startTime = Time.time;
            m_duration = GenerateRandomDuration();
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate()
        {
            if (blackboard.waitSeeExhibit == false)
                return State.Success;
            if (Time.time - m_startTime > m_duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
