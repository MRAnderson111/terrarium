using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AntHomeTest : MonoBehaviour
{
    // 居住地内的蚂蚁列表
    private List<NewAntTest> antsInHome = new List<NewAntTest>();
    
    // 属性：当前居住地内的蚂蚁数量
    public int AntsCount => antsInHome.Count;
    
    // 属性：获取居住地内的所有蚂蚁
    public List<NewAntTest> AntsInHome => antsInHome;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// 蚂蚁到达居住地时调用的接口方法
    /// </summary>
    /// <param name="ant">到达的蚂蚁实例</param>
    /// <param name="onAntEntered">回调action，用于设置蚂蚁的布尔属性</param>
    public void OnAntEntered(NewAntTest ant, Action<bool> onAntEntered)
    {
        if (ant == null)
        {
            Debug.LogError("传入的蚂蚁实例为空");
            return;
        }
        
        // 检查蚂蚁是否已经在列表中
        if (!antsInHome.Contains(ant))
        {
            // 将蚂蚁添加到居住地的蚂蚁列表中
            antsInHome.Add(ant);
            Debug.Log($"蚂蚁已进入居住地，当前居住地内蚂蚁数量: {antsInHome.Count}");
            
            // 回调蚂蚁的action，设置蚂蚁的布尔属性为true（表示已在居住地内）
            onAntEntered?.Invoke(true);
        }
        else
        {
            Debug.LogWarning("该蚂蚁已经在居住地列表中");
        }
    }
    
    /// <summary>
    /// 蚂蚁离开居住地时调用的接口方法
    /// </summary>
    /// <param name="ant">离开的蚂蚁实例</param>
    /// <param name="onAntLeft">回调action，用于设置蚂蚁的布尔属性</param>
    public void OnAntLeft(NewAntTest ant, Action<bool> onAntLeft)
    {
        if (ant == null)
        {
            Debug.LogError("传入的蚂蚁实例为空");
            return;
        }
        
        // 从列表中移除蚂蚁
        if (antsInHome.Remove(ant))
        {
            Debug.Log($"蚂蚁已离开居住地，当前居住地内蚂蚁数量: {antsInHome.Count}");
            
            // 回调蚂蚁的action，设置蚂蚁的布尔属性为false（表示已离开居住地）
            onAntLeft?.Invoke(false);
        }
        else
        {
            Debug.LogWarning("该蚂蚁不在居住地列表中");
        }
    }
    
    /// <summary>
    /// 获取居住地内的所有蚂蚁信息（用于调试）
    /// </summary>
    public void PrintAntsInHome()
    {
        Debug.Log($"居住地内共有 {antsInHome.Count} 只蚂蚁:");
        for (int i = 0; i < antsInHome.Count; i++)
        {
            Debug.Log($"  {i + 1}. 蚂蚁 {antsInHome[i].gameObject.name}");
        }
    }
}
