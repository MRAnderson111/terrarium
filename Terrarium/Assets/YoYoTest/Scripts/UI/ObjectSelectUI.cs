using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ObjectSelectUI : MonoBehaviour
{
    public  GameObject plantButtonGroup;
    public  GameObject animalButtonGroup;
    public  GameObject killerButtonGroup;
    public  GameObject selectButtonPrefab;
    public TextAsset jsonFile;

    





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


        //tset
        LoadFromJson();
    }

    void Update()
    {

    }

    void LoadFromJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON文件未分配！");
            return;
        }

        // 解析JSON数据
        string json = jsonFile.text;
        
        // 配置按钮生成参数
        var buttonConfigs = new UIJsonButtonGenerator.ButtonSectionConfig[]
        {
            new UIJsonButtonGenerator.ButtonSectionConfig { SectionName = "Plant", ParentGroup = plantButtonGroup },
            new UIJsonButtonGenerator.ButtonSectionConfig { SectionName = "Animal", ParentGroup = animalButtonGroup },
            new UIJsonButtonGenerator.ButtonSectionConfig { SectionName = "Killer", ParentGroup = killerButtonGroup }
        };
        
        // 使用工具类生成按钮
        UIJsonButtonGenerator.ParseJsonAndGenerateButtons(
            json,
            buttonConfigs,
            selectButtonPrefab,
            SetupButton
        );
    }
    
    /// <summary>
    /// 设置按钮属性的回调函数
    /// </summary>
    /// <param name="buttonObj">按钮对象</param>
    /// <param name="objectName">对象名称</param>
    /// <param name="quantity">数量</param>
    void SetupButton(GameObject buttonObj, string objectName, int quantity)
    {
        // 获取对应的预制体
        GameObject prefab = ResourceLoadUtils.LoadPrefabByName(objectName);
        if (prefab == null)
        {
            Debug.LogWarning($"未找到{objectName}对应的预制体");
            return;
        }
        
        // 获取SelectButton组件
        SelectButton selectButton = buttonObj.GetComponent<SelectButton>();
        if (selectButton != null)
        {
            // 设置按钮的预制体引用和数量
            selectButton.selectPrefab = prefab;
            selectButton.quantity = quantity;
            selectButton.quantityText.text = quantity.ToString();
        }
        else
        {
            Debug.LogError("按钮预制体上没有SelectButton组件！");
        }
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
