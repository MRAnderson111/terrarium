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
    //是否“有居住地”
    public bool isHaveHome;
    //是否“完成繁殖”
    public bool isFinishReproduction;

    //成虫睡觉时间
    public float toSleepTime = 20f; // 默认晚上8点睡觉
    //成虫醒来时间
    public float toAwakeTime = 6f;  // 默认早上6点醒来



    // Start is called before the first frame update
    void Start()
    {

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
            Debug.Log("没吃饱，找东西吃");
        }
    }

    private void GoHomeAndSleep()
    {
        // 成虫夜晚回家睡觉的具体方法实现
        // TODO: 实现回家睡觉的逻辑
        Debug.Log("成虫夜晚，有居住地，回家睡觉");
    }
}
