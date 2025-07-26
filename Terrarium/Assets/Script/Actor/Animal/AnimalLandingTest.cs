using UnityEngine;

/// <summary>
/// 动物落地测试脚本 - 用于诊断落地问题
/// </summary>
public class AnimalLandingTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float testHeight = 20f;
    
    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== 动物落地测试开始 ===");
            
            // 检查场景中的地面对象
            CheckSceneGroundObjects();
            
            // 检查动物的物理组件
            CheckAnimalPhysicsComponents();
            
            // 每5秒检查一次动物状态
            InvokeRepeating(nameof(CheckAnimalStatus), 2f, 5f);
        }
    }
    
    private void CheckSceneGroundObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int groundCount = 0;
        
        Debug.Log("=== 场景地面对象检查 ===");
        
        foreach (GameObject obj in allObjects)
        {
            if (IsGroundObject(obj))
            {
                groundCount++;
                Collider collider = obj.GetComponent<Collider>();
                string colliderInfo = collider != null ? $"碰撞器: {collider.GetType().Name}" : "无碰撞器";
                
                Debug.Log($"地面对象: {obj.name}, 位置: {obj.transform.position}, {colliderInfo}");
            }
        }
        
        Debug.Log($"总共发现 {groundCount} 个地面对象");
        
        if (groundCount == 0)
        {
            Debug.LogWarning("警告：场景中没有发现任何地面对象！动物可能无法落地。");
            Debug.LogWarning("请确保场景中有名称包含 'Ground', 'Plane', 'BarrenGround', 'FertileGround', 'Terrain' 的对象，或者有 'Ground' 标签的对象。");
        }
    }
    
    private void CheckAnimalPhysicsComponents()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        
        Debug.Log("=== 动物物理组件检查 ===");
        Debug.Log($"场景中共有 {animals.Length} 只动物");
        
        foreach (AnimalItem animal in animals)
        {
            if (animal != null)
            {
                Rigidbody rb = animal.GetComponent<Rigidbody>();
                Collider col = animal.GetComponent<Collider>();
                
                string rbInfo = rb != null ? $"刚体: 质量={rb.mass}, 运动学={rb.isKinematic}" : "无刚体";
                string colInfo = col != null ? $"碰撞器: {col.GetType().Name}" : "无碰撞器";
                
                Debug.Log($"动物 {animal.name}: 位置={animal.transform.position}, {rbInfo}, {colInfo}");
                
                if (rb == null)
                {
                    Debug.LogError($"错误：动物 {animal.name} 缺少刚体组件！");
                }
                
                if (col == null)
                {
                    Debug.LogError($"错误：动物 {animal.name} 缺少碰撞器组件！");
                }
            }
        }
    }
    
    private void CheckAnimalStatus()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        
        Debug.Log("=== 动物状态检查 ===");
        
        foreach (AnimalItem animal in animals)
        {
            if (animal != null)
            {
                Rigidbody rb = animal.GetComponent<Rigidbody>();
                float height = animal.transform.position.y;
                
                string status = "未知";
                if (rb != null)
                {
                    if (rb.isKinematic)
                    {
                        status = "已落地（运动学模式）";
                    }
                    else if (Mathf.Abs(rb.velocity.magnitude) < 0.1f && height < 5f)
                    {
                        status = "可能已落地（静止状态）";
                    }
                    else
                    {
                        status = $"下落中（速度: {rb.velocity.magnitude:F2}）";
                    }
                }
                
                Debug.Log($"动物 {animal.name}: 高度={height:F2}, 状态={status}");
                
                // 如果动物高度异常，给出警告
                if (height > testHeight)
                {
                    Debug.LogWarning($"警告：动物 {animal.name} 高度异常 ({height:F2})，可能没有正常下落！");
                }
                else if (height < -10f)
                {
                    Debug.LogWarning($"警告：动物 {animal.name} 掉落到地面以下 ({height:F2})！");
                }
            }
        }
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
    
    // 手动测试方法
    [ContextMenu("手动检查地面对象")]
    public void ManualCheckGroundObjects()
    {
        CheckSceneGroundObjects();
    }
    
    [ContextMenu("手动检查动物组件")]
    public void ManualCheckAnimalComponents()
    {
        CheckAnimalPhysicsComponents();
    }
    
    [ContextMenu("手动检查动物状态")]
    public void ManualCheckAnimalStatus()
    {
        CheckAnimalStatus();
    }
    
    [ContextMenu("生成测试动物")]
    public void SpawnTestAnimal()
    {
        Vector3 spawnPos = new(0, testHeight, 0);

        GameObject testAnimal = new("TestAnimal");
        testAnimal.transform.position = spawnPos;
        testAnimal.AddComponent<AnimalItem>();
        
        Debug.Log($"在位置 {spawnPos} 生成了测试动物");
    }
}
