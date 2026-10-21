using UnityEngine;

/// <summary>
/// 基于效用分数选择动物当前行为的决策层。
///
/// 本组件不重复实现寻路、觅食、喝水或回巢；它只根据 NewAntTest 的状态
/// 选择分数最高的行为，并调用已有的 Needs/Home/Walk Manager 来执行。
/// 这样同一套逻辑既可驱动食草动物，也可驱动挂有 AntKillerNeedsManager 的捕食者。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NewAntTest))]
public class S_UtilityAnimal : MonoBehaviour
{
    public enum UtilityAction
    {
        Idle,
        Drink,
        Eat,
        ReturnHome,
        Sleep,
        Wander
    }

    [Header("Decision")]
    [Tooltip("重新评估需求优先级的时间间隔，动作的执行仍会每帧持续。")]
    [SerializeField, Min(0.05f)] private float decisionInterval = 0.25f;

    [Header("Utility scores")]
    [SerializeField] private float sleepScore = 1000f;
    [SerializeField] private float drinkScore = 800f;
    [SerializeField] private float eatScore = 700f;
    [SerializeField] private float homeScore = 300f;
    [SerializeField] private float wanderScore = 100f;

    [Header("Runtime (read-only)")]
    [SerializeField] private UtilityAction currentAction;
    [SerializeField] private float currentActionScore;

    private NewAntTest animal;
    private INewAntNeeds needsManager;
    private INewAntHome homeManager;
    private INewAntWalk walkManager;
    private float nextDecisionTime;
    private bool dependenciesReported;

    /// <summary>
    /// NewAntTest 使用此标识停止旧的硬编码决策链，仍保留它的昼夜和成长状态同步。
    /// </summary>
    public bool IsControllingBehavior => isActiveAndEnabled;
    public UtilityAction CurrentAction => currentAction;

    private void Awake()
    {
        ResolveDependencies();
    }

    private void OnEnable()
    {
        nextDecisionTime = 0f;
    }

    private void Update()
    {
        if (!ResolveDependencies())
        {
            return;
        }

        if (Time.time >= nextDecisionTime)
        {
            ChooseAction();
            nextDecisionTime = Time.time + decisionInterval;
        }

        ExecuteCurrentAction();
    }

    private bool ResolveDependencies()
    {
        animal ??= GetComponent<NewAntTest>();
        needsManager ??= GetComponent<INewAntNeeds>();
        homeManager ??= GetComponent<INewAntHome>();
        walkManager ??= GetComponent<INewAntWalk>();

        bool ready = animal != null && needsManager != null && homeManager != null && walkManager != null;
        if (!ready && !dependenciesReported)
        {
            Debug.LogError(
                "S_UtilityAnimal requires NewAntTest plus INewAntNeeds, INewAntHome and INewAntWalk managers.",
                this);
            dependenciesReported = true;
        }

        return ready;
    }

    private void ChooseAction()
    {
        UtilityAction bestAction = UtilityAction.Idle;
        float bestScore = 0f;

        // 成年动物在夜间必须先回巢睡觉；未成年动物沿用原项目的昼夜均可成长/觅食逻辑。
        if (animal.isAdult && !animal.isDay)
        {
            Consider(UtilityAction.Sleep, sleepScore, ref bestAction, ref bestScore);
        }

        // 保持原项目“先喝水、再吃东西”的顺序。需求管理器自行寻找各自种类的食物。
        if (!needsManager.IsDrinkWater)
        {
            Consider(UtilityAction.Drink, drinkScore, ref bestAction, ref bestScore);
        }
        else if (!needsManager.IsFull)
        {
            Consider(UtilityAction.Eat, eatScore, ref bestAction, ref bestScore);
        }

        // 吃饱的成年动物回到栖息地参与原有的配对/繁殖流程；完成繁殖后才游荡。
        if (animal.isAdult && needsManager.IsFull && !animal.isFinishReproduction)
        {
            Consider(UtilityAction.ReturnHome, homeScore, ref bestAction, ref bestScore);
        }

        if (needsManager.IsFull && (!animal.isAdult || animal.isFinishReproduction))
        {
            Consider(UtilityAction.Wander, wanderScore, ref bestAction, ref bestScore);
        }

        SetCurrentAction(bestAction, bestScore);
    }

    private static void Consider(UtilityAction candidate, float score, ref UtilityAction bestAction, ref float bestScore)
    {
        if (score > bestScore)
        {
            bestAction = candidate;
            bestScore = score;
        }
    }

    private void SetCurrentAction(UtilityAction action, float score)
    {
        if (currentAction != action && walkManager.IsWalking && action != UtilityAction.Wander)
        {
            walkManager.StopWalking();
        }

        currentAction = action;
        currentActionScore = score;
    }

    private void ExecuteCurrentAction()
    {
        switch (currentAction)
        {
            case UtilityAction.Drink:
                if (LeaveHomeIfNeeded())
                {
                    needsManager.DrinkWater();
                }
                break;

            case UtilityAction.Eat:
                if (LeaveHomeIfNeeded())
                {
                    needsManager.EatFood();
                }
                break;

            case UtilityAction.ReturnHome:
                if (!animal.isInHome)
                {
                    homeManager.GoHomeAndStay(animal, animal.MoveSpeed);
                }
                break;

            case UtilityAction.Sleep:
                if (!animal.isSleeping)
                {
                    homeManager.GoHomeAndSleep(animal, animal.MoveSpeed);
                }
                break;

            case UtilityAction.Wander:
                if (LeaveHomeIfNeeded())
                {
                    if (!walkManager.IsWalking)
                    {
                        walkManager.StartWalking(animal);
                    }

                    walkManager.UpdateWalking();
                }
                break;
        }
    }

    private bool LeaveHomeIfNeeded()
    {
        if (!animal.isInHome)
        {
            return true;
        }

        homeManager.LeaveHome(animal, animal.SetInHome);
        return false;
    }
}
