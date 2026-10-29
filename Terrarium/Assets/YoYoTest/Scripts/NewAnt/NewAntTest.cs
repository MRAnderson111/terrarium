using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAntTest : MonoBehaviour,IGetObjectClass, INewAnt
{
    //是否是"成虫"
    public bool isAdult;
    //是否是"白天"
    public bool isDay;
    //是否"完成繁殖"
    public bool isFinishReproduction;
    //是否在"睡觉"
    public bool isSleeping;
    //是否在"居住地内"
    public bool isInHome;
    
    // 引用需求管理器
    private INewAntNeeds needsManager;
    
    // 引用居住地管理器
    private INewAntHome homeManager;
    
    // 引用散步管理器
    private INewAntWalk walkManager;
    
    // 引用导航移动组件
    private AnimalNavMove navMove;
    
    // 当前食物目标对象
    private GameObject currentFoodTarget;

    // 新的效用决策器存在时，由它选择行为；本脚本仍负责昼夜、成长和碰撞状态同步。
    private S_UtilityAnimal utilityAnimal;

    //成虫睡觉时间
    public float toSleepTime = 20f; // 默认晚上8点睡觉
    //成虫醒来时间
    public float toAwakeTime = 6f;  // 默认早上6点醒来
    
    // 蚂蚁移动速度
    public float moveSpeed = 2f;
    public float MoveSpeed => moveSpeed;

    [Header("Flee")]
    [Tooltip("被攻击时的移动速度，相对当前平时速度。")]
    public float fleeSpeedMultiplier = 0.8f;
    [Tooltip("每次朝远离捕食者的方向逃出的距离。")]
    public float fleeDistance = 3f;
    [Tooltip("最后一次受击后继续逃跑的时间，避免刚脱离接触就停住。")]
    public float fleeHoldDuration = 0.6f;

    private Vector3 fleeThreatPosition;
    private float fleeUntil;
    private bool fleeActive;

    public bool IsFleeing => fleeActive && Time.time < fleeUntil;

    // 上一次的白天状态，用于检测状态变化
    private bool wasDayLastFrame = false;
    
    // 上一次的成年状态，用于检测成年状态变化
    private bool wasAdultLastFrame = false;
    
    public string bigClass;

    public string BigClass => bigClass;
    public string smallClass;

    public string SmallClass => smallClass;

    // 白天开始事件
    public event System.Action OnDayStart;



    // Start is called before the first frame update
    void Start()
    {
        // 获取需求管理器组件
        needsManager = GetComponent<INewAntNeeds>();
        
        // 获取居住地管理器组件
        homeManager = GetComponent<INewAntHome>();
        
        // 获取散步管理器组件
        walkManager = GetComponent<INewAntWalk>();
        
        // 获取导航移动组件
        navMove = GetComponent<AnimalNavMove>();

        // 试点接入 Utility AI 时避免新旧两套决策逻辑同时控制同一只动物。
        utilityAnimal = GetComponent<S_UtilityAnimal>();

        // 绑定事件监听器
        OnDayStart += CheckIfAntIsSleepingWhenDayStarts;

        Events.OnCreateObject.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        StateCheck();

        if (TryFlee())
        {
            return;
        }

        if (utilityAnimal != null && utilityAnimal.IsControllingBehavior)
        {
            return;
        }
        
        // 如果蚂蚁正在睡觉，则不执行任何操作
        if (isSleeping)
        {
            return;
        }
        
        // 更新散步状态
        if (walkManager != null)
        {
            walkManager.UpdateWalking();
        }

        if (isAdult)
        {
            if (isDay)
            {
                if (needsManager.IsFull)
                {
                    if (homeManager.IsHaveHome)
                    {
                        if (isFinishReproduction)
                            TakeAWalk();
                        else
                            GoHomeAndStay();
                    }
                    else
                    {
                        homeManager.FindOrCreateHome(bigClass, smallClass, transform.position);
                    }
                }
                else
                {
                    FindAndEat();
                }
            }
            else if (homeManager.IsHaveHome)
            {
                GoHomeAndSleep();
            }
            else
            {
                homeManager.FindOrCreateHome(bigClass, smallClass, transform.position);
                GoHomeAndSleep();
            }
        }
        else
        {
            FindAndEat();
        }
    }

    /// <summary>
    /// 捕食者造成伤害时调用。猎物朝远离威胁的方向逃跑。
    /// </summary>
    public void FleeFrom(Vector3 threatPosition)
    {
        fleeThreatPosition = threatPosition;
        fleeUntil = Time.time + Mathf.Max(0.05f, fleeHoldDuration);
        if (fleeActive)
        {
            return;
        }

        fleeActive = true;
        navMove ??= GetComponent<AnimalNavMove>();
        walkManager ??= GetComponent<INewAntWalk>();
        homeManager ??= GetComponent<INewAntHome>();

        isSleeping = false;
        walkManager?.StopWalking();
        navMove?.StopEatingAnimation();
        if (isInHome && homeManager != null)
        {
            homeManager.LeaveHome(this, SetInHome);
        }

        ApplyFleeDestination();
    }

    private bool TryFlee()
    {
        if (Time.time >= fleeUntil)
        {
            fleeActive = false;
            return false;
        }

        ApplyFleeDestination();
        return true;
    }

    private void ApplyFleeDestination()
    {
        navMove ??= GetComponent<AnimalNavMove>();
        if (navMove == null)
        {
            return;
        }

        Vector3 away = transform.position - fleeThreatPosition;
        away.y = 0f;
        if (away.sqrMagnitude < 0.01f)
        {
            away = Vector3.forward;
        }

        navMove.StopEatingAnimation();
        navMove.SetTarget(transform.position + away.normalized * Mathf.Max(0.5f, fleeDistance));
    }

    private void LateUpdate()
    {
        if (!IsFleeing || navMove == null || navMove.agent == null)
        {
            return;
        }

        navMove.agent.speed = moveSpeed * Mathf.Clamp(fleeSpeedMultiplier, 0.1f, 1f);
    }

    private void StateCheck()
    {
        IsDayCheck();
        CheckAdultTransition();
    }

    private void IsDayCheck()
    {
        // 获取当前时间
        float currentTime = DTimeManager.Instance.CurrentTime;

        // 判断是否是白天
        // 处理24小时循环逻辑：如果睡觉时间 > 醒来时间，说明跨天（比如20点睡觉，6点醒来）
        if (toSleepTime > toAwakeTime)
        {
            // 跨天情况：白天是醒来时间到睡觉时间之间
            isDay = currentTime >= toAwakeTime && currentTime < toSleepTime;
        }
        else
        {
            // 不跨天情况：白天是睡觉时间到醒来时间之间
            isDay = currentTime >= toSleepTime || currentTime < toAwakeTime;
        }

        // 检测从黑夜切换到白天的瞬间
        if (!wasDayLastFrame && isDay)
        {
            // 触发白天开始事件
            OnDayStart?.Invoke();
        }

        // 更新上一次的状态
        wasDayLastFrame = isDay;

        // 调试输出
        
    }



    private void TakeAWalk()
    {
        // 检查是否已经在散步，避免重复调用
        if (walkManager != null && walkManager.IsWalking)
        {
            return;
        }

        
        // 如果蚂蚁在居住地内，先离开居住地
        if (isInHome)
        {
            homeManager.LeaveHome(this, (isNowInHome) => {
                this.isInHome = isNowInHome; // 这里回调会传入false
                // 离开居住地后开始散步
                walkManager.StartWalking(this);
            });
        }
        else
        {
            // 不在居住地内，直接开始散步
            walkManager.StartWalking(this);
        }
    }


    private void GoHomeAndStay()
    {
        // 停止散步状态，避免与回家移动逻辑冲突
        if (walkManager != null)
        {
            walkManager.StopWalking();
        }
        
        // 使用居住地管理器的回家方法
        homeManager.GoHomeAndStay(this, moveSpeed);
    }

    private void FindAndEat()
    {
        // 在寻找食物前，检查是否需要先离开居住地
        if (isInHome)
        {
            homeManager.LeaveHome(this, (isNowInHome) => {
                this.isInHome = isNowInHome; // 回调会传入false
                // 离开居住地后开始寻找食物
                ExecuteFindAndEatSequence();
            });
        }
        else
        {
            // 如果不在居住地内，直接开始寻找食物
            ExecuteFindAndEatSequence();
        }
    }

    /// <summary>
    /// 执行寻找食物和水的具体逻辑
    /// </summary>
    private void ExecuteFindAndEatSequence()
    {
        if (needsManager.IsFull)
        {
            return;
        }

        if (needsManager.IsDrinkWater)
        {
            needsManager.EatFood();
            currentFoodTarget = needsManager.FoodTarget;
        }
        else
        {
            needsManager.DrinkWater();
        }
    }


    private void GoHomeAndSleep()
    {
        // 停止散步状态，避免与回家移动逻辑冲突
        if (walkManager != null)
        {
            walkManager.StopWalking();
        }
        
        // 使用居住地管理器的回家方法
        homeManager.GoHomeAndSleep(this, moveSpeed);
    }

    /// <summary>
    /// 当白天开始时检查蚂蚁是否在睡觉状态
    /// </summary>
    private void CheckIfAntIsSleepingWhenDayStarts()
    {
        // 当白天开始时，无论蚂蚁是否在睡觉，都重置其状态
        // 这确保了在回家途中被清晨打断的蚂蚁也能正确开始新的一天
        ResetStates();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "water")
        {
            needsManager.IsTouchWaterTarget = true;
        }
        
        // Utility AI 直接调用需求管理器，因此以管理器当前目标为准；旧流程仍可使用 currentFoodTarget。
        GameObject foodTarget = needsManager != null && needsManager.FoodTarget != null
            ? needsManager.FoodTarget
            : currentFoodTarget;

        // 精确检查：碰撞对象是否就是当前食物目标对象
        if (foodTarget != null && other.attachedRigidbody != null && other.attachedRigidbody.gameObject == foodTarget)
        {
            needsManager.IsTouchFoodTarget = true;
            return;
        }
        
        // 检查碰撞对象是否有刚体和IGetObjectClass组件
        if (other.attachedRigidbody == null)
            return;
            

        IGetObjectClass objectClass = other.attachedRigidbody.GetComponent<IGetObjectClass>();
        if (objectClass == null)
            return;
        


    }

    private void ResetStates()
    {
        // 重置需求状态
        // needsManager.isFull = false; // This is now managed within AntNeedsManager
        needsManager.ResetStates();
        
        // 重置当前食物目标
        currentFoodTarget = null;
        
        // 如果蚂蚁在重置状态时仍在居住地内，则需要先执行离开逻辑
        if (isInHome)
        {
            // 调用LeaveHome来确保蚂蚁从居住地的列表中被移除
            // 这里我们不需要回调，因为我们将在下面手动设置isInHome为false
            homeManager.LeaveHome(this, null);
        }
        
        // 重置蚂蚁自身状态
        isFinishReproduction = false;
        isSleeping = false;
        isInHome = false;
        
        // 停止散步状态
        if (walkManager != null)
        {
            walkManager.StopWalking();
        }
        
    }

    public void Death()
    {
        Events.OnDestroyObject.Invoke(this);
        Destroy(gameObject);
    }

    public void SetAdult(bool isAdult)
    {
        this.isAdult = isAdult;
    }
    
    /// <summary>
    /// 检查蚂蚁是否从未成年变成成年，如果是则触发成长动画
    /// </summary>
    private void CheckAdultTransition()
    {
        // 检测从未成年到成年的状态变化
        if (!wasAdultLastFrame && isAdult)
        {
            StartCoroutine(GrowthAnimation());
        }
        
        // 更新上一帧的成年状态
        wasAdultLastFrame = isAdult;
    }
    
    /// <summary>
    /// 成长动画协程，在0.2秒内将scale从1变为1.2
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrowthAnimation()
    {
        float duration = 0.2f; // 动画持续时间
        float startScale = 1.0f; // 初始scale
        float targetScale = 1.2f; // 目标scale
        float elapsedTime = 0f;
        
        Vector3 initialScale = transform.localScale;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // 使用线性插值计算当前scale
            float currentScale = Mathf.Lerp(startScale, targetScale, progress);
            transform.localScale = initialScale * currentScale;
            
            yield return null; // 等待下一帧
        }
        
        // 确保最终scale准确设置为目标值
        transform.localScale = initialScale * targetScale;
    }

    public void SetInHome(bool inHome)
    {
        this.isInHome = inHome;
    }

    public void SetSleeping(bool sleeping)
    {
        this.isSleeping = sleeping;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
