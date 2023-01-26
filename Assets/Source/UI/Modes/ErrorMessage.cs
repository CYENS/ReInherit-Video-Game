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
        [SerializeField] private Image m_image;
        [SerializeField] private TextMeshProUGUI m_title;
        [SerializeField] private TextMeshProUGUI m_message;
        [SerializeField] private bool m_panelShowing = false;
        [SerializeField] private float m_dissapearDelay = 4f;
        private IEnumerator m_coroutine;
        private float time = 0.5f;

        // Creates a new error message. If panel already showing, hide it and reshow the new one.
        public void CreateErrorMessage(string title, string text)
        {
            if (m_panelShowing)
            {
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

            StartCoroutine(LerpImageColorAlpha(m_panel, 0.0f, 1f));
            StartCoroutine(LerpImageColorAlpha(m_image, 0.0f, 1f));
            StartCoroutine(LerpTextColorAlpha(m_title, 0.0f, 1f));
            StartCoroutine(LerpTextColorAlpha(m_message, 0.0f, 1f));
            StartCoroutine(LerpHeight());
            m_panel.gameObject.SetActive(true);

        }

        private void HideAnimation()
        {
            
            StartCoroutine(LerpImageColorAlpha(m_panel, 1.0f, 0f));
            StartCoroutine(LerpImageColorAlpha(m_image, 1.0f, 0f));
            StartCoroutine(LerpTextColorAlpha(m_title, 1.0f, 0f));
            StartCoroutine(LerpTextColorAlpha(m_message, 1.0f, 0f));
            StartCoroutine(HideAfterTime());
        }

        public override void Awake()
        {
            base.Awake();
        }

        private IEnumerator LerpImageColorAlpha(Image imageToLerp, float start, float end)
        {
            Color newColor = imageToLerp.color;
            float currentTime = 0f;
            while (currentTime < time)
            {
                newColor.a = Mathf.Lerp(start, end, currentTime / time);
                imageToLerp.color = newColor;
                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator LerpTextColorAlpha(TextMeshProUGUI textToLerp, float start, float end)
        {
            Color newColor = textToLerp.color;
            float currentTime = 0f;
            while (currentTime < time)
            {
                newColor.a = Mathf.Lerp(start, end, currentTime / time);
                textToLerp.color = newColor;
                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator HideAfterTime()
        {
            yield return new WaitForSeconds(time);

            // Code to execute after the delay
            m_panel.gameObject.SetActive(false);
        }

        private IEnumerator LerpHeight()
        {
            float currentTime = 0f;
            Vector3 panelPosition = m_panel.rectTransform.position;
            while (currentTime < time)
            {
                m_panel.transform.position = new Vector3(panelPosition.x, Mathf.Lerp(panelPosition.y + 100, panelPosition.y, currentTime / time), panelPosition.z);

                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
