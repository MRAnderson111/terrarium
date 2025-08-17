using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAntTest : MonoBehaviour
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
    private AntNeedsManager needsManager;
    
    // 引用居住地管理器
    private AntHomeManager homeManager;
    
    // 引用散步管理器
    private AntWalkManager walkManager;

    //成虫睡觉时间
    public float toSleepTime = 20f; // 默认晚上8点睡觉
    //成虫醒来时间
    public float toAwakeTime = 6f;  // 默认早上6点醒来
    
    // 蚂蚁移动速度
    public float moveSpeed = 2f;

    // 上一次的白天状态，用于检测状态变化
    private bool wasDayLastFrame = false;

    // 白天开始事件
    public event System.Action OnDayStart;



    // Start is called before the first frame update
    void Start()
    {
        // 获取需求管理器组件
        needsManager = GetComponent<AntNeedsManager>();
        
        // 获取居住地管理器组件
        homeManager = GetComponent<AntHomeManager>();
        
        // 获取散步管理器组件
        walkManager = GetComponent<AntWalkManager>();

        // 绑定事件监听器
        OnDayStart += CheckIfAntIsSleepingWhenDayStarts;
    }

    // Update is called once per frame
    void Update()
    {
        StateCheck();
        
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
                if (needsManager.isFull)
                {
                    Debug.Log("成虫白天吃饱了");
                    if (homeManager.IsHaveHome)
                    {
                        if (isFinishReproduction)
                        {
                            Debug.Log("成虫白天吃饱了，有居住地，完成繁殖");
                            TakeAWalk();
                        }
                        else
                        {
                            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖");
                            GoHomeAndStay();
                        }
                        // Debug.Log("成虫白天吃饱了，有居住地");
                        // GoHomeAndStay();
                    }
                    else
                    {
                        Debug.Log("成虫白天吃饱了，没有居住地");
                        homeManager.ForceCreateHome();
                    }

                }
                else
                {
                    FindAndEat();
                }
            }
            else
            {
                Debug.Log("成虫夜晚");
                if (homeManager.IsHaveHome)
                {
                    Debug.Log("成虫夜晚，有居住地，回家");
                    GoHomeAndSleep();
                }
                else
                {
                    Debug.Log("成虫夜晚，没有居住地，创建居住地");
                    homeManager.ForceCreateHome();
                    Debug.Log("成虫夜晚，新建居住地后，回家");
                    GoHomeAndSleep();
                }
            }
        }
        else
        {
            FindAndEat();
        }
    }

    private void StateCheck()
    {
        IsDayCheck();

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
            Debug.Log("白天开始了！触发事件");
        }

        // 更新上一次的状态
        wasDayLastFrame = isDay;

        // 调试输出
        // Debug.Log($"当前时间: {currentTime:F2}, 睡觉时间: {toSleepTime:F2}, 醒来时间: {toAwakeTime:F2}, 是否白天: {isDay}");
    }



    private void TakeAWalk()
    {
        Debug.Log("成虫白天吃饱了，有居住地，完成繁殖，去散步");
        
        // 如果蚂蚁在居住地内，先离开居住地
        if (isInHome)
        {
            homeManager.LeaveHome(this, (isNowInHome) => {
                this.isInHome = isNowInHome; // 这里回调会传入false
                Debug.Log($"蚂蚁已离开居住地，isInHome状态: {this.isInHome}");
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
            Debug.Log("停止散步状态，开始回家停留");
        }
        
        // 使用居住地管理器的回家方法
        homeManager.GoHomeAndStay(this, moveSpeed);
    }

    private void FindAndEat()
    {
        // 在寻找食物前，检查是否需要先离开居住地
        if (isInHome)
        {
            Debug.Log("需要寻找食物，但当前在居住地内，先离开。");
            homeManager.LeaveHome(this, (isNowInHome) => {
                this.isInHome = isNowInHome; // 回调会传入false
                Debug.Log($"为寻找食物而离开居住地，isInHome状态: {this.isInHome}");
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
        if (needsManager.isFull)
        {
            Debug.Log("吃饱了");
            return;
        }

        if (needsManager.isDrinkWater)
        {
            Debug.Log("喝过水了，去吃东西");
            needsManager.EatFood();
        }
        else
        {
            Debug.Log("没喝过水，去喝水");
            needsManager.DrinkWater();
        }
    }


    private void GoHomeAndSleep()
    {
        // 停止散步状态，避免与回家移动逻辑冲突
        if (walkManager != null)
        {
            walkManager.StopWalking();
            Debug.Log("停止散步状态，开始回家睡觉");
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
        Debug.Log("白天开始了，重置蚂蚁状态。");
        ResetStates();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "water")
        {
            needsManager.isTouchWaterTarget = true;
            Debug.Log("碰到水目标");
        }
        if (other.gameObject.name == "food")
        {
            needsManager.isTouchFoodTarget = true;
            Debug.Log("碰到食物目标");
        }
    }

    private void ResetStates()
    {
        // 重置需求状态
        needsManager.isFull = false;
        needsManager.ResetStates();
        
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
        
        Debug.Log("蚂蚁状态已重置，开始新的一天");
    }
}
