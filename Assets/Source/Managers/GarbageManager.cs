using System;
using System.Collections;
using System.Collections.Generic;
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

        public override void Awake()
        {
            base.Awake();
        }

        protected new void Start() 
        {
            base.Start();
            m_garbageModels = m_collection.Get<Garbage>();
        }
        

        protected new void Update()
        {
            base.Update();
        }

        public bool CheckPathValidity(Vector3 source, Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Path is reachable");
                return true;
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                Debug.Log("Path is partially reachable");
                return false;
            }
            else
            {
                Debug.Log("Path is not reachable");
                return false;
            }
        }
    }
}