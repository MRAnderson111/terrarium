using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AnimalNavMove : MonoBehaviour
{
    public NavMeshAgent agent;
    private Animator animator;
    
    // 动画状态控制
    private bool isEating = false;
    private bool isAnimationPaused = false;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 检查进食动画是否完成
        CheckEatingAnimationComplete();
        
        // 只有在动画未暂停时才控制移动动画
        if (!isAnimationPaused && animator != null)
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
    
    /// <summary>
    /// 播放进食动画
    /// </summary>
    public void PlayEatingAnimation()
    {
        if (animator != null)
        {
            isEating = true;
            isAnimationPaused = true;
            
            // 停止移动动画，播放进食动画
            animator.SetBool("bIsWalking", false);
            animator.SetBool("bIsEating", true);
            animator.SetBool("bEatingComplete", false);
            
            Debug.Log("开始播放进食动画");
        }
    }
    
    /// <summary>
    /// 停止进食动画，恢复正常状态
    /// </summary>
    public void StopEatingAnimation()
    {
        if (animator != null)
        {
            isEating = false;
            isAnimationPaused = false;
            
            // 停止进食动画，恢复移动动画控制
            animator.SetBool("bIsEating", false);
            animator.SetBool("bEatingComplete", false);
            
            Debug.Log("停止进食动画，恢复正常状态");
        }
    }
    
    /// <summary>
    /// 检查进食动画是否完成
    /// </summary>
    private void CheckEatingAnimationComplete()
    {
        if (isEating && animator != null)
        {
            // 检查Animator中的bEatingComplete参数
            if (animator.GetBool("bEatingComplete"))
            {
                Debug.Log("检测到进食动画完成");
                StopEatingAnimation();
            }
        }
    }
    
    /// <summary>
    /// 获取当前是否正在进食
    /// </summary>
    public bool IsEating()
    {
        return isEating;
    }
    
    /// <summary>
    /// 获取当前动画是否被暂停
    /// </summary>
    public bool IsAnimationPaused()
    {
        return isAnimationPaused;
    }
}
