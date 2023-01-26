using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class VisitorManager : Singleton<VisitorManager>
    {

        [SerializeField]
        [Tooltip("Visitor prefab to spawn")]
        private GameObject m_visitorPrefab;

        private List<Visitor> m_visitors = new List<Visitor>();


        [SerializeField] 
        [Tooltip("The overall impression that visitors got from the museum")]
        private float m_impression;


        [SerializeField][Tooltip("The number of visitors that will visit your museum on the next round")]
        private int m_visitorCount;

        [SerializeField][Tooltip("Hard limit to prevent the game from crashing!")]
        [Range(10,1000)]
        private int m_maxVisitorCount = 100;


        private void Start()
        {
            m_visitorCount = Random.Range(5, 10); 
        }




        /// <summary>
        /// Spawn visitors around the exhibits
        /// </summary>
        public void Spawn()
        {
            // Get a list of exhibits
            Exhibit[] exhibits = ArtifactManager.Instance.GetExhibits();

            // Grab interest amount and store in a 1-to-1 array
            float sumAttraction = 0.0f;
            float[] probabilities = new float[exhibits.Length];
            for(int i=0; i<exhibits.Length; i++ )
            {
                float attraction = exhibits[i].GetAttraction();
                sumAttraction += attraction;
                probabilities[i] = attraction;
            }

            // Normalize that array
            for( int i=0; i<probabilities.Length; i++ )
                probabilities[i] /= sumAttraction;

            // Distribute visitors
            for(int i=0; i<exhibits.Length; i++)
            {
                var exhibit = exhibits[i];
                int spectators = Mathf.Max( Mathf.RoundToInt(m_visitorCount * probabilities[i]), 0 );

                float angleStep = 360.0f / spectators;
                float angle = Random.Range(0, 360.0f);

                for ( int v=0; v<spectators; v++ )
                {
                    Vector3 center = exhibit.transform.position;
                    Vector3 offset = Vector3.forward * 1.5f;
                    offset = Quaternion.Euler(Vector3.up * angle) * offset;

                    GameObject temp = GameObject.Instantiate(m_visitorPrefab, transform);
                    temp.name = "Visitor";
                    temp.transform.position = center + offset;

                    angle += angleStep;

                    Visitor visitor = temp.GetComponent<Visitor>();
                    if (visitor == null) continue;

                    visitor.SetExhibit(exhibit);
                    m_visitors.Add(visitor);
                }
            }
            float meanAttraction = (exhibits.Length > 0) ? sumAttraction / exhibits.Length  : 0.0f;
 
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
