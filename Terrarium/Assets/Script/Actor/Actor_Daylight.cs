using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Daylight : MonoBehaviour
{
    [Header("日夜循环设置")]
    [SerializeField] private float dayDuration = 20f;      // 白天持续时间（秒）
    [SerializeField] private float nightDuration = 10f;    // 夜晚持续时间（秒）
    [SerializeField] private float dayIntensity = 2f;      // 白天光照强度
    [SerializeField] private float nightIntensity = 0.5f;  // 夜晚光照强度
    [SerializeField] private float transitionSpeed = 2f;   // 日夜过渡速度

    // 日夜状态
    public enum DayNightState
    {
        Day,            // 白天
        TransitionToNight, // 过渡到夜晚
        Night,          // 夜晚
        TransitionToDay    // 过渡到白天
    }

    [Header("当前状态")]
    [SerializeField] private DayNightState currentState = DayNightState.Day;

    // 内部变量
    private Light daylightSource;
    private float stateTimer = 0f;
    private float targetIntensity;
    private float currentIntensity;

    // 公共属性，供其他脚本访问
    public static bool IsNight { get; private set; } = false;
    public static float CurrentLightIntensity { get; private set; } = 1f;

    void Start()
    {
        InitializeDaylight();
    }

    void InitializeDaylight()
    {
        // 查找场景中名为"Daylight"的光源
        FindDaylightSource();

        // 初始化状态
        currentState = DayNightState.Day;
        stateTimer = 0f;
        targetIntensity = dayIntensity;
        currentIntensity = dayIntensity;
        IsNight = false;

        // 设置初始光照
        if (daylightSource != null)
        {
            daylightSource.intensity = currentIntensity;
            CurrentLightIntensity = currentIntensity;
        }

        Debug.Log($"日夜循环系统初始化完成 - 白天: {dayDuration}秒, 夜晚: {nightDuration}秒");
    }

    void FindDaylightSource()
    {
        // 查找场景中名为"Daylight"的GameObject上的Light组件
        GameObject daylightObject = GameObject.Find("Daylight");
        if (daylightObject != null)
        {
            daylightSource = daylightObject.GetComponent<Light>();
            if (daylightSource != null)
            {
                Debug.Log("找到Daylight光源，已建立连接");
            }
            else
            {
                Debug.LogWarning("找到名为'Daylight'的对象，但没有Light组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到名为'Daylight'的光源对象");
        }
    }

    void Update()
    {
        UpdateDayNightCycle();
        UpdateLightIntensity();
    }

    void UpdateDayNightCycle()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case DayNightState.Day:
                if (stateTimer >= dayDuration)
                {
                    StartTransitionToNight();
                }
                break;

            case DayNightState.TransitionToNight:
                if (Mathf.Approximately(currentIntensity, nightIntensity))
                {
                    StartNight();
                }
                break;

            case DayNightState.Night:
                if (stateTimer >= nightDuration)
                {
                    StartTransitionToDay();
                }
                break;

            case DayNightState.TransitionToDay:
                if (Mathf.Approximately(currentIntensity, dayIntensity))
                {
                    StartDay();
                }
                break;
        }
    }

    void UpdateLightIntensity()
    {
        // 平滑过渡光照强度
        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, transitionSpeed * Time.deltaTime);

        // 更新光源强度
        if (daylightSource != null)
        {
            daylightSource.intensity = currentIntensity;
        }

        // 更新静态属性
        CurrentLightIntensity = currentIntensity;
    }

    void StartDay()
    {
        currentState = DayNightState.Day;
        stateTimer = 0f;
        targetIntensity = dayIntensity;
        IsNight = false;

        Debug.Log("白天开始 - 光照强度目标: " + dayIntensity);
    }

    void StartTransitionToNight()
    {
        currentState = DayNightState.TransitionToNight;
        targetIntensity = nightIntensity;

        Debug.Log("开始过渡到夜晚 - 光照强度目标: " + nightIntensity);
    }

    void StartNight()
    {
        currentState = DayNightState.Night;
        stateTimer = 0f;
        targetIntensity = nightIntensity;
        IsNight = true;

        Debug.Log("夜晚开始 - 光照强度: " + nightIntensity);
    }

    void StartTransitionToDay()
    {
        currentState = DayNightState.TransitionToDay;
        targetIntensity = dayIntensity;

        Debug.Log("开始过渡到白天 - 光照强度目标: " + dayIntensity);
    }
}
