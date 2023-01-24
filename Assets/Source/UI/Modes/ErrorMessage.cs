using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class ErrorMessage : Singleton<ErrorMessage>
    {
        [SerializeField] private Image m_panel;
        [SerializeField] private TextMeshProUGUI m_title;
        [SerializeField] private TextMeshProUGUI m_message;
        [SerializeField] private bool m_panelShowing = false;
        [SerializeField] private float m_dissapearDelay = 4f;
        private IEnumerator m_coroutine;

        // Creates a new error message. If panel already showing, hide it and reshow the new one.
        public void CreateErrorMessage(string title, string text)
        {
            if (m_panelShowing) {
                StopCoroutine(m_coroutine);
                HideAnimation();
            }

            m_title.text = title;
            m_message.text = text;
            m_coroutine = AnimatePanel(m_dissapearDelay);
            StartCoroutine(m_coroutine);
        }

        // Show panel, wait delay seconds, and then hide it
        private IEnumerator AnimatePanel(float delay)
        {
            m_panelShowing = true;
            // Here run code to show the panel
            ShowAnimation();

            yield return new WaitForSeconds(delay);

            m_panelShowing = false;
            // Here run code to hide panel
            HideAnimation();
        }

        private void ShowAnimation()
        {
            m_panel.gameObject.SetActive(true);   
        }

        private void HideAnimation()
        {
            m_panel.gameObject.SetActive(false); 
        }

        public override void Awake()
        {
            base.Awake();
        }
    }
}
