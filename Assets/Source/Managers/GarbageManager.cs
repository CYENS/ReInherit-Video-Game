using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit.Architect;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Exhibition;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Manages and coordinates a group of workers known as Janitors.
    /// </summary>
    public class GarbageManager : CollectionManager<GarbageManager, Garbage>
    {
        protected private Garbage[] m_garbageModels;
        [SerializeField] private Transform m_basePosition;
        private SortedDictionary<float, Vector3> m_garbageSpawning;
        private float m_timer = 0;
        private bool m_startPlacing = false;

        public override void Awake()
        {
            base.Awake();
            m_garbageSpawning = new SortedDictionary<float, Vector3>();
        }

        protected new void Start() 
        {
            base.Start();
            m_garbageModels = m_collection.Get<Garbage>();
        }

        private GameObject GetRandomGarbageModel()
        {
            int randIndex = UnityEngine.Random.Range(0, m_garbageModels.Length);
            return m_garbageModels[randIndex].gameObject;
        }
        
        protected new void Update()
        {
            base.Update();
            if (m_startPlacing) {
                m_timer += Time.deltaTime;
                if (m_garbageSpawning.Count > 0) {
                    var item = m_garbageSpawning.First();
                    if (m_timer >= item.Key) {
                        GameObject prefab = GetRandomGarbageModel();
                        Vector3 pos = item.Value;
                        GameObject garbage = Instantiate(prefab, pos, Quaternion.identity, transform);
                        garbage.transform.localRotation = 
                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0));
                        garbage.SetActive(true);
                        m_garbageSpawning.Remove(item.Key);
                    }
                }
            }
        }

        public void RemoveAllGarbageFromScene()
        {
            foreach (Transform garbage in transform) {
                Destroy(garbage.gameObject);
            }
        }

        public void PlanGarbageSpawning(BlockModel[] rooms, float openDuration)
        {
            m_startPlacing = false;
            m_timer = 0;
            m_garbageSpawning = new SortedDictionary<float, Vector3>();
            int roomsContainingArtifacts = 0;
            foreach (var r in rooms) {
                if (r.ContainsArtifact())
                    roomsContainingArtifacts += 1;
            }

            int quantity = roomsContainingArtifacts * UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < quantity; i++) {
                float timestep = UnityEngine.Random.Range(2f, openDuration - 5f);
                Vector3 spawnPoint = Vector3.zero;
                foreach (var r in rooms) {
                    if (r.ContainsArtifact()) {
                        spawnPoint = r.GetGarbageSpawnPosition();
                        if (spawnPoint != Vector3.zero)
                            break;
                    }
                }
                m_garbageSpawning.Add(timestep, spawnPoint);
            }

            m_startPlacing = true;
        }
    }
}