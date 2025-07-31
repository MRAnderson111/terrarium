using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象统计管理器：负责追踪游戏中各类对象的生成和销毁，并维护其数量统计。
/// </summary>
public class ObjectStatisticsManager : MonoBehaviour
{
    /// <summary>
    /// 存储大类对象的数量统计，键为BigClass，值为该大类对象的总数量。
    /// </summary>
    public Dictionary<string, int> bigClassCount = new();

    /// <summary>
    /// 存储小类对象的数量统计，键为SmallClass，值为该小类对象的数量。
    /// </summary>
    public Dictionary<string, int> smallClassCount = new();

    void Start()
    {
        // 注册对象生成和销毁事件的监听器
        Events.OnCreateObject.AddListener(OnCreateObject);
        Events.OnDestroyObject.AddListener(OnDestroyObject);
    }

    /// <summary>
    /// 处理对象生成事件。当有新对象生成时，更新BigClass和SmallClass的统计数量。
    /// </summary>
    /// <param name="arg0">实现了 IGetObjectClass 接口的新生成对象。</param>
    private void OnCreateObject(IGetObjectClass arg0)
    {
        Debug.Log("生成物种：" + arg0.BigClass + " " + arg0.SmallClass);

        // 更新BigClass数量统计
        if (bigClassCount.ContainsKey(arg0.BigClass))
        {
            bigClassCount[arg0.BigClass]++;
        }
        else
        {
            bigClassCount[arg0.BigClass] = 1;
        }

        // 更新SmallClass数量统计
        if (smallClassCount.ContainsKey(arg0.SmallClass))
        {
            smallClassCount[arg0.SmallClass]++;
        }
        else
        {
            smallClassCount[arg0.SmallClass] = 1;
        }

        UpdateStatistics(); // 更新并打印统计信息
    }

    /// <summary>
    /// 处理对象销毁事件。当对象被销毁时，更新BigClass和SmallClass的统计数量。
    /// </summary>
    /// <param name="arg0">实现了 IGetObjectClass 接口的被销毁对象。</param>
    private void OnDestroyObject(IGetObjectClass arg0)
    {
        Debug.Log("销毁物种：" + arg0.BigClass + " " + arg0.SmallClass);

        // 更新BigClass数量统计
        if (bigClassCount.ContainsKey(arg0.BigClass))
        {
            bigClassCount[arg0.BigClass]--;
            if (bigClassCount[arg0.BigClass] <= 0)
            {
                bigClassCount.Remove(arg0.BigClass);
            }
        }

        // 更新SmallClass数量统计
        if (smallClassCount.ContainsKey(arg0.SmallClass))
        {
            smallClassCount[arg0.SmallClass]--;
            if (smallClassCount[arg0.SmallClass] <= 0)
            {
                smallClassCount.Remove(arg0.SmallClass);
            }
        }

        UpdateStatistics(); // 更新并打印统计信息
    }

    /// <summary>
    /// 更新并打印当前所有物种的数量统计信息。
    /// </summary>
    private void UpdateStatistics()
    {
        Debug.Log("=== 大类统计 ===");
        foreach (var item in bigClassCount)
        {
            Debug.Log("大类：" + item.Key + " 总数量：" + item.Value);
        }

        Debug.Log("=== 小类统计 ===");
        foreach (var item in smallClassCount)
        {
            Debug.Log("小类：" + item.Key + " 数量：" + item.Value);
        }
    }
}
