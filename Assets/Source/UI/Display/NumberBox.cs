using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A UI helper that displays a number.
    /// </summary>
    public class NumberBox : MonoBehaviour
    {
        public enum Mode { Integer = 0, Float = 1, Currency = 2 }

        [SerializeField] protected Mode m_mode;

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
        protected float m_amount = 0;
        protected float m_prevAmount = 0;

        protected float m_changeAmount = 0;


        // This is the actual amount displayed in the text box
        protected float m_actual;


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

        public void SetAmount(  float newAmount ) => m_amount = newAmount;


        /// <summary>
        /// Function that changes the text to match the number
        /// </summary>
        protected virtual void RefreshText()
        {

            switch( m_mode )
            {

                case Mode.Integer:
                    m_textbox.text = Mathf.RoundToInt( m_actual ).ToString();
                    m_changebox.text = Mathf.RoundToInt( m_changeAmount ).ToString();
                break;

                case Mode.Float:
                    m_textbox.text = String.Format("{0:0.0}", m_actual);
                    m_changebox.text = String.Format("{0:0.0}", m_changeAmount);
                break;

                case Mode.Currency:

                    int value = Mathf.RoundToInt( m_actual );
                    m_textbox.text = m_actual >= 0.0f ? "€ "+value.ToString() : "-€ "+value.ToString();
                    m_changebox.text = Mathf.RoundToInt( m_changeAmount  ).ToString();
                break;
            }
        }


        /// <summary>
        /// Changes the displayed number to match the target number.
        /// </summary>
        protected virtual void UpdateNumber()
        {
            if( Mathf.Abs(m_actual-m_amount) < float.Epsilon )
            {
                m_actual = m_amount;
                return;
            }

            float remaining = m_amount - m_actual;

            float speed = Time.deltaTime * m_speed;
            float delta = Mathf.Clamp( remaining, -speed, speed );
            m_actual += delta;
        }

        protected virtual void PlayAnimation()
        {
            bool positiveChange = (m_amount > m_prevAmount);
            m_animator.Play( positiveChange ? m_animGain : m_animLose );
        }

        // Update is called once per frame
        protected void Update()
        {
            // Detect sudden changes in the amount
            bool amountChanged = Mathf.Abs( m_amount - m_prevAmount ) > float.Epsilon;
            if( amountChanged && m_animator != null ) PlayAnimation();
            if( amountChanged ) m_changeAmount = m_amount - m_prevAmount;

            UpdateNumber();
            RefreshText();

            // Keep track of the change
            m_prevAmount = m_amount;
        }
    }
}
