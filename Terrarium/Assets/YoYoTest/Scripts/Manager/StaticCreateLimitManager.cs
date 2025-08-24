using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建限制管理器类，用于管理各种对象类型的创建限制
/// 在每次进入游戏时自动重置数据
/// </summary>
public static class StaticCreateLimitManager
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
        
        // 重新填充默认的创建限制
        createLimit.Add("BigAnt", 10);
        createLimit.Add("AntKiller", 5);
        createLimit.Add("Flower", 3);
        createLimit.Add("Tree", 20);
        
        Debug.Log("创建限制管理器已清理并重置默认值");
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

    public static void LogAllCreateLimit()
    {
        foreach (var item in createLimit)
        {
            Debug.Log("CreateLimit: " + item.Key + " " + item.Value);
        }
    }

    /// <summary>
    /// 给指定键的创建限制值增加指定数值
    /// </summary>
    /// <param name="key">要修改的键名</param>
    /// <param name="valueToAdd">要增加的数值</param>
    public static void AddToCreateLimit(string key, int valueToAdd)
    {
        if (createLimit.ContainsKey(key))
        {
            createLimit[key] += valueToAdd;
            Debug.Log($"键 '{key}' 的创建限制值已增加 {valueToAdd}，当前值为: {createLimit[key]}");
        }
        else
        {
            Debug.LogWarning($"键 '{key}' 不存在，无法增加数值");
        }
    }

    // 在Unity子系统注册时自动清理数据
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Cleanup();
    }
}
