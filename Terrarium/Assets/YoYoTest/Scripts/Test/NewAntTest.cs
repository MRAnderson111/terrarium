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
    //是否"有居住地"
    public bool isHaveHome;
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

    //成虫睡觉时间
    public float toSleepTime = 20f; // 默认晚上8点睡觉
    //成虫醒来时间
    public float toAwakeTime = 6f;  // 默认早上6点醒来
    
    // 蚂蚁移动速度
    public float moveSpeed = 2f;
    
    // 散步相关变量
    private Vector3 walkTargetPosition; // 散步目标位置
    private bool isWalking = false; // 是否正在散步
    private float walkRadius = 20f; // 散步半径

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

        // 绑定事件监听器
        OnDayStart += CheckIfAntIsSleepingWhenDayStarts;
    }

    // Update is called once per frame
    void Update()
    {
        StateCheck();
        
        // 更新散步状态
        UpdateWalking();

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
        Debug.Log($"当前时间: {currentTime:F2}, 睡觉时间: {toSleepTime:F2}, 醒来时间: {toAwakeTime:F2}, 是否白天: {isDay}");
    }



    private void TakeAWalk()
    {
        Debug.Log("成虫白天吃饱了，有居住地，开始散步");
        
        // 如果蚂蚁在居住地内，先离开居住地
        if (isInHome)
        {
            homeManager.LeaveHome(this, (isInHome) => {
                this.isInHome = isInHome;
                Debug.Log($"蚂蚁已离开居住地，isInHome状态: {this.isInHome}");
                // 离开居住地后开始散步
                StartWalking();
            });
        }
        else
        {
            // 不在居住地内，直接开始散步
            StartWalking();
        }
    }
    
    /// <summary>
    /// 开始散步
    /// </summary>
    private void StartWalking()
    {
        // 设置散步状态
        isWalking = true;
        
        // 生成随机目标位置
        walkTargetPosition = GetRandomWalkPosition();
        
        Debug.Log($"蚂蚁开始散步，目标位置: {walkTargetPosition}");
    }
    
    /// <summary>
    /// 获取随机散步位置
    /// </summary>
    /// <returns>随机位置</returns>
    private Vector3 GetRandomWalkPosition()
    {
        // 在当前位置周围随机半径内生成目标位置
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
        randomDirection.y = 0; // 保持Y轴为0，确保在平面上移动
        
        return transform.position + randomDirection;
    }
    
    /// <summary>
    /// 更新散步状态
    /// </summary>
    private void UpdateWalking()
    {
        if (!isWalking)
            return;
            
        // 检查是否到达目标位置
        if (Vector3.Distance(transform.position, walkTargetPosition) < 0.5f)
        {
            // 到达目标位置，生成新的目标位置
            walkTargetPosition = GetRandomWalkPosition();
            Debug.Log($"蚂蚁到达目标位置，设置新目标: {walkTargetPosition}");
        }
        else
        {
            // 向目标位置移动
            Vector3 direction = (walkTargetPosition - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }


    private void GoHomeAndStay()
    {
        // 使用居住地管理器的回家方法
        homeManager.GoHomeAndStay(this, moveSpeed);
    }

    private void FindAndEat()
    {
        if (needsManager.isFull)
        {
            Debug.Log("吃饱了");
        }
        else
        {
            if (needsManager.isDrinkWater)
            {
                Debug.Log("喝过水了");
                needsManager.EatFood();
            }
            else
            {
                Debug.Log("没喝过水，去喝水");
                needsManager.DrinkWater();
            }
        }
    }


    private void GoHomeAndSleep()
    {
        // 使用居住地管理器的回家方法
        homeManager.GoHomeAndSleep(this, moveSpeed);
    }

    /// <summary>
    /// 当白天开始时检查蚂蚁是否在睡觉状态
    /// </summary>
    private void CheckIfAntIsSleepingWhenDayStarts()
    {
        if (isSleeping)
        {
            isSleeping = false;
            ResetStates();
        }
        else
        {
            // 
        }
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
        needsManager.isFull = false;
        isFinishReproduction = false;
        needsManager.ResetStates();
    }
}
