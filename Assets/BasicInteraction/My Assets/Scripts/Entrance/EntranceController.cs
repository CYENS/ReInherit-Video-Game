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
	    //For TESTING
	    public int m_numOfAgents;
	    public GameObject m_agentPrefab;
	    public Transform m_agentsParent;
	    public BoxCollider m_spawnArea;
	    public BoxCollider m_goalArea;
	    //For TESTING
	    
	    private class EntranceRow
	    {
		    public int m_id;
		    public int m_density;
		    public Vector3 m_point;

		    public EntranceRow(int id, Vector3 p)
		    {
			    m_id = id;
			    m_density = 0;
			    m_point = p;
		    }
	    }
	    
	    private List<EntranceRow> m_entranceRows;
	    private List<EntranceRow> m_exitRows;
	    public Transform m_EntranceRowsParent;
	    public Transform m_ExitRowsParent;

	    public int GetFreeSpotRowId(Vector3 agentPos)
	    {
		    //Find least busier row
		    List<float> distances = new List<float>(m_entranceRows.Count);
		    float maxDistance = 0;
		    for (int i = 0; i < m_entranceRows.Count; i++) {
			    float dis = Vector3.Distance(m_entranceRows[i].m_point, agentPos);
			    distances.Add(dis);
			    if (dis > maxDistance)
				    maxDistance = dis;
		    }

		    for (int i = 0; i < distances.Count; i++) {
			    distances[i] /= maxDistance;
		    }

		    float density = 1000;
		    int index = 0;
		    for (int i = 0; i < m_entranceRows.Count; i++) {
			    float tempDensity = m_entranceRows[i].m_density + (distances[i] * m_entranceRows.Count);
			    if (tempDensity < density) {
				    index = i;
				    density = tempDensity;
			    }
		    }

		    m_entranceRows[index].m_density += 1;
		    return index;
	    }

	    public Vector3 GetRowPoint(int id)
	    {
		    return m_entranceRows[id].m_point;
	    }

	    public void DecreaseDesnity(int id)
	    {
		    m_entranceRows[id].m_density -= 1;
	    }

	    public Vector3 GetExitRowPoint()
	    {
		    return m_exitRows[UnityEngine.Random.Range(0,m_exitRows.Count)].m_point;
	    }
	    
	    //Return random point inside a box collider
    	public Vector3 GetRandomGoalPoint()
    	{
      	  return new Vector3(
      	      Random.Range(m_goalArea.bounds.min.x, m_goalArea.bounds.max.x),
      	      0,
      	      Random.Range(m_goalArea.bounds.min.z, m_goalArea.bounds.max.z)
      	  );
    	}

        //Return spawn point based on agent's ID
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

            if (id >= spawnPoints.Count)
	            id = id % spawnPoints.Count;
            
        	return spawnPoints[id];
    	}
    
    	// Start is called before the first frame update
    	void Start()
        {
	        InitializeEntranceRows();
	        InitializeExitRows();
	        StartCoroutine(InstantiateAgents());
    	}

        private void InitializeEntranceRows()
        {
	        m_entranceRows = new List<EntranceRow>();
	        int count = 0;
	        foreach (Transform child in m_EntranceRowsParent) {
		        Transform point = child.Find("Point");
		        EntranceRow entranceRow = new EntranceRow(count, point.position);
		        m_entranceRows.Add(entranceRow);
		        count += 1;
	        }
        }
        
        private void InitializeExitRows()
        {
	        m_exitRows = new List<EntranceRow>();
	        int count = 0;
	        foreach (Transform child in m_ExitRowsParent) {
		        Transform point = child.Find("Point");
		        EntranceRow entranceRow = new EntranceRow(count, point.position);
		        m_exitRows.Add(entranceRow);
		        count += 1;
	        }
        }

        //Spawn agents with a random interval
    	private IEnumerator InstantiateAgents()
    	{
        	WaitForSeconds wait = new WaitForSeconds(UnityEngine.Random.Range(4f, 12.5f));
        	for (int i = 1; i < m_numOfAgents; i++)
        	{
            	//--------Select Spawn Area-------------
            	Vector3 spawnPoint = GetSpawnPoint(i);
            	GameObject tempAgent = Instantiate(m_agentPrefab, spawnPoint, Quaternion.identity, m_agentsParent);
                yield return wait;
        	}
    	}
    }
}
