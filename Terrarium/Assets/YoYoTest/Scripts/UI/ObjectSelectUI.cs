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

    





    void Start()
    {
        // 初始化时所有按钮组都设为激活状态
        plantButtonGroup.SetActive(true);
        animalButtonGroup.SetActive(true);
        killerButtonGroup.SetActive(true);

        //tset
        // LoadFromJson();
    }

    void Update()
    {

    }

    public void LoadFromJson(TextAsset externalJsonFile = null)
    {
        // 如果传入了外部JSON文件，则使用外部文件，否则使用内部的jsonFile
        TextAsset jsonAsset = externalJsonFile != null ? externalJsonFile : jsonFile;
        
        if (jsonAsset == null)
        {
            Debug.LogError("JSON文件未分配！");
            return;
        }

        // 解析JSON数据
        string json = jsonAsset.text;
        
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
            
            // 设置按钮名称为预制体名称
            if (selectButton.nameText != null)
            {
                selectButton.nameText.text = prefab.name;
            }
            else
            {
                Debug.LogWarning("按钮预制体上没有nameText组件！");
            }
        }
        else
        {
            Debug.LogError("按钮预制体上没有SelectButton组件！");
        }
    }

}
