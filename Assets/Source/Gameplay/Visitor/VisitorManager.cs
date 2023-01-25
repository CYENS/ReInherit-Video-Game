using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    public class VisitorManager : Singleton<VisitorManager>
    {

        [SerializeField]
        [Tooltip("Visitor prefab to spawn")]
        private GameObject m_visitorPrefab;

        private List<Visitor> m_visitors = new List<Visitor>();

        /// <summary>
        /// Spawn visitors around the exhibits
        /// </summary>
        public void Spawn()
        {
            // Get a list of exhibits
            Exhibit[] exhibits = ArtifactManager.Instance.GetExhibits();

            // Spawn a random number of visitors around each exhibit
            foreach( var exhibit in exhibits )
            {
                int count = Random.Range(1, 3);
                float angleStep = 360.0f / count;
                float angle = Random.Range(0, 360.0f);

                for( int i=0; i<count; i++ )
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
        }



    }
}
