using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour {

	public GameObject critterPrefab;
	public int quantity;
	public float spawnRadius;
	public bool SpawnOnStart = false;

	void Start ()
	{
        if(SpawnOnStart)
		SpawnAll();
	}

	void SpawnAll()
	{
		for (int i = 0; i < quantity; i++)
        {
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
            {
                Instantiate(critterPrefab, randomPoint, Quaternion.identity);
            }
            else
                i--;

        }
	}

	void OnTriggerEnter(Collider collider) 
	{
		if(!SpawnOnStart && collider.gameObject.tag == "Player")
		SpawnAll();
	}
		
	
}	
	