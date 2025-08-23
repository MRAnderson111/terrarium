using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AnimalNavMove : MonoBehaviour
{
    public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         agent.destination = hit.point;
        //     }
        // }
    }

    public void SetTarget(Vector3 target)
    {
        agent.destination = target;
    }
}
