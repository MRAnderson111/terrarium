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
        TryPlaceOnNavMesh();
    }

    void Update()
    {
        CheckEatingAnimationComplete();

        if (!isAnimationPaused && animator != null && agent != null && agent.isOnNavMesh)
        {
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
        if (!EnsureAgentOnNavMesh())
            return;

        agent.isStopped = false;
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    /// <summary>
    /// 昆虫生成点经常高于烘焙好的 NavMesh，代理创建失败后每帧 SetDestination 都会抛异常，移动逻辑就此停住。
    /// </summary>
    bool EnsureAgentOnNavMesh()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (agent == null || !agent.enabled)
            return false;
        if (agent.isOnNavMesh)
            return true;

        return TryPlaceOnNavMesh();
    }

    bool TryPlaceOnNavMesh()
    {
        if (agent == null)
            return false;

        if (agent.isOnNavMesh)
            return true;

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 12f, NavMesh.AllAreas))
            return false;

        // 代理离 NavMesh 太远时会创建失败，先挪到网格上再重新启用。
        agent.enabled = false;
        transform.position = hit.position;
        agent.enabled = true;
        return agent.isOnNavMesh;
    }

    public void StopMoving()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
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
