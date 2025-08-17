using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAntTest : MonoBehaviour
{
    //是否是“成虫”
    public bool isAdult;
    //是否是“白天”
    public bool isDay;
    //是否“吃饱”
    public bool isFull;
    //是否“喝过水”
    public bool isDrinkWater;
    //当前饱腹感
    public float currentFullness;
    //是否有食物目标
    public bool isHaveFoodTarget;
    //是否“有居住地”
    public bool isHaveHome;
    //是否“完成繁殖”
    public bool isFinishReproduction;
    //是否在“睡觉”
    public bool isSleeping;

    //成虫睡觉时间
    public float toSleepTime = 20f; // 默认晚上8点睡觉
    //成虫醒来时间
    public float toAwakeTime = 6f;  // 默认早上6点醒来

    // 上一次的白天状态，用于检测状态变化
    private bool wasDayLastFrame = false;

    // 白天开始事件
    public event System.Action OnDayStart;



    // Start is called before the first frame update
    void Start()
    {
        // 绑定事件监听器
        OnDayStart += CheckIfAntIsSleepingWhenDayStarts;
    }

    // Update is called once per frame
    void Update()
    {
        StateCheck();


        if (isAdult)
        {
            if (isDay)
            {
                if (isFull)
                {
                    Debug.Log("成虫白天吃饱了");
                    if (isHaveHome)
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
                        CreateHome();
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
                if (isHaveHome)
                {
                    Debug.Log("成虫夜晚，有居住地，回家");
                    GoHomeAndSleep();
                }
                else
                {
                    Debug.Log("成虫夜晚，没有居住地，新建居住地");
                    CreateHome();
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
        Debug.Log("成虫白天吃饱了，有居住地，完成繁殖，去散步");
    }

    private void CreateHome()
    {
        Debug.Log("成虫白天吃饱了，没有居住地，新建居住地");
    }

    private void GoHomeAndStay()
    {
        Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，回家");
    }

    private void FindAndEat()
    {
        if (isFull)
        {
            Debug.Log("吃饱了");
        }
        else
        {
            if (isDrinkWater)
            {
                Debug.Log("喝过水了");
                if (isHaveFoodTarget)
                {
                    Debug.Log("有食物目标，去吃");
                    EatFood();
                }
                else
                {
                    Debug.Log("没有食物目标，去寻找");
                    FindFoodTarget();
                }
            }
            else
            {
                Debug.Log("没喝过水，去喝水");
                DrinkWater();
            }

        }
    }

    private void EatFood()
    {
        Debug.Log("吃食物");
        currentFullness += 10 * Time.deltaTime;
        if (currentFullness >= 100)
        {
            isFull = true;
        }
    }

    
    //水目标
    public GameObject waterTarget;
    //口渴度
    public float currentThirst;
    //是否碰到水目标
    public bool isTouchWaterTarget;
    private void DrinkWater()
    {
        if (waterTarget != null)
        {
            Debug.Log("有水目标，去喝水");
            if (isTouchWaterTarget)
            {
                Debug.Log("碰到水目标，喝水");
                currentThirst -= 10 * Time.deltaTime;
                if (currentThirst <= 0)
                {
                    isDrinkWater = true;
                }
            }
            else
            {
                Debug.Log("没碰到水目标，去喝水");
                GoToWaterTarget();
            }
        }
        else
        {
            Debug.Log("没有水目标，去寻找");
            FindWaterTarget();
        }
        
    }

    private void GoToWaterTarget()
    {
        if (waterTarget != null)
        {
            // 计算从当前位置到水目标的方向
            Vector3 direction = waterTarget.transform.position - transform.position;
            
            // 归一化方向向量，确保移动速度一致
            direction.Normalize();
            
            // 设置移动速度
            float moveSpeed = 2f;
            
            // 向水目标方向移动
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            
            Debug.Log("正在向水目标移动");
        }
        else
        {
            Debug.Log("水目标不存在");
        }
    }

    private void FindWaterTarget()
    {
        // 在场景中寻找名为 "water" 的对象
        GameObject waterObject = GameObject.Find("water");

        if (waterObject != null)
        {
            Debug.Log("找到水目标: " + waterObject.name);
            waterTarget = waterObject;
        }
        else
        {
            Debug.Log("未找到水目标");
            waterTarget = null;
        }
    }

    private void FindFoodTarget()
    {
        // 在场景中寻找名为 "food" 的对象
        GameObject foodObject = GameObject.Find("food");

        if (foodObject != null)
        {
            Debug.Log("找到食物目标: " + foodObject.name);
            isHaveFoodTarget = true;
        }
        else
        {
            Debug.Log("未找到食物目标");
            isHaveFoodTarget = false;
        }
    }

    private void GoHomeAndSleep()
    {
        // 成虫夜晚回家睡觉的具体方法实现
        // TODO: 实现回家睡觉的逻辑
        Debug.Log("成虫夜晚，有居住地，回家睡觉");
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

    private void ResetStates()
    {
        isFull = false;
        isFinishReproduction = false;
    }
}
