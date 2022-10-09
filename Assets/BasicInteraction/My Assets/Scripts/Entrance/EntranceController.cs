using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

namespace Cyens.ReInherit
{
    public class EntranceController : MonoBehaviour
    {
        public int m_numOfAgents;
    	public GameObject m_agentPrefab;
    	public Transform m_agentsParent;
    	public BoxCollider m_spawnArea;
    	public BoxCollider m_goalArea;
        public BoxCollider m_areaCollider;

	    //Return random point inside a box xollider
    	private Vector3 GetRandomGoalPoint()
    	{
      	  return new Vector3(
      	      Random.Range(m_goalArea.bounds.min.x, m_goalArea.bounds.max.x),
      	      0,
      	      Random.Range(m_goalArea.bounds.min.z, m_goalArea.bounds.max.z)
      	  );
    	}

    	private Vector3 GetSpawnPoint(int id)
    	{
        	float spawnHeight = Mathf.Abs(m_spawnArea.bounds.max.x - m_spawnArea.bounds.min.x);
        	float spawnWidth = Mathf.Abs(m_spawnArea.bounds.max.z - m_spawnArea.bounds.min.z);
        	float agentSize = (m_agentPrefab.GetComponent<IAstarAI>().radius * 2f + 0.3f);
        	int gridHeight = (int)Mathf.Ceil(spawnHeight / agentSize);
        	int gridWidth = (int)Mathf.Ceil(spawnWidth / agentSize);
        	float heightStep = spawnHeight / gridHeight;
        	float widthStep = spawnWidth / gridWidth;

        	List<Vector3> spawnPoints = new List<Vector3>(m_numOfAgents);

            for (int j = gridWidth - 1; j > 0; j--)
            {
                for (int i = 1; i < gridHeight; i++)
                {
                    Vector3 point = new Vector3(m_spawnArea.bounds.min.x + (heightStep * i), 1.01f,
                        m_spawnArea.bounds.min.z + (widthStep * j));
                    spawnPoints.Add(point);
                }
            }

        	return spawnPoints[id];
    	}
    
    	// Start is called before the first frame update
    	void Start()
    	{
        	StartCoroutine(InstantiateAgents());
    	}

        //Spawn agents with a random interval
    	private IEnumerator InstantiateAgents()
    	{
        	WaitForSeconds wait = new WaitForSeconds(UnityEngine.Random.Range(0.75f, 2.25f));
        	for (int i = 1; i < m_numOfAgents; i++)
        	{
            	//--------Select Spawn Area-------------
            	Vector3 spawnPoint = GetSpawnPoint(i);
            	GameObject tempAgent = Instantiate(m_agentPrefab, spawnPoint, Quaternion.identity, m_agentsParent);
            	//--------Select Goal Area-------------
            	tempAgent.GetComponent<IAstarAI>().destination = GetRandomGoalPoint();
            
            	yield return wait;
        	}
    	}

        //Calculate and assign penalty to nodes based on desnity
        private void CalculateNodeDensity()
        {
	        RecastGraph navmeshGraph = AstarPath.active.data.recastGraph;
	        int[] nodesAgents = new int[navmeshGraph.CountNodes()];
	        for (int i = 0; i < nodesAgents.Length; i++) {
		        nodesAgents[i] = 0;
	        }
	        
	        //Count number of agents in each node
	        foreach (Transform agent in m_agentsParent)
	        {
		        int index = navmeshGraph.GetNearest(agent.position).node.NodeIndex - 1;
		        nodesAgents[index] += 1;
	        }

	        navmeshGraph.GetNodes(node => {
		        node.Penalty = 0;
		        if (m_areaCollider.bounds.Contains((Vector3)node.position)) {
			        node.Penalty = (uint)nodesAgents[node.NodeIndex - 1] * 7500;
		        }
	        });
        }

        private void Update()
        {
	        CalculateNodeDensity();
        }
    }
}
