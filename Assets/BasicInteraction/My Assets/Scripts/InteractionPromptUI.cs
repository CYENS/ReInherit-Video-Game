using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera m_mainCam;
    [SerializeField] private GameObject m_uiPanel;
    [SerializeField] private TextMeshProUGUI m_promptText;
    private bool m_isDisplayed = false;

    public bool GetIsDisplayed()
    {
        return m_isDisplayed;
    }
    
    private void Start()
    {
        m_mainCam = Camera.main;
        m_uiPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        //Match UI rotation to camera rotation
        var rotation = m_mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetUp(string promptText)
    {
        m_promptText.text = promptText;
        m_uiPanel.SetActive(true);
        m_isDisplayed = true;
    }

    public void Close()
    {
        m_uiPanel.SetActive(false);
        m_isDisplayed = false;
    }
}
