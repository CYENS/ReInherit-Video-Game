using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit.Exhibition;
using UnityEngine;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;
using Random = UnityEngine.Random;

namespace Cyens.ReInherit
{
    public class VisitorManager : Singleton<VisitorManager>
    {
        [SerializeField]
        [Tooltip("Visitor prefab to spawn")]
        private GameObject m_visitorPrefab;

        [SerializeField] private Transform m_visitorsParent;
        
        private List<Visitor> m_visitors = new List<Visitor>();
        
        [SerializeField] 
        [Tooltip("The overall impression that visitors got from the museum")]
        private float m_impression;
        
        [SerializeField][Tooltip("The number of visitors that will visit your museum on the next round")]
        private int m_visitorCount;
        
        [SerializeField][Tooltip("Hard limit to prevent the game from crashing!")]
        [Range(10,1000)]
        private int m_maxVisitorCount = 100;
        
        [SerializeField] private Vector3 m_exitPosition;
        
        private Exhibit[] m_displayExhibits;
        
        private ExhibitVisitorHandler[] m_handlers;

        public Exhibit[] GetDisplayExhibits() { return m_displayExhibits; }
        public ExhibitVisitorHandler[] GetVisitorHandlers() { return m_handlers; }

        public Vector3 GetExitPosition()
        {
            Vector3 randomizeX = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, 0f);
            return m_exitPosition + randomizeX;
        }
        
        private void Start()
        {
            m_visitorCount = Random.Range(15, 20);
            m_exitPosition = GameObject.Find("Exit").transform.position;
        }

        // Find closer visitor in maxDistance
        public Visitor FindCloserAgent(Visitor current, float maxDistance)
        {
            Visitor closerAgent = null;
            float dis = 100000f;
            foreach (var visitor in m_visitors) {
                if (visitor != null && current != visitor) {
                    float temp = Vector3.Distance(current.transform.position, visitor.transform.position);
                    if (temp < dis) {
                        dis = temp;
                        closerAgent = visitor;
                    }
                }
            }

            if (dis <= maxDistance)
                return closerAgent;
            return null;
        }

        /// <summary>
        /// Spawn visitors around the exhibits
        /// </summary>
        public void Spawn()
        {
            // Get a list of exhibits
            m_displayExhibits = ExhibitManager.Instance.GetExhibitsByState(Exhibit.State.Display);

            int visitorID = 0;
            // Grab interest amount and store in a 1-to-1 array
            float sumAttraction = 0.0f;
            float[] probabilities = new float[m_displayExhibits.Length];
            for(int i = 0; i < m_displayExhibits.Length; i++ ) {
                // TO-DO get exhibit attraction
                float attraction = 1f;
                sumAttraction += attraction;
                probabilities[i] = attraction;
            }

            // Normalize that array
            for( int i=0; i<probabilities.Length; i++ )
                probabilities[i] /= sumAttraction;

            // Distribute visitors
            m_handlers = new ExhibitVisitorHandler[m_displayExhibits.Length];
            for(int i = 0; i < m_displayExhibits.Length; i++)
            {
                var exhibit = m_displayExhibits[i];

                var handler = exhibit.GetVisitorHandler();
                m_handlers[i] = handler;

                int spectators = Mathf.Max( Mathf.RoundToInt(m_visitorCount * probabilities[i]), 0 );

                for ( int v = 0; v < spectators; v++ )
                {
                    GameObject temp = GameObject.Instantiate(m_visitorPrefab, m_visitorsParent);
                    temp.name = "Visitor";

                    Visitor visitor = temp.GetComponent<Visitor>();
                    if (visitor == null) continue;

                    visitor.ID = visitorID;
                    visitor.InSpawnArtifact = true;
                    visitorID += 1;

                    
                    Vector3 freeSlot;
                    if( ! handler.TryGetFreeSpot(out freeSlot) )
                    {
                        // Not enough free space
                        Destroy(temp);
                        break;
                    }

                    visitor.visitedExhibits.Add(handler);
                    visitor.SetExhibit(handler, (int)freeSlot.y);
                    visitor.transform.position = freeSlot;
                    
                    Vector3 targetPos = m_displayExhibits[i].transform.position;
                    Vector3 lookDir = targetPos - visitor.transform.position;
                    lookDir.y = 0.0f;
                    visitor.transform.rotation = Quaternion.LookRotation(lookDir);

                    m_visitors.Add(visitor);
                }
            }
            float meanAttraction = (m_displayExhibits.Length > 0) ? sumAttraction / m_displayExhibits.Length  : 0.0f;
 
            // Calculate base impression based on how well the museum is setup;
            // TODO: Consider how this impression may be affected while the museum is open
            m_impression = meanAttraction;
        }
        
        public void DeSpawn()
        {
            foreach (var visitor in m_visitors)
            {
                if (visitor == null) continue;
                if (visitor.gameObject == null) continue;

                visitor.DeSpawn();
            }
            m_visitors.Clear();

            // TODO: Calculate formula to calculate the total funds
            // ? Donations, Ticket sales?
            float averageDonation = Mathf.Lerp(5.0f, 100.0f, GameManager.Rating);
            float donationProbability = Mathf.Lerp(0.1f, 0.3f, m_impression);
            float gainedFunds = m_visitorCount * donationProbability * averageDonation;

            // TODO: We need some flashy effect to show that you gained funds
            GameManager.Funds += Mathf.RoundToInt(gainedFunds);

            //TODO: grants (extra funds) when reaching a milestone?
            // TODO: extra funds from shops

            // TODO: Decide on how many more visitors you'll have based on how well the museum did
            float percentageIncrease = Mathf.Lerp(0.0f, 0.2f, m_impression);
            m_visitorCount += Mathf.RoundToInt( m_visitorCount * percentageIncrease );
            m_visitorCount = Mathf.Min(m_visitorCount, m_maxVisitorCount);


            // TODO: Decide on how much the rating increases

        }



    }
}
