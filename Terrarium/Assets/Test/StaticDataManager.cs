using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 静态管理器类，在每次进入游戏时自动重置数据
/// </summary>
public static class GlobalCooldownManager 
{
    // 示例数据字段 - 根据需要添加您的数据
    private static int score = 0;

    private static readonly Dictionary<string, float> coolDownTime = new Dictionary<string, float>();
    private static readonly Dictionary<string, float> coolDownTiming = new Dictionary<string, float>();
    private static bool _isInitialized = false;

    
    
    /// <summary>
    /// 清理所有数据的方法
    /// </summary>
    public static void Cleanup()
    {
        // 重置示例数据
        score = 0;
        // playerInventory.Clear();
        // gameData.Clear();
        // floatData1.Clear();
        // floatData2.Clear();
        coolDownTime.Clear();
        coolDownTiming.Clear();
        _isInitialized = false;
        
        // 在这里添加您需要清理的所有资源
        Debug.Log("静态数据管理器已清理");
    }
    
    /// <summary>
    /// 初始化静态数据管理器
    /// </summary>
    public static void Initialize()
    {
        if (!_isInitialized)
        {
            // 在这里初始化您的数据
            _isInitialized = true;
            Debug.Log("静态数据管理器已初始化");
        }
    }
    
    // 示例属性和方法 - 根据需要添加您的功能
    public static int Score
    {
        get { return score; }
        set { score = value; }
    }
    

    // coolDownTime字典的访问方法
    public static void SetCoolDownTime(string key, float value)
    {
        coolDownTime[key] = value;
    }
    
    public static float GetCoolDownTime(string key, float defaultValue = 0f)
    {
        if (coolDownTime.TryGetValue(key, out float value))
        {
            return value;
        }
        return defaultValue;
    }
    
    public static bool HasCoolDownTime(string key)
    {
        return coolDownTime.ContainsKey(key);
    }
    
    public static void RemoveCoolDownTime(string key)
    {
        coolDownTime.Remove(key);
    }

    // coolDownTiming字典的访问方法 
    public static void SetCoolDownTiming(string key, float value)
    {
        coolDownTiming[key] = value;
    }
    
    public static float GetCoolDownTiming(string key, float defaultValue = 0f)
    {
        if (coolDownTiming.TryGetValue(key, out float value))
        {
            return value;
        }
        return defaultValue;
    }
    
    public static bool HasCoolDownTiming(string key)
    {
        return coolDownTiming.ContainsKey(key);
    }
    
    public static void RemoveCoolDownTiming(string key)
    {
        coolDownTiming.Remove(key);
    }
    
    // 在Unity子系统注册时自动清理数据
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Cleanup();
    }
}
