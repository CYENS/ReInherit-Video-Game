using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cyens.ReInherit
{
    public class NumberBox : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] protected TMP_Text m_textbox;
        [SerializeField] protected TMP_Text m_changebox;


        // An animator can be used to create special effects whenever the amount changes.
        protected Animator m_animator;


        [Header("Animations")]
        [Tooltip("What animation state to play when you gain money")]
        [SerializeField] protected string m_animGain = "Gain";

        [Tooltip("What animation to play when you lose money")]
        [SerializeField] protected string m_animLose = "Lose";
        


        [Header("Parameters")]

        [SerializeField]
        protected int m_amount = 0;
        protected int m_prevAmount = 0;

        protected int m_changeAmount = 0;


        // This is the actual amount displayed in the text box
        protected int m_actual;


        [SerializeField]
        [Tooltip("How fast to update the amount in the text box")]
        protected float m_speed = 10.0f;

        // Internal timer for animation purposes
        protected float m_timer;


        // Start is called before the first frame update
        void Start()
        {
            m_animator = GetComponent<Animator>();
        }

        public void SetAmount( int newAmount ) =>  m_amount = newAmount;
        public void AddAmount( int addedAmount ) => m_amount += addedAmount;


        /// <summary>
        /// Function that changes the text to match the number
        /// </summary>
        protected virtual void RefreshText()
        {
            m_textbox.text = m_actual.ToString();
            m_changebox.text = m_changeAmount.ToString();
        }


        /// <summary>
        /// Changes the displayed number to match the target number.
        /// </summary>
        protected virtual void UpdateNumber()
        {
            if( m_actual == m_amount ) return;

            m_timer += Time.deltaTime * m_speed;
            if( m_timer > 1.0f )
            {
                // Add more than 1 point each time, otherwise it will be unbearably slow.
                // Increasing the frequency won't matter either, because at some point it will be
                // bound by the game's frame rate.
                int remaining = m_amount - m_actual;
                remaining = Mathf.Clamp(remaining, -10,10);
                m_actual += remaining;
                m_timer = 0.0f;
            }
        }

        protected virtual void PlayAnimation()
        {
            bool positiveChange = (m_amount > m_prevAmount);
            m_animator.Play( positiveChange ? m_animGain : m_animLose );
        }

        // Update is called once per frame
        protected void Update()
        {
            bool amountChanged = (m_amount != m_prevAmount);
            if( amountChanged && m_animator != null ) PlayAnimation();
            if( amountChanged ) m_changeAmount = m_amount - m_prevAmount;

            UpdateNumber();
            RefreshText();
            m_prevAmount = m_amount;
        }
    }
}
