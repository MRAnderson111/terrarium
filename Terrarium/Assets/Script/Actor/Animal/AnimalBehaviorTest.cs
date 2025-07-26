using UnityEngine;

/// <summary>
/// 动物行为测试脚本 - 用于验证动物行为流程
/// </summary>
public class AnimalBehaviorTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float checkInterval = 5f;
    
    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== 动物行为测试开始 ===");
            
            // 定期检查动物行为状态
            InvokeRepeating(nameof(CheckAnimalBehaviors), 2f, checkInterval);
        }
    }
    
    private void CheckAnimalBehaviors()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        
        Debug.Log($"=== 动物行为检查 (共{animals.Length}只动物) ===");
        
        foreach (AnimalItem animal in animals)
        {
            if (animal != null)
            {
                CheckIndividualAnimal(animal);
            }
        }
        
        Debug.Log("=== 动物行为检查完成 ===");
    }
    
    private void CheckIndividualAnimal(AnimalItem animal)
    {
        // 获取各个系统组件
        var movement = animal.GetComponent<AnimalMovementSystem>();
        var needs = animal.GetComponent<AnimalNeedsSystem>();
        var reproduction = animal.GetComponent<AnimalReproductionSystem>();
        var environment = animal.GetComponent<AnimalEnvironmentSystem>();
        var lifecycle = animal.GetComponent<AnimalLifecycleSystem>();
        var visual = animal.GetComponent<AnimalVisualSystem>();
        var habitat = animal.GetComponent<AnimalHabitatSystem>();
        
        string status = $"动物 {animal.name}:\n";
        
        // 基础状态
        status += $"  位置: {animal.transform.position}\n";
        status += $"  生命时间: {(lifecycle != null ? lifecycle.LifeTimer.ToString("F1") : "N/A")}s / {(lifecycle != null ? lifecycle.MaxLifeTime.ToString("F1") : "N/A")}s\n";
        
        // 移动状态
        if (movement != null)
        {
            status += $"  移动: 正在移动={movement.IsMoving}, 游荡中={movement.IsWandering}\n";
        }
        
        // 需求状态
        if (needs != null)
        {
            status += $"  需求: 饥饿={needs.IsHungry}, 口渴={needs.IsThirsty}, 已进食={needs.HasEaten}\n";
            status += $"  目标: 植物={GetObjectName(needs.TargetPlant)}, 水源={GetObjectName(needs.TargetWater)}\n";
        }
        
        // 繁衍状态
        if (reproduction != null)
        {
            status += $"  繁衍: 成年={reproduction.IsAdult}, 新生={reproduction.IsNewborn}, 等待={reproduction.IsWaiting()}\n";
            status += $"  配偶: 寻找中={reproduction.IsLookingForMate}, 可繁衍={reproduction.CanReproduce}\n";
            status += $"  冷却: {reproduction.ReproductionCooldown:F1}s\n";
        }
        
        // 环境状态
        if (environment != null)
        {
            status += $"  环境: 压力={environment.EnvironmentalStress:F2}, 适宜={environment.IsInOptimalEnvironment}\n";
        }
        
        // 居住地状态
        if (habitat != null)
        {
            status += $"  居住地: 拥有={habitat.HasHabitat}, 睡眠={habitat.IsSleeping}\n";
        }
        
        Debug.Log(status);
        
        // 检查潜在问题
        CheckPotentialIssues(animal, movement, needs, reproduction);
    }
    
    private void CheckPotentialIssues(AnimalItem animal, AnimalMovementSystem movement, AnimalNeedsSystem needs, AnimalReproductionSystem reproduction)
    {
        // 检查是否卡在某个状态
        if (needs != null && needs.IsHungry && needs.TargetPlant == null)
        {
            Debug.LogWarning($"警告: {animal.name} 饥饿但没有找到植物目标！");
        }
        
        if (needs != null && needs.IsThirsty && needs.TargetWater == null)
        {
            Debug.LogWarning($"警告: {animal.name} 口渴但没有找到水源目标！");
        }
        
        if (reproduction != null && reproduction.IsAdult && !reproduction.IsLookingForMate && 
            reproduction.CanReproduce && movement != null && movement.IsWandering)
        {
            Debug.Log($"提示: {animal.name} 是成年体且可以繁衍，应该会寻找配偶");
        }
        
        if (movement != null && !movement.IsMoving && !movement.IsWandering && 
            needs != null && (needs.IsHungry || needs.IsThirsty))
        {
            Debug.LogWarning($"警告: {animal.name} 有需求但没有移动！");
        }
    }
    
    private string GetObjectName(Transform target)
    {
        return target != null ? target.name : "无";
    }
    
    // 手动测试方法
    [ContextMenu("立即检查动物行为")]
    public void ManualCheckBehaviors()
    {
        CheckAnimalBehaviors();
    }
    
    [ContextMenu("强制动物饥饿")]
    public void ForceAnimalHungry()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        foreach (AnimalItem animal in animals)
        {
            var needs = animal.GetComponent<AnimalNeedsSystem>();
            if (needs != null)
            {
                needs.SetHungryAndThirsty();
                Debug.Log($"强制设置 {animal.name} 为饥饿和口渴状态");
            }
        }
    }
    
    [ContextMenu("强制动物成年")]
    public void ForceAnimalAdult()
    {
        AnimalItem[] animals = FindObjectsOfType<AnimalItem>();
        foreach (AnimalItem animal in animals)
        {
            var reproduction = animal.GetComponent<AnimalReproductionSystem>();
            if (reproduction != null && !reproduction.IsAdult)
            {
                reproduction.GrowToAdult();
                Debug.Log($"强制 {animal.name} 成长为成年体");
            }
        }
    }
    
    [ContextMenu("检查场景植物和水源")]
    public void CheckSceneResources()
    {
        // 检查植物
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int plantCount = 0;
        int waterCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Plant") || obj.GetComponent<MonoBehaviour>()?.GetType().Name.Contains("Plant") == true)
            {
                plantCount++;
                Debug.Log($"发现植物: {obj.name} 位置: {obj.transform.position}");
            }
            
            if (obj.name.Contains("Water") || obj.name.Contains("water") ||
                obj.name.Contains("Lake") || obj.name.Contains("River") || obj.name.Contains("Pond"))
            {
                waterCount++;
                Debug.Log($"发现水源: {obj.name} 位置: {obj.transform.position}");
            }
        }
        
        Debug.Log($"场景资源统计 - 植物: {plantCount}, 水源: {waterCount}");
        
        if (plantCount == 0)
        {
            Debug.LogError("错误: 场景中没有植物！动物无法进食。");
        }
        
        if (waterCount == 0)
        {
            Debug.LogError("错误: 场景中没有水源！动物无法喝水。");
        }
    }
}
