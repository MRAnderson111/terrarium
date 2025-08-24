using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建限制管理器类，用于管理各种对象类型的创建限制
/// 在每次进入游戏时自动重置数据
/// </summary>
public static class CreateLimitManager
{
    private static readonly Dictionary<string, int> createLimit = new Dictionary<string, int>();
    private static bool _isInitialized = false;

    
    
    /// <summary>
    /// 清理所有数据的方法
    /// </summary>
    public static void Cleanup()
    {
        createLimit.Clear();
        _isInitialized = false;
        
        Debug.Log("创建限制管理器已清理");
    }
    
    /// <summary>
    /// 初始化创建限制管理器
    /// </summary>
    public static void Initialize()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            Debug.Log("创建限制管理器已初始化");
        }
    }
    
    // createLimit字典的访问方法
    public static void SetCreateLimit(string key, int value)
    {
        createLimit[key] = value;
    }
    
    public static int GetCreateLimit(string key, int defaultValue = 0)
    {
        if (createLimit.TryGetValue(key, out int value))
        {
            return value;
        }
        return defaultValue;
    }
    
    public static bool HasCreateLimit(string key)
    {
        return createLimit.ContainsKey(key);
    }
    
    public static void RemoveCreateLimit(string key)
    {
        createLimit.Remove(key);
    }
    
    // 在Unity子系统注册时自动清理数据
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Cleanup();
    }
}
