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


    /// <summary>
    /// 存储全局冷却时间，键为SmallClass，值为冷却时间。
    /// </summary>
    public Dictionary<string, float> globalCoolDown = new();



    void Start()
    {
        // 注册对象生成和销毁事件的监听器
        Events.OnCreateObject.AddListener(OnCreateObject);
        Events.OnDestroyObject.AddListener(OnDestroyObject);
    }

    /// <summary>
    /// 每帧增加冷却时间
    /// </summary>
    void Update()
    {
        // 检查全局冷却时间字典是否为空，不为空才遍历增加时间
        if (globalCoolDown.Count > 0)
        {
            // 先获取所有键，避免在遍历过程中修改字典
            var keys = new List<string>(globalCoolDown.Keys);
            
            // 遍历键列表，增加冷却时间
            foreach (var key in keys)
            {
                // 增加冷却时间
                globalCoolDown[key] += Time.deltaTime;
            }
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            // 检查字典中是否包含 "Grass" 键
            if (globalCoolDown.ContainsKey("Grass"))
            {
                Debug.LogError("全局冷却时间：" + globalCoolDown["Grass"]);
            }
            else
            {
                Debug.LogError("Grass 不在全局冷却时间字典中");
            }
        }
    }

    /// <summary>
    /// 处理对象生成事件。当有新对象生成时，更新BigClass和SmallClass的统计数量。
    /// </summary>
    /// <param name="arg0">实现了 IGetObjectClass 接口的新生成对象。</param>
    private void OnCreateObject(IGetObjectClass arg0)
    {
        Debug.Log("生成物种：" + arg0.BigClass + " " + arg0.SmallClass);

        // // 检查这个小类是否已经在全局冷却时间中
        // if (globalCoolDown.ContainsKey(arg0.SmallClass))
        // {
        //     Debug.Log("小类 " + arg0.SmallClass + " 已在全局冷却时间中，跳过生成");
        //     return;
        // }

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

        // 将这个小类添加到全局冷却时间中
        globalCoolDown[arg0.SmallClass] = 0f;
        Debug.Log("小类 " + arg0.SmallClass + " 已添加到全局冷却时间中");

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
                
                // 如果小类数量小于等于0，重置全局冷却时间
                if (globalCoolDown.ContainsKey(arg0.SmallClass))
                {
                    globalCoolDown[arg0.SmallClass] = 0f;
                    Debug.Log("小类 " + arg0.SmallClass + " 数量已为0，重置全局冷却时间");
                }
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
