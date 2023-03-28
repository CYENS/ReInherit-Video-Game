using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class BudgetBox : NumberBox
    {
        public Vector3 pos;
        [SerializeField] private float m_scaleAnimSpeed = 0.75f;
        private RectTransform m_cashierImage;
        private bool m_coinCollected = false;
        
        protected override void RefreshText()
        {
            base.RefreshText();
            m_textbox.text = "€" + m_actual.ToString();
        }

        public void CoinCollected()
        {
            m_coinCollected = true;
        }

        private void Awake()
        {
            m_cashierImage = transform.Find("CashRegister").GetComponent<RectTransform>();
        }

        protected new void Update() 
        {
            if (m_coinCollected) {
                float val = Mathf.PingPong(m_scaleAnimSpeed * Time.time, 0.1f);
                m_cashierImage.localScale = Vector3.one + (Vector3.one * val);
                if (val <= 0.01f)
                    m_coinCollected = false;
            }

            // Get number amount from Game Manager
            int funds = GameManager.Funds;
            SetAmount(funds);

            // Everything else should work the same
            base.Update();
            
            pos = Camera.main.ViewportToWorldPoint(transform.position);
        }
    }
}
