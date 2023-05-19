using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;
using TMPro;


namespace Cyens.ReInherit.Managers
{

    // Delegates
    public delegate void roundEnded(int round);


    /// <summary>
    /// Stores the state of the game.
    /// Keeps track of various metrics.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {

        [Header("References")]

        [SerializeField]
        private StageManager m_stageManager;


        [Header("Building")] public Vector2 entryRoomCoordinates = new Vector2(4.5f, 4.5f);

        public enum MuseumState { Close = 0, Open = 1 }

        [SerializeField]
        [Tooltip("Whether the museum is currently open or closed")]
        private MuseumState m_museumState = MuseumState.Close;
        public static MuseumState GetMuseumState() => Instance.m_museumState;

        public static bool IsMuseumOpen => Instance.m_museumState == MuseumState.Open;

        private ErrorMessage m_errorMessageManager; 


        [Header("Game Data")]

        [SerializeField]
        [Tooltip("The current game round")]
        private int m_round = 0;


        [SerializeField]
        [Tooltip("The amount of available funds to buy items and services with")]
        private int m_funds = 1000;

        [SerializeField] GameObject Quiz;

        public static int Funds
        {
            get => Instance.m_funds;
            set
            {
                Instance.m_funds = Mathf.Max(value,0);
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
            }
        }

        [Header("Variables")]
        [SerializeField] 
        private float m_time;

        public float remainingTime => m_time;


        // -----------------------
        private float m_bufferTimer;





        // Events
        public roundEnded whenRoundEnds;



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
            m_time = 30.0f;

            // Spawn visitors
            VisitorManager.Instance.Spawn();
        }

        public override void Awake()
        {
            base.Awake();
            m_errorMessageManager = ErrorMessage.Instance;

        }

        public void Start() 
        {
            
            m_stageManager = StageManager.Instance;

        }


        public void EndRound()
        {
            // Advance round
            m_round++;

            // Notify all attached events that the round has ended
            if( whenRoundEnds != null )
            {
                whenRoundEnds(m_round);
            }
        }

        public void CloseMuseum()
        {



            m_museumState = MuseumState.Close;
            VisitorManager.Instance.DeSpawn();
            ArtifactManager.Instance.UpdateNovelty();
            ArtifactManager.Instance.ApplyDamage();
            ArtifactManager.Instance.FixArtifacts();
            m_bufferTimer = 1.0f;

            EndRound();
            
            
            // Increase the size of the stage
            m_stageManager.IncreaseSize();
            

            Quiz.SetActive(true);
        }

        private void MuseumOpenUpdate()
        {
            m_time -= Time.deltaTime;


            if( m_time <= float.Epsilon )
            {
                CloseMuseum();
                return;
            }
            
        }


        private void Update()
        {
            m_bufferTimer = Mathf.Clamp(m_bufferTimer - Time.deltaTime, 0.0f, 1.0f);
            
            if (m_museumState == MuseumState.Open) 
            {
                MuseumOpenUpdate();
            }
        }

       

    }
}
