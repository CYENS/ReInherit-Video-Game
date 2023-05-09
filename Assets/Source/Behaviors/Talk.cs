using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Talk : StateMachineBehaviour
    {
        private float m_timeCounter;
        private Visitor m_visitor;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_visitor = animator.GetComponent<Visitor>();
            m_visitor.VisitorBehavior = Visitor.Behavior.Talking;
            m_timeCounter = m_visitor.TalkDuration;
            m_visitor.RotateVisitorTowardsDestination();
            Visitor.Emotion emotion = (Visitor.Emotion)UnityEngine.Random.Range(0, 5);
            animator.SetFloat("Emotion", ((float)emotion / 5f));
            m_visitor.VisitorEmotion = emotion;
            m_visitor.ChatBubble.Setup(emotion);
            m_visitor.ChatBubble.ShowBubble(true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            float dis = 0;
            if(m_visitor.TalkVisitor == null)
                ExitTalkState(animator);
            else {
                dis = Vector3.Distance(animator.transform.position, m_visitor.TalkVisitor.transform.position);
            }

            m_timeCounter -= Time.deltaTime;
            
            if (m_timeCounter <= 0 || dis >= 3f) {
                ExitTalkState(animator);
            }
            animator.SetFloat("TalkTimer", m_timeCounter);
        }
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        private void ExitTalkState(Animator animator)
        {
            m_visitor.TalkVisitor = null;
            m_visitor.ChatBubble.ShowBubble(false);
            animator.SetBool("OpenToTalk", false);
            animator.SetFloat("TalkTimer", 0f);
            animator.SetBool("Talk", false);
        }
    }
}