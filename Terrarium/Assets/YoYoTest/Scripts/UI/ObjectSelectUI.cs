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

    





    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>(true);
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            if (canvas.transform.localScale == Vector3.zero)
                canvas.transform.localScale = Vector3.one;
            canvas.sortingOrder = Mathf.Max(canvas.sortingOrder, 10);
        }
    }

    void Start()
    {
        plantButtonGroup.SetActive(true);
        animalButtonGroup.SetActive(true);
        killerButtonGroup.SetActive(true);

        PrepareGroup(plantButtonGroup, new Vector2(-520f, 220f));
        PrepareGroup(animalButtonGroup, new Vector2(-280f, 220f));
        PrepareGroup(killerButtonGroup, new Vector2(-40f, 220f));

        LoadFromJson();
    }

    void PrepareGroup(GameObject group, Vector2 anchoredPosition)
    {
        if (group == null)
            return;

        RectTransform rect = group.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(220f, 460f);
        }

        GridLayoutGroup grid = group.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.cellSize = new Vector2(210f, 36f);
            grid.spacing = new Vector2(4f, 4f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
        }
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

        ClearGeneratedButtons(plantButtonGroup);
        ClearGeneratedButtons(animalButtonGroup);
        ClearGeneratedButtons(killerButtonGroup);

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
            if (selectButton.quantityText != null)
                selectButton.quantityText.text = quantity.ToString();
            if (selectButton.nameText != null)
                selectButton.nameText.text = prefab.name;
        }
        else
        {
            Debug.LogError("按钮预制体上没有SelectButton组件！");
        }
    }

    void ClearGeneratedButtons(GameObject group)
    {
        if (group == null)
            return;

        for (int i = group.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = group.transform.GetChild(i).gameObject;
            if (child.activeSelf)
                Destroy(child);
        }
    }

}
