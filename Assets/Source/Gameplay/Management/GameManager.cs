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
        [Header("Building")] public Vector2 entryRoomCoordinates = new Vector2(4.5f, 4.5f);

        public enum MuseumState { Close = 0, Open = 1 }

        [SerializeField]
        [Tooltip("Whether the museum is currently open or closed")]
        private MuseumState m_museumState = MuseumState.Close;
        public static MuseumState GetMuseumState() => Instance.m_museumState;

        private ErrorMessage m_errorMessageManager; 


        [Header("Game Data")]

        [SerializeField]
        [Tooltip("The amount of available funds to buy items and services with")]
        private int m_funds = 1000;

        public static int Funds
        {
            get => Instance.m_funds;
            set
            {
                Instance.m_funds = Mathf.Max(value,0);
                Instance.Refresh();
            }
        }


        [SerializeField][Tooltip("The star rating of your museum")]
        private float m_rating = 0.35f;

        public static float Rating
        {
            get => Instance.m_rating;
            set
            {
                Instance.m_rating = Mathf.Clamp(value, 0.0f, 5.0f);
                Instance.Refresh();
            }
        }


        public float time;



        // -----------------------
        private float m_bufferTimer;

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



        public void OpenMuseum()
        {
            if( m_museumState == MuseumState.Open )
            {
                m_errorMessageManager.CreateErrorMessage("Cannot Open Museum", 
                    "Attempting to open museum while it is Already Open!");
                return;
            }

            // A small delay to prevent restarting the museum immediately after it closes.
            // This will give the little visitors enough time to gracefully despawn.
            if(m_bufferTimer > float.Epsilon)
            {
                return;
            }

            // Check whether we should open museum
            if( KeeperManager.Instance.AllTasksFinished() == false )
            {
                m_errorMessageManager.CreateErrorMessage("Cannot Open Museum", 
                    "Attempting to open museum while there are still unfinished Keeper tasks!");
                return;
            }

            m_museumState = MuseumState.Open;
            
            // TODO: Decide the amount of time for the museum to be open
            time = 20.0f;

            // Spawn visitors
            VisitorManager.Instance.Spawn();
        }


        public void Refresh()
        {
            txtFunds.text = "€" + m_funds.ToString();
        }

        public override void Awake()
        {
            base.Awake();
            eventSys = GetComponentInChildren<EventSystem>();
            m_errorMessageManager = ErrorMessage.Instance;
        }


        public void CloseMuseum()
        {
            m_museumState = MuseumState.Close;
            VisitorManager.Instance.DeSpawn();
            ArtifactManager.Instance.UpdateNovelty();
            ArtifactManager.Instance.ApplyDamage();
            ArtifactManager.Instance.FixArtifacts();
            m_bufferTimer = 1.0f;
        }

        private void MuseumOpenUpdate()
        {
            time -= Time.deltaTime;


            if( time <= float.Epsilon )
            {
                CloseMuseum();
                return;
            }


            // Update UI with the time remaining text
            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(Mathf.Repeat(time, 60.0f));
            txtTime.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        }


        private void Update()
        {
            m_bufferTimer = Mathf.Clamp(m_bufferTimer - Time.deltaTime, 0.0f, 1.0f);

            m_pointerOverUIElement = eventSys.IsPointerOverGameObject();

            closedMuseumUI.SetActive(m_museumState == MuseumState.Close);
            openMuseumUI.SetActive(m_museumState == MuseumState.Open);

            if (m_museumState == MuseumState.Open) MuseumOpenUpdate();
        }

       

    }
}
