using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AnimalNavMove : MonoBehaviour
{
    public NavMeshAgent agent;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            // 检查agent是否在移动：有路径且没有停止且速度大于阈值
            bool isMoving = agent.hasPath && !agent.isStopped && agent.velocity.magnitude > 0.1f;
            animator.SetBool("bIsWalking", isMoving);
        }
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         agent.destination = hit.point;
        //     }
        // }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StopMoving();
        // }
    }

    public void SetTarget(Vector3 target)
    {
        agent.destination = target;
    }

    public void StopMoving()
    {
        if (agent != null)
        {
            agent.isStopped = true;  // 立即停止
            agent.ResetPath();       // 清除当前路径
            agent.velocity = Vector3.zero;  // 速度归零
        }

        if (animator != null)
        {
            animator.SetBool("bIsWalking", false);
        }
    }
}
