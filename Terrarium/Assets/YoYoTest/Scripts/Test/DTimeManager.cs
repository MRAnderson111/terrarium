using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTimeManager : MonoBehaviour
{
    #region 单例模式
    private static DTimeManager instance;
    
    /// <summary>
    /// 获取DTimeManager的单例实例
    /// </summary>
    public static DTimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 查找场景中已有的实例
                instance = FindObjectOfType<DTimeManager>();
                
                if (instance == null)
                {
                    // 如果场景中没有，创建新的GameObject并添加组件
                    GameObject go = new GameObject("DTimeManager");
                    instance = go.AddComponent<DTimeManager>();
                    DontDestroyOnLoad(go); // 确保切换场景时不被销毁
                }
            }
            return instance;
        }
    }
    
    /// <summary>
    /// 唤醒时检查单例
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // 如果已存在实例且不是当前实例，销毁当前实例
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject); // 确保切换场景时不被销毁
    }
    #endregion

    [Header("时间设置")]
    [Tooltip("游戏内一天相当于现实中的多少秒")]
    public float realSecondsPerGameDay = 120f; // 默认2分钟一天
    
    [Header("当前时间状态")]
    [SerializeField] private float currentTime; // 当前时间（0-24小时）
    [SerializeField] private int currentDay;    // 当前天数
    
    [Header("时间控制")]
    [SerializeField] private bool isTimeRunning = false;
    [SerializeField] private float timeSpeed = 1f; // 时间流逝速度倍率
    
    #region 公共属性
    /// <summary>
    /// 获取当前时间（小时）
    /// </summary>
    public float CurrentTime => currentTime;
    
    /// <summary>
    /// 获取当前天数
    /// </summary>
    public int CurrentDay => currentDay;
    
    /// <summary>
    /// 获取当前时间格式化字符串（HH:mm）
    /// </summary>
    public string FormattedTime => $"{(int)currentTime:D2}:{((int)((currentTime % 1) * 60)):D2}";
    
    /// <summary>
    /// 获取当前日期时间字符串（第X天 HH:mm）
    /// </summary>
    public string FormattedDateTime => $"第{currentDay}天 {FormattedTime}";
    
    /// <summary>
    /// 获取时间是否正在运行
    /// </summary>
    public bool IsTimeRunning => isTimeRunning;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // 初始化时间为第1天的0点
        ResetTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeRunning)
        {
            // 更新时间
            UpdateTime();
        }
    }

    #region 静态方法 - 通过单例调用
    /// <summary>
    /// 开始计时（静态方法）
    /// </summary>
    public static void StartTimer()
    {
        Instance.StartTime();
    }

    /// <summary>
    /// 暂停计时（静态方法）
    /// </summary>
    public static void PauseTimer()
    {
        Instance.PauseTime();
    }

    /// <summary>
    /// 重置时间到第1天的0点（静态方法）
    /// </summary>
    public static void ResetTimer()
    {
        Instance.ResetTime();
    }

    /// <summary>
    /// 设置时间流逝速度（静态方法）
    /// </summary>
    /// <param name="speed">速度倍率（1 = 正常速度）</param>
    public static void SetTimerSpeed(float speed)
    {
        Instance.SetTimeSpeed(speed);
    }

    /// <summary>
    /// 手动设置当前时间（静态方法）
    /// </summary>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    public static void SetTimerTime(int hour, int minute)
    {
        Instance.SetTime(hour, minute);
    }

    /// <summary>
    /// 手动设置当前天数（静态方法）
    /// </summary>
    /// <param name="day">天数（必须大于0）</param>
    public static void SetTimerDay(int day)
    {
        Instance.SetDay(day);
    }

    /// <summary>
    /// 获取当前时间的小时部分（静态方法）
    /// </summary>
    public static int GetTimerHours()
    {
        return Instance.GetHours();
    }

    /// <summary>
    /// 获取当前时间的分钟部分（静态方法）
    /// </summary>
    public static int GetTimerMinutes()
    {
        return Instance.GetMinutes();
    }

    /// <summary>
    /// 检查是否在指定的时间范围内（静态方法）
    /// </summary>
    /// <param name="startHour">开始小时</param>
    /// <param name="startMinute">开始分钟</param>
    /// <param name="endHour">结束小时</param>
    /// <param name="endMinute">结束分钟</param>
    /// <returns>如果在时间范围内返回true，否则返回false</returns>
    public static bool IsTimerInRange(int startHour, int startMinute, int endHour, int endMinute)
    {
        return Instance.IsTimeInRange(startHour, startMinute, endHour, endMinute);
    }
    #endregion

    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartTime()
    {
        isTimeRunning = true;
        Debug.Log($"时间管理器开始计时 - {FormattedDateTime}");
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    public void PauseTime()
    {
        isTimeRunning = false;
        Debug.Log($"时间管理器暂停计时 - {FormattedDateTime}");
    }

    /// <summary>
    /// 重置时间到第1天的0点
    /// </summary>
    public void ResetTime()
    {
        currentTime = 10f;
        currentDay = 1;
        isTimeRunning = false;
        Debug.Log($"时间已重置 - {FormattedDateTime}");
    }

    /// <summary>
    /// 设置时间流逝速度
    /// </summary>
    /// <param name="speed">速度倍率（1 = 正常速度）</param>
    public void SetTimeSpeed(float speed)
    {
        timeSpeed = Mathf.Max(0f, speed);
        Debug.Log($"时间流逝速度设置为: {timeSpeed}x");
    }

    /// <summary>
    /// 更新时间逻辑
    /// </summary>
    private void UpdateTime()
    {
        // 计算每帧增加的时间（基于现实时间流逝速度）
        float deltaTime = Time.deltaTime * timeSpeed;
        
        // 计算每帧增加的游戏内小时数
        float hoursPerSecond = 24f / realSecondsPerGameDay;
        float hoursToAdd = deltaTime * hoursPerSecond;
        
        // 更新当前时间
        currentTime += hoursToAdd;
        
        // 检查是否达到24小时（新的一天）
        if (currentTime >= 24f)
        {
            // 计算超出的小时数
            float excessHours = currentTime - 24f;
            
            // 增加天数
            currentDay++;
            
            // 重置时间为新的一天的时间
            currentTime = excessHours;
            
            // 触发新的一天事件
            OnNewDay();
        }
    }

    /// <summary>
    /// 新的一天到来时触发
    /// </summary>
    private void OnNewDay()
    {
        Debug.Log($"新的一天开始了！ - 第{currentDay}天");
        // 这里可以添加新一天的事件，比如重置某些状态、触发特定事件等
    }

    /// <summary>
    /// 手动设置当前时间
    /// </summary>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    public void SetTime(int hour, int minute)
    {
        if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
        {
            Debug.LogWarning("无效的时间设置！小时必须在0-23之间，分钟必须在0-59之间。");
            return;
        }
        
        currentTime = hour + (minute / 60f);
        Debug.Log($"时间已设置为: {FormattedTime}");
    }

    /// <summary>
    /// 手动设置当前天数
    /// </summary>
    /// <param name="day">天数（必须大于0）</param>
    public void SetDay(int day)
    {
        if (day <= 0)
        {
            Debug.LogWarning("天数必须大于0！");
            return;
        }
        
        currentDay = day;
        Debug.Log($"天数已设置为: 第{currentDay}天");
    }

    /// <summary>
    /// 获取当前时间的小时部分
    /// </summary>
    public int GetHours()
    {
        return (int)currentTime;
    }

    /// <summary>
    /// 获取当前时间的分钟部分
    /// </summary>
    public int GetMinutes()
    {
        return (int)((currentTime % 1) * 60);
    }

    /// <summary>
    /// 检查是否在指定的时间范围内
    /// </summary>
    /// <param name="startHour">开始小时</param>
    /// <param name="startMinute">开始分钟</param>
    /// <param name="endHour">结束小时</param>
    /// <param name="endMinute">结束分钟</param>
    /// <returns>如果在时间范围内返回true，否则返回false</returns>
    public bool IsTimeInRange(int startHour, int startMinute, int endHour, int endMinute)
    {
        float startTime = startHour + (startMinute / 60f);
        float endTime = endHour + (endMinute / 60f);
        
        if (startTime <= endTime)
        {
            // 正常的时间范围（比如8:00-18:00）
            return currentTime >= startTime && currentTime <= endTime;
        }
        else
        {
            // 跨天的时间范围（比如22:00-06:00）
            return currentTime >= startTime || currentTime <= endTime;
        }
    }
}
