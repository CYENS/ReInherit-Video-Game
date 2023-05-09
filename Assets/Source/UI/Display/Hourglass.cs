using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class Hourglass : MonoBehaviour
    {

        private GameManager m_gameManager;

        [Header("References")]
        [SerializeField]
        private TMP_Text txtTime;

        // Start is called before the first frame update
        void Start()
        {
            m_gameManager = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            float time = m_gameManager.remainingTime;

            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(Mathf.Repeat(time, 60.0f));
            txtTime.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
