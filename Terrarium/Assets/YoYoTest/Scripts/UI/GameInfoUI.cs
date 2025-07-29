using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUI : MonoBehaviour
{
    public Text timeText;
    public Text stageText;
    public Text foodQuantityText;
    





    private StageManager stageManager; // 缓存单例引用
    private float updateInterval = 0.1f; // 更新间隔（秒）
    private float lastUpdateTime;

    void Start()
    {
        stageManager = StageManager.Instance; // 缓存单例
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        // 按间隔更新，减少性能消耗
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            timeText.text = $"剩余时间: {stageManager.RemainingTime:F2}";
            stageText.text = $"阶段: {stageManager.stage}";
            foodQuantityText.text = $"食物数量: {stageManager.foodQuantity}";
            lastUpdateTime = Time.time;
        }
    }
}
