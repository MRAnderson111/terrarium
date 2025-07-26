using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Plant : MonoBehaviour
{
    [Header("植物库存UI组件")]
    [SerializeField] private TextMeshProUGUI plantCountText;
    [SerializeField] private TextMeshProUGUI environmentalFoodText;
    [SerializeField] private TextMeshProUGUI animalCountText;
    [SerializeField] private TextMeshProUGUI environmentalAnimalText;
    [SerializeField] private GameObject plantInventoryPanel;

    [Header("UI设置")]
    [SerializeField] private float updateInterval = 0.5f; // 更新间隔（秒）

    private float updateTimer = 0f;

    void Start()
    {
        // 确保在UI初始化时重置植物数据（如果没有实际植物对象）
        ResetPlantDataIfNeeded();

        // 确保动物计数准确
        ResetAnimalDataIfNeeded();

        Debug.Log($"UI_Plant Start() - 初始植物数量: {GetTotalPlantCount()}, 环境食物: {GetEnvironmentalFood()}");
        Debug.Log($"UI_Plant Start() - 初始动物数量: {GetTotalAnimalCount()}, 环境动物数量: {GetEnvironmentalAnimalValue()}");
        CreatePlantInventoryUI();
        UpdatePlantInventoryDisplay();
    }

    void CreatePlantInventoryUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // 创建植物数量面板
        CreateRoundedPanel("PlantCountPanel", "植物数量: 0", new Vector2(10f, 100f), new Vector2(120f, 30f), ref plantCountText);
        
        // 创建环境食物面板
        CreateRoundedPanel("EnvironmentalFoodPanel", "环境食物: 0", new Vector2(10f, 60f), new Vector2(120f, 30f), ref environmentalFoodText);
        
        // 创建动物数量面板
        CreateRoundedPanel("AnimalCountPanel", "动物数量: 0", new Vector2(10f, 20f), new Vector2(120f, 30f), ref animalCountText);
        
        // 创建环境动物数量面板
        CreateRoundedPanel("EnvironmentalAnimalPanel", "环境动物数量: 0", new Vector2(140f, 20f), new Vector2(140f, 30f), ref environmentalAnimalText);
    }

    void CreateRoundedPanel(string panelName, string text, Vector2 position, Vector2 size, ref TextMeshProUGUI textComponent)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        // 创建面板
        GameObject panel = new(panelName);
        panel.transform.SetParent(canvas.transform, false);

        // 添加圆角背景
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        panelImage.type = Image.Type.Sliced;
        
        // 设置面板位置和大小
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 0f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = position;
        panelRect.sizeDelta = size;

        // 创建文本
        GameObject textObj = new(panelName + "Text");
        textObj.transform.SetParent(panel.transform, false);

        textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5f, 2f);
        textRect.offsetMax = new Vector2(-5f, -2f);
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            UpdatePlantInventoryDisplay();
            updateTimer = 0f;
        }
    }

    void UpdatePlantInventoryDisplay()
    {
        // 获取植物数据
        int plantCount = GetTotalPlantCount();
        int environmentalFood = GetEnvironmentalFood();
        
        // 获取动物数据
        int animalCount = GetTotalAnimalCount();
        int environmentalAnimalValue = GetEnvironmentalAnimalValue();

        Debug.Log($"UI更新 - 植物数量: {plantCount}, 环境食物: {environmentalFood}, 动物数量: {animalCount}, 环境动物数量: {environmentalAnimalValue}");

        if (plantCountText != null)
        {
            plantCountText.text = $"植物数量: {plantCount}";
        }

        if (environmentalFoodText != null)
        {
            environmentalFoodText.text = $"环境食物: {environmentalFood}";
        }
        
        if (animalCountText != null)
        {
            animalCountText.text = $"动物数量: {animalCount}";
        }

        if (environmentalAnimalText != null)
        {
            environmentalAnimalText.text = $"环境动物数量: {environmentalAnimalValue}";
        }
    }

    int GetTotalPlantCount()
    {
        // 尝试通过反射调用PlantItem的静态方法
        try
        {
            var plantItemType = System.Type.GetType("PlantItem");
            if (plantItemType != null)
            {
                var method = plantItemType.GetMethod("GetTotalPlantCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    return (int)method.Invoke(null, null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取植物数量失败: {e.Message}");
        }

        // 备用方案：通过查找GameObject计算
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("PlantItem"))
            {
                count++;
            }
        }
        return count;
    }

    int GetEnvironmentalFood()
    {
        // 尝试通过反射调用PlantItem的静态方法
        try
        {
            var plantItemType = System.Type.GetType("PlantItem");
            if (plantItemType != null)
            {
                var method = plantItemType.GetMethod("GetEnvironmentalFood", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    return (int)method.Invoke(null, null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取环境食物数值失败: {e.Message}");
        }

        // 备用方案：根据植物数量计算（每个植物贡献2点环境食物）
        return GetTotalPlantCount() * 2;
    }

    void ResetPlantDataIfNeeded()
    {
        // 通过反射检查场景中是否真的有PlantItem对象
        bool hasPlantItems = false;
        try
        {
            var plantItemType = System.Type.GetType("PlantItem");
            if (plantItemType != null)
            {
                var existingPlants = FindObjectsOfType(plantItemType);
                hasPlantItems = existingPlants.Length > 0;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"检查植物对象失败: {e.Message}");
        }

        if (!hasPlantItems)
        {
            // 如果没有植物对象，通过反射重置静态变量
            try
            {
                var plantItemType = System.Type.GetType("PlantItem");
                if (plantItemType != null)
                {
                    var totalCountField = plantItemType.GetField("totalPlantCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var environmentalFoodField = plantItemType.GetField("environmentalFood", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    
                    totalCountField?.SetValue(null, 0);
                    environmentalFoodField?.SetValue(null, 0);

                    Debug.Log("UI重置了植物静态数据");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"重置植物数据失败: {e.Message}");
            }
        }
    }

    int GetTotalAnimalCount()
    {
        // 通过反射调用AnimalLifecycleSystem的静态方法
        try
        {
            var animalLifecycleType = System.Type.GetType("AnimalLifecycleSystem");
            if (animalLifecycleType != null)
            {
                var method = animalLifecycleType.GetMethod("GetTotalAnimalCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    return (int)method.Invoke(null, null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取动物数量失败: {e.Message}");
        }

        // 备用方案：通过查找AnimalItem组件计算
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        return animals.Length;
    }

    int GetEnvironmentalAnimalValue()
    {
        // 通过反射调用AnimalLifecycleSystem的静态方法
        try
        {
            var animalLifecycleType = System.Type.GetType("AnimalLifecycleSystem");
            if (animalLifecycleType != null)
            {
                var method = animalLifecycleType.GetMethod("GetEnvironmentalAnimalValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    return (int)method.Invoke(null, null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取环境动物数量失败: {e.Message}");
        }

        // 备用方案：根据动物数量计算（每个动物贡献2点环境动物数值）
        return GetTotalAnimalCount() * 2;
    }

    void ResetAnimalDataIfNeeded()
    {
        // 获取场景中实际的动物数量
        AnimalItem[] actualAnimals = FindObjectsOfType<AnimalItem>();
        int actualAnimalCount = actualAnimals.Length;

        // 获取静态变量中的动物数量
        int staticAnimalCount = 0;
        try
        {
            var animalLifecycleType = System.Type.GetType("AnimalLifecycleSystem");
            if (animalLifecycleType != null)
            {
                var totalCountField = animalLifecycleType.GetField("totalAnimalCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (totalCountField != null)
                {
                    staticAnimalCount = (int)totalCountField.GetValue(null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取静态动物数量失败: {e.Message}");
        }

        Debug.Log($"动物数量检查 - 场景中实际: {actualAnimalCount}, 静态变量: {staticAnimalCount}");

        // 如果数量不匹配，重置静态变量
        if (actualAnimalCount != staticAnimalCount)
        {
            try
            {
                var animalLifecycleType = System.Type.GetType("AnimalLifecycleSystem");
                if (animalLifecycleType != null)
                {
                    var totalCountField = animalLifecycleType.GetField("totalAnimalCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var environmentalAnimalField = animalLifecycleType.GetField("environmentalAnimalValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                    if (totalCountField != null)
                    {
                        totalCountField.SetValue(null, actualAnimalCount);
                    }
                    if (environmentalAnimalField != null)
                    {
                        environmentalAnimalField.SetValue(null, actualAnimalCount * 2);
                    }

                    Debug.Log($"UI重置了动物静态数据 - 动物数量: {actualAnimalCount}, 环境动物数量: {actualAnimalCount * 2}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"重置动物数据失败: {e.Message}");
            }
        }
    }
}
