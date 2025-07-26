using UnityEngine;

/// <summary>
/// 无标签测试脚本 - 验证动物系统不再依赖Unity标签
/// </summary>
public class TaglessTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableDebug = true;
    
    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== 无标签依赖测试开始 ===");
            TestTaglessDetection();
        }
    }
    
    private void TestTaglessDetection()
    {
        Debug.Log("测试动物检测（不使用标签）...");
        
        // 创建测试对象
        CreateTestObjects();
        
        // 测试动物检测
        TestAnimalDetection();
        
        // 测试地面检测
        TestGroundDetection();
        
        // 测试水源检测
        TestWaterDetection();
        
        Debug.Log("无标签依赖测试完成");
    }
    
    private void CreateTestObjects()
    {
        // 创建测试动物
        GameObject testAnimal = new("TestAnimalItem");
        testAnimal.AddComponent<AnimalItem>();
        testAnimal.transform.position = new Vector3(0, 5, 0);
        
        // 创建测试地面
        GameObject testGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testGround.name = "TestGround";
        testGround.transform.position = new Vector3(0, -1, 0);
        testGround.transform.localScale = new Vector3(10, 1, 10);
        
        // 创建测试水源
        GameObject testWater = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testWater.name = "TestWater";
        testWater.transform.position = new Vector3(5, 0, 5);
        testWater.transform.localScale = new Vector3(3, 0.5f, 3);
        
        // 设置颜色
        Renderer waterRenderer = testWater.GetComponent<Renderer>();
        if (waterRenderer != null)
        {
            Material waterMaterial = new(Shader.Find("Standard"));
            waterMaterial.color = Color.blue;
            waterRenderer.material = waterMaterial;
        }
        
        Debug.Log("创建了测试对象：动物、地面、水源");
    }
    
    private void TestAnimalDetection()
    {
        Debug.Log("--- 测试动物检测 ---");
        
        // 获取AnimalItem实例来测试IsAnimalObject方法
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        
        if (animals.Length > 0)
        {
            AnimalItem testAnimal = animals[0];
            
            // 测试各种对象
            GameObject[] testObjects = {
                testAnimal.gameObject,
                GameObject.Find("TestGround"),
                GameObject.Find("TestWater")
            };
            
            foreach (GameObject obj in testObjects)
            {
                if (obj != null)
                {
                    // 使用反射调用私有方法进行测试
                    bool isAnimal = HasAnimalComponent(obj);
                    Debug.Log($"对象 '{obj.name}' 是否为动物: {isAnimal}");
                }
            }
        }
        else
        {
            Debug.LogWarning("没有找到动物对象进行测试");
        }
    }
    
    private void TestGroundDetection()
    {
        Debug.Log("--- 测试地面检测 ---");
        
        GameObject[] testObjects = {
            GameObject.Find("TestGround"),
            GameObject.Find("TestWater"),
            new GameObject("RandomObject")
        };
        
        foreach (GameObject obj in testObjects)
        {
            if (obj != null)
            {
                bool isGround = IsGroundObject(obj);
                Debug.Log($"对象 '{obj.name}' 是否为地面: {isGround}");
            }
        }
        
        // 清理临时对象
        GameObject randomObj = GameObject.Find("RandomObject");
        if (randomObj != null) Destroy(randomObj);
    }
    
    private void TestWaterDetection()
    {
        Debug.Log("--- 测试水源检测 ---");
        
        GameObject[] testObjects = {
            GameObject.Find("TestWater"),
            GameObject.Find("TestGround"),
            new GameObject("TestLake"),
            new GameObject("TestRiver")
        };
        
        foreach (GameObject obj in testObjects)
        {
            if (obj != null)
            {
                bool isWater = IsWaterObject(obj);
                Debug.Log($"对象 '{obj.name}' 是否为水源: {isWater}");
            }
        }
        
        // 清理临时对象
        GameObject lake = GameObject.Find("TestLake");
        GameObject river = GameObject.Find("TestRiver");
        if (lake != null) Destroy(lake);
        if (river != null) Destroy(river);
    }
    
    // 模拟AnimalItem中的检测逻辑
    private bool HasAnimalComponent(GameObject obj)
    {
        // 检查对象是否有AnimalItem组件
        if (obj.GetComponent<AnimalItem>() != null)
        {
            return true;
        }

        // 检查对象名称是否包含Animal相关字符串
        if (obj.name.Contains("Animal") || obj.name.Contains("AnimalItem") || obj.name.Contains("Offspring"))
        {
            return true;
        }

        return false;
    }
    
    private bool IsGroundObject(GameObject obj)
    {
        return obj.name.Contains("Ground") ||
               obj.name.Contains("Plane") ||
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround") ||
               obj.name.Contains("Terrain") ||
               obj.name.Contains("Floor") ||
               obj.name.Contains("Surface");
    }
    
    private bool IsWaterObject(GameObject obj)
    {
        return obj.name.Contains("Water") || 
               obj.name.Contains("water") || 
               obj.name.Contains("Lake") || 
               obj.name.Contains("River") || 
               obj.name.Contains("Pond");
    }
    
    [ContextMenu("运行标签测试")]
    public void RunTagTest()
    {
        TestTaglessDetection();
    }
    
    [ContextMenu("清理测试对象")]
    public void CleanupTestObjects()
    {
        // 清理所有测试对象
        string[] testNames = { "TestAnimalItem", "TestGround", "TestWater", "TestLake", "TestRiver" };
        
        foreach (string name in testNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Destroy(obj);
                Debug.Log($"清理了测试对象: {name}");
            }
        }
        
        Debug.Log("测试对象清理完成");
    }
    
    void OnDestroy()
    {
        // 自动清理
        CleanupTestObjects();
    }
}
