using UnityEngine;
using UnityEngine.AI;

public class AnimalNavMove : MonoBehaviour
{
    public NavMeshAgent agent;
    private Animator animator;
    private bool hasWalkingParameter;
    private bool hasEatingParameter;
    private bool isEating;
    private float nextPlacementAttempt;
    private bool navigationWarningReported;

    private static readonly int Walking = Animator.StringToHash("bIsWalking");
    private static readonly int Eating = Animator.StringToHash("bIsEating");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null) return;

        // 各物种的控制器不同；没有进食动画的物种仍可正常进食。
        foreach (var parameter in animator.parameters)
        {
            if (parameter.type != AnimatorControllerParameterType.Bool) continue;
            hasWalkingParameter |= parameter.nameHash == Walking;
            hasEatingParameter |= parameter.nameHash == Eating;
        }
    }

    private void Start()
    {
        EnsureAgentOnNavMesh();
    }

    private bool AgentReady => agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh;

    private void Update()
    {
        if (hasWalkingParameter && animator != null)
        {
            bool moving = !isEating && AgentReady && agent.hasPath &&
                          !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;
            animator.SetBool(Walking, moving);
        }
    }

    public void SetTarget(Vector3 target)
    {
        StopEatingAnimation();
        if (!EnsureAgentOnNavMesh()) return;

        var filter = new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask };
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 4f, filter))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    private bool EnsureAgentOnNavMesh()
    {
        if (AgentReady) return true;
        if (agent == null || !isActiveAndEnabled || Time.time < nextPlacementAttempt) return false;
        nextPlacementAttempt = Time.time + 1f;

        // 预制体中的代理默认关闭，先找到同类型的导航网格，再启用，避免创建失败。
        agent.enabled = false;
        var filter = new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask };
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 12f, filter))
        {
            if (!navigationWarningReported)
            {
                Debug.LogWarning($"{name} 附近没有可用的 NavMesh，暂时无法移动。请检查地面的导航烘焙和 Agent Type。", this);
                navigationWarningReported = true;
            }
            return false;
        }

        transform.position = hit.position;
        agent.enabled = true;
        return AgentReady;
    }

    public void StopMoving()
    {
        if (AgentReady)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
        if (hasWalkingParameter && animator != null) animator.SetBool(Walking, false);
    }

    public void PlayEatingAnimation()
    {
        if (isEating) return;
        isEating = true;
        StopMoving();
        if (hasEatingParameter && animator != null) animator.SetBool(Eating, true);
    }

    public void StopEatingAnimation()
    {
        if (!isEating) return;
        isEating = false;
        if (hasEatingParameter && animator != null) animator.SetBool(Eating, false);
    }

    public bool IsEating() => isEating;
    public bool IsAnimationPaused() => isEating;
}
