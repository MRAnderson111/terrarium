using UnityEngine;

/// <summary>
/// 水源测试脚本 - 用于测试水源查找功能
/// </summary>
public class WaterSourceTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableDebug = true;
    
    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== 水源查找测试开始 ===");
            TestWaterSourceFinding();
        }
    }
    
    private void TestWaterSourceFinding()
    {
        // 模拟AnimalNeedsSystem的水源查找逻辑
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int waterCount = 0;
        
        Debug.Log("搜索场景中的水源对象...");
        
        foreach (GameObject obj in allObjects)
        {
            // 使用与AnimalNeedsSystem相同的逻辑
            if (obj.name.Contains("Water") || 
                obj.name.Contains("water") || 
                obj.name.Contains("Lake") || 
                obj.name.Contains("River") || 
                obj.name.Contains("Pond"))
            {
                waterCount++;
                Debug.Log($"发现水源: {obj.name}, 位置: {obj.transform.position}");
                
                // 检查是否有碰撞器
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    Debug.Log($"  - 碰撞器: {collider.GetType().Name}");
                }
                else
                {
                    Debug.LogWarning($"  - 警告: {obj.name} 没有碰撞器，动物可能无法与其交互");
                }
            }
        }
        
        Debug.Log($"总共发现 {waterCount} 个水源对象");
        
        if (waterCount == 0)
        {
            Debug.LogError("错误: 场景中没有发现任何水源！");
            Debug.LogError("请确保场景中有名称包含以下关键词的对象: Water, water, Lake, River, Pond");
            
            // 建议创建测试水源
            Debug.Log("建议: 使用右键菜单 -> 创建测试水源 来添加测试水源");
        }
        else
        {
            Debug.Log("水源查找测试完成，动物应该能够找到水源");
        }
    }
    
    [ContextMenu("创建测试水源")]
    public void CreateTestWaterSource()
    {
        // 创建一个简单的测试水源
        GameObject waterSource = GameObject.CreatePrimitive(PrimitiveType.Cube);
        waterSource.name = "TestWater";
        waterSource.transform.position = new Vector3(10, 0, 10);
        waterSource.transform.localScale = new Vector3(5, 0.5f, 5);
        
        // 设置为蓝色
        Renderer renderer = waterSource.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material waterMaterial = new Material(Shader.Find("Standard"));
            waterMaterial.color = Color.blue;
            renderer.material = waterMaterial;
        }
        
        Debug.Log($"创建了测试水源: {waterSource.name} 在位置 {waterSource.transform.position}");
    }
    
    [ContextMenu("创建多个测试水源")]
    public void CreateMultipleTestWaterSources()
    {
        Vector3[] positions = {
            new Vector3(10, 0, 10),
            new Vector3(-10, 0, 10),
            new Vector3(10, 0, -10),
            new Vector3(-10, 0, -10),
            new Vector3(0, 0, 15)
        };
        
        string[] names = {
            "TestWater_1",
            "TestLake",
            "TestRiver",
            "TestPond",
            "WaterSource"
        };
        
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject waterSource = GameObject.CreatePrimitive(PrimitiveType.Cube);
            waterSource.name = names[i];
            waterSource.transform.position = positions[i];
            waterSource.transform.localScale = new Vector3(3, 0.3f, 3);
            
            // 设置为蓝色
            Renderer renderer = waterSource.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material waterMaterial = new Material(Shader.Find("Standard"));
                waterMaterial.color = new Color(0, 0.5f, 1f, 0.8f); // 半透明蓝色
                renderer.material = waterMaterial;
            }
            
            Debug.Log($"创建了水源: {waterSource.name} 在位置 {waterSource.transform.position}");
        }
        
        Debug.Log("创建了5个测试水源");
    }
    
    [ContextMenu("测试动物水源查找")]
    public void TestAnimalWaterFinding()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        
        Debug.Log($"=== 测试 {animals.Length} 只动物的水源查找 ===");
        
        foreach (AnimalItem animal in animals)
        {
            if (animal != null)
            {
                AnimalNeedsSystem needs = animal.GetComponent<AnimalNeedsSystem>();
                if (needs != null)
                {
                    Debug.Log($"测试动物 {animal.name} 的水源查找...");
                    
                    try
                    {
                        needs.FindNearestWater();
                        
                        if (needs.TargetWater != null)
                        {
                            float distance = Vector3.Distance(animal.transform.position, needs.TargetWater.position);
                            Debug.Log($"  - 找到水源: {needs.TargetWater.name}, 距离: {distance:F2}");
                        }
                        else
                        {
                            Debug.LogWarning($"  - 警告: {animal.name} 没有找到水源目标");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"  - 错误: {animal.name} 水源查找失败: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"动物 {animal.name} 没有 AnimalNeedsSystem 组件");
                }
            }
        }
        
        Debug.Log("动物水源查找测试完成");
    }
    
    [ContextMenu("重新测试水源查找")]
    public void RetestWaterSourceFinding()
    {
        TestWaterSourceFinding();
    }
}
