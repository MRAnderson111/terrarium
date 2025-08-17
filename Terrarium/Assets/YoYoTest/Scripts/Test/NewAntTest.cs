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
    
    // 居住地对象
    private GameObject homeObject;


    //居住地相关
    public GameObject homePrefab;
    public GameObject homeTarget;
    // 居住地组件引用
    private AntHomeTest homeScript;

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
        // 获取需求管理器组件
        needsManager = GetComponent<AntNeedsManager>();

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
                if (needsManager.isFull)
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
        
        // 检查是否有预制体
        if (homePrefab == null)
        {
            Debug.LogError("Home预制体未设置，请在Inspector中设置Home预制体");
            return;
        }
        
        // 在随机位置生成居住地
        Vector3 randomPosition = transform.position + new Vector3(
            UnityEngine.Random.Range(-10f, 10f),
            0f,
            UnityEngine.Random.Range(-10f, 10f)
        );
        
        // 生成居住地预制体
        homeObject = Instantiate(homePrefab, randomPosition, Quaternion.identity);
        homeTarget = homeObject;
        
        // 获取居住地脚本组件
        homeScript = homeObject.GetComponent<AntHomeTest>();
        if (homeScript == null)
        {
            Debug.LogError("居住地预制体上没有AntHomeTest组件");
            return;
        }
        
        isHaveHome = true;
        
        Debug.Log("居住地已创建在位置: " + randomPosition);
    }

    private void GoHomeAndStay()
    {
        if (homeTarget != null)
        {
            // 计算从当前位置到居住地的方向
            Vector3 direction = homeTarget.transform.position - transform.position;
            
            // 检查是否到达居住地（1米以内）
            float distance = direction.magnitude;
            if (distance <= 1f)
            {
                Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，已到达居住地");
                
                // 调用居住地的接口方法，通知蚂蚁已到达
                if (homeScript != null)
                {
                    homeScript.OnAntEntered(this, (isInHome) => {
                        this.isInHome = isInHome;
                        Debug.Log($"蚂蚁已进入居住地，isInHome状态: {this.isInHome}");
                    });
                }
                else
                {
                    Debug.LogError("居住地脚本组件未找到");
                }
            }
            else
            {
                // 归一化方向向量，确保移动速度一致
                direction.Normalize();
                
                // 向居住地方向移动
                transform.Translate(direction * 2f * Time.deltaTime);
                
                Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，正在回家，距离: " + distance);
            }
        }
        else
        {
            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，但居住地不存在");
        }
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
        if (homeTarget != null)
        {
            // 计算从当前位置到居住地的方向
            Vector3 direction = homeTarget.transform.position - transform.position;
            
            // 检查是否到达居住地（1米以内）
            float distance = direction.magnitude;
            if (distance <= 1f)
            {
                Debug.Log("成虫夜晚，有居住地，已到达居住地");
                
                // 调用居住地的接口方法，通知蚂蚁已到达
                if (homeScript != null)
                {
                    homeScript.OnAntEntered(this, (isInHome) => {
                        this.isInHome = isInHome;
                        Debug.Log($"蚂蚁已进入居住地，isInHome状态: {this.isInHome}");
                    });
                }
                else
                {
                    Debug.LogError("居住地脚本组件未找到");
                }
            }
            else
            {
                // 归一化方向向量，确保移动速度一致
                direction.Normalize();
                
                // 向居住地方向移动
                transform.Translate(direction * 2f * Time.deltaTime);
                
                Debug.Log("成虫夜晚，有居住地，正在回家睡觉，距离: " + distance);
            }
        }
        else
        {
            Debug.Log("成虫夜晚，有居住地，但居住地不存在");
        }
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
