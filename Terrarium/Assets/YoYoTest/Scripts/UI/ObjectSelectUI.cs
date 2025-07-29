using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectUI : MonoBehaviour
{
    public  GameObject plantButtonGroup;
    public  GameObject animalButtonGroup;
    public  GameObject killerButtonGroup;

    





    private StageManager stageManager; // 缓存单例引用
    private float updateInterval = 0.1f; // 更新间隔（秒）
    private float lastUpdateTime;

    void Start()
    {
        stageManager = StageManager.Instance; // 缓存单例
        lastUpdateTime = Time.time;

        // 监听阶段事件
        Events.OnGameEnterStage.AddListener(OnStageChanged);

        // 初始化时所有按钮组都设为不激活
        plantButtonGroup.SetActive(false);
        animalButtonGroup.SetActive(false);
        killerButtonGroup.SetActive(false);
    }

    void Update()
    {

    }

    /// <summary>
    /// 处理阶段变化事件
    /// </summary>
    /// <param name="stageNumber">阶段编号：1-植物阶段，2-动物阶段，3-杀手阶段</param>
    private void OnStageChanged(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1: // 第一阶段 - 解锁植物按钮组
                plantButtonGroup.SetActive(true);
                break;
            case 2: // 第二阶段 - 解锁动物按钮组
                animalButtonGroup.SetActive(true);
                break;
            case 3: // 第三阶段 - 解锁杀手按钮组
                killerButtonGroup.SetActive(true);
                break;
            default:
                Debug.LogWarning($"未知的阶段编号: {stageNumber}");
                break;
        }
    }

    void OnDestroy()
    {
        // 清理事件监听，防止内存泄漏
        if (Events.OnGameEnterStage != null)
        {
            Events.OnGameEnterStage.RemoveListener(OnStageChanged);
        }
    }
}
