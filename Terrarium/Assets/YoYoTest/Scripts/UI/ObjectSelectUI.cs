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
        
        // 使用简单的字符串解析处理JSON数据
        ParseJsonAndGenerateButtons(json);
    }
    
    /// <summary>
    /// 解析JSON并生成按钮
    /// </summary>
    /// <param name="json">JSON字符串</param>
    void ParseJsonAndGenerateButtons(string json)
    {
        try
        {
            // 提取Plant部分
            GenerateButtonsFromJsonSection(json, "Plant", plantButtonGroup);
            // 提取Animal部分
            GenerateButtonsFromJsonSection(json, "Animal", animalButtonGroup);
            // 提取Killer部分
            GenerateButtonsFromJsonSection(json, "Killer", killerButtonGroup);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON解析错误: {e.Message}");
        }
    }
    
    /// <summary>
    /// 从JSON的特定部分生成按钮
    /// </summary>
    /// <param name="json">完整JSON字符串</param>
    /// <param name="sectionName">部分名称（Plant/Animal/Killer）</param>
    /// <param name="parentGroup">父组对象</param>
    void GenerateButtonsFromJsonSection(string json, string sectionName, GameObject parentGroup)
    {
        if (parentGroup == null)
        {
            Debug.LogWarning($"父组为空，跳过{sectionName}按钮生成");
            return;
        }
        
        // 查找部分开始和结束位置
        string sectionStart = $"\"{sectionName}\": {{";
        int startIndex = json.IndexOf(sectionStart);
        if (startIndex == -1)
        {
            Debug.LogWarning($"在JSON中未找到{sectionName}部分");
            return;
        }
        
        startIndex += sectionStart.Length - 1; // 调整到花括号位置
        
        // 查找部分结束位置（下一个}或整个JSON结束）
        int endIndex = json.IndexOf('}', startIndex);
        if (endIndex == -1)
        {
            Debug.LogWarning($"在JSON中未找到{sectionName}部分的结束");
            return;
        }
        
        // 提取部分内容
        string sectionContent = json.Substring(startIndex + 1, endIndex - startIndex - 1);
        
        // 解析键值对
        string[] pairs = sectionContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (keyValue.Length >= 2)
            {
                string objectName = keyValue[0].Trim().Trim('"', ' ');
                string quantityStr = keyValue[1].Trim();
                
                if (int.TryParse(quantityStr, out int quantity))
                {
                    // 获取对应的预制体
                    GameObject prefab = GetPrefabByName(objectName);
                    if (prefab == null)
                    {
                        Debug.LogWarning($"未找到{objectName}对应的预制体");
                        continue;
                    }
                    
                    // 实例化按钮预制体
                    GameObject buttonObj = Instantiate(selectButtonPrefab, parentGroup.transform);
                    buttonObj.SetActive(true);
                    
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
                else
                {
                    Debug.LogWarning($"无法解析数量: {quantityStr}");
                }
            }
        }
    }
    
    /// <summary>
    /// 根据名称获取对应的预制体
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <returns>对应的预制体，如果找不到则返回null</returns>
    GameObject GetPrefabByName(string objectName)
    {
        string prefabPath = $"Prefabs/{objectName}";
        
        // 从Resources文件夹加载预制体
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"无法从路径 {prefabPath} 加载预制体");
        }
        
        return prefab;
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
