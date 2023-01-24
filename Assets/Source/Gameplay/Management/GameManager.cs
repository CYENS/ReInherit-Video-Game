using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Cyens.ReInherit.Gameplay.Management
{
    /// <summary>
    /// Stores the state of the game.
    /// Keeps track of various metrics.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {

        public enum MuseumState { Close = 0, Open = 1 }

        [SerializeField]
        [Tooltip("Whether the museum is currently open or closed")]
        private MuseumState m_museumState = MuseumState.Close;


        public int funds = 1000;

        public float time;


        private bool m_pointerOverUIElement;
        public bool isPointerOverUIElement => m_pointerOverUIElement; 


        [Header("References")]
        public TMP_Text txtFunds;
        private EventSystem eventSys;

        [SerializeField]
        private GameObject closedMuseumUI;

        [SerializeField]
        private GameObject openMuseumUI;

        [SerializeField]
        private TMP_Text txtTime;


        public static int GetFunds() => Instance.funds;
        public static void SetFunds(int amount)
        {
            Instance.funds = amount;
            Instance.Refresh();
        }


        public void OpenMuseum()
        {
            if( m_museumState == MuseumState.Open )
            {
                Debug.LogError("Attempting to open museum while it is Already Open!");
                return;
            }

            // Check whether we should open museum
            if( KeeperManager.Instance.AllTasksFinished() == false )
            {
                Debug.LogError("Attempting to open museum while there are still unfinished Keeper tasks!");
                return;
            }

            m_museumState = MuseumState.Open;
            
            // TODO: Decide the amount of time for the museum to be open
            time = 65.0f;


        }


        public void Refresh()
        {
            txtFunds.text = "€" + funds.ToString();
        }

        public override void Awake()
        {
            base.Awake();
            eventSys = GetComponentInChildren<EventSystem>();
            
        }


        private void MuseumOpenUpdate()
        {
            time -= Time.deltaTime;


            if( time <= float.Epsilon )
            {
                m_museumState = MuseumState.Close;
            }


            // Update UI with the time remaining text
            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(Mathf.Repeat(time, 60.0f));
            txtTime.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        }


        private void Update()
        {
            m_pointerOverUIElement = eventSys.IsPointerOverGameObject();

            closedMuseumUI.SetActive(m_museumState == MuseumState.Close);
            openMuseumUI.SetActive(m_museumState == MuseumState.Open);

            if (m_museumState == MuseumState.Open) MuseumOpenUpdate();
        }

    }
}
