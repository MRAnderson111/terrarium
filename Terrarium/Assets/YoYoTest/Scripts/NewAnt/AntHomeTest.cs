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

    //蚂蚁预制体
    public GameObject antPrefab;

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
            
            // 检查是否有两只未繁殖的蚂蚁，如果有则繁殖
            CheckAndReproduce();
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
    /// 检查是否有两只未繁殖的蚂蚁，如果有则繁殖
    /// </summary>
    private void CheckAndReproduce()
    {
        // 如果蚂蚁预制体未设置，则无法繁殖
        if (antPrefab == null)
        {
            Debug.LogError("蚂蚁预制体未设置，无法繁殖");
            return;
        }
        
        // 查找所有未繁殖的成虫蚂蚁
        List<NewAntTest> unReproducedAdultAnts = new List<NewAntTest>();
        foreach (NewAntTest ant in antsInHome)
        {
            // 只考虑成虫且未繁殖的蚂蚁
            if (ant.isAdult && !ant.isFinishReproduction)
            {
                unReproducedAdultAnts.Add(ant);
            }
        }
        
        // 循环处理所有可繁殖的蚂蚁对
        while (unReproducedAdultAnts.Count >= 2)
        {
            // 选择前两只蚂蚁进行繁殖
            NewAntTest parent1 = unReproducedAdultAnts[0];
            NewAntTest parent2 = unReproducedAdultAnts[1];
            
            // 检查两只蚂蚁的睡觉状态
            bool parent1IsSleeping = parent1.isSleeping;
            bool parent2IsSleeping = parent2.isSleeping;
            
            // 如果有任何一只蚂蚁处于睡觉状态，则不繁殖，但标记为已繁殖
            if (parent1IsSleeping || parent2IsSleeping)
            {
                // 将两只蚂蚁的状态设置为已繁殖，但不进行繁殖
                parent1.isFinishReproduction = true;
                parent2.isFinishReproduction = true;
                
                Debug.Log($"蚂蚁繁殖取消: {parent1.gameObject.name} (睡觉: {parent1IsSleeping}) 和 {parent2.gameObject.name} (睡觉: {parent2IsSleeping})，因为至少有一只蚂蚁在睡觉");
                
                // 从列表中移除已经繁殖过的蚂蚁
                unReproducedAdultAnts.Remove(parent1);
                unReproducedAdultAnts.Remove(parent2);
            }
            else
            {
                // 两只蚂蚁都不在睡觉状态，正常进行繁殖
                // 生成新的蚂蚁
                CreateNewAnt();
                
                // 将两只蚂蚁的状态设置为已繁殖
                parent1.isFinishReproduction = true;
                parent2.isFinishReproduction = true;
                
                // 从列表中移除已经繁殖过的蚂蚁
                unReproducedAdultAnts.Remove(parent1);
                unReproducedAdultAnts.Remove(parent2);
                
                Debug.Log($"蚂蚁繁殖成功: {parent1.gameObject.name} 和 {parent2.gameObject.name} 已繁殖");
            }
        }
    }
    
    /// <summary>
    /// 创建新的蚂蚁
    /// </summary>
    private void CreateNewAnt()
    {
        // 在居住地位置生成新的蚂蚁
        Vector3 spawnPosition = transform.position + new Vector3(
            UnityEngine.Random.Range(-1f, 1f),  // 在居住地周围随机位置生成
            0f,
            UnityEngine.Random.Range(-1f, 1f)
        );
        
        // 生成蚂蚁预制体
        GameObject newAntObject = Instantiate(antPrefab, spawnPosition, Quaternion.identity);
        newAntObject.name = "NewAnt_" + DateTime.Now.ToString("HHmmssfff");
        
        Debug.Log($"新蚂蚁已生成: {newAntObject.name}");
    }
    
    /// <summary>
    /// 获取居住地内的所有蚂蚁信息（用于调试）
    /// </summary>
    public void PrintAntsInHome()
    {
        Debug.Log($"居住地内共有 {antsInHome.Count} 只蚂蚁:");
        for (int i = 0; i < antsInHome.Count; i++)
        {
            Debug.Log($"  {i + 1}. 蚂蚁 {antsInHome[i].gameObject.name} - 成虫: {antsInHome[i].isAdult}, 已繁殖: {antsInHome[i].isFinishReproduction}");
        }
    }
}
