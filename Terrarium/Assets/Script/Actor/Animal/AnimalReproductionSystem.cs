using System.Collections;
using UnityEngine;

/// <summary>
/// 动物繁衍系统 - 处理寻找配偶、繁衍、成长等逻辑
/// </summary>
public class AnimalReproductionSystem : MonoBehaviour, IAnimalReproduction
{
    [Header("繁衍设置")]
    [SerializeField] private float mateSearchRadius = 20f;
    [SerializeField] private float cooldownDuration = 2f;
    [SerializeField] private float reproductionChance = 0.8f;
    [SerializeField] private float growthTime = 1f;
    
    // 繁衍状态
    private bool isLookingForMate = false;
    private bool isReproducing = false;
    private bool isAdult = false;
    private bool isNewborn = false;
    private float reproductionCooldown = 0f;
    private Transform targetMate = null;
    
    // 等待相关（新生幼体）
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private readonly float waitDuration = 3f;
    
    // 环境影响修正系数
    private float environmentalReproductionModifier = 1f;
    
    // 属性实现
    public bool CanReproduce
    {
        get
        {
            if (!isAdult || isReproducing || reproductionCooldown > 0f)
                return false;

            // 检查是否饥饿或口渴
            var needsSystem = GetComponent<AnimalNeedsSystem>();
            if (needsSystem != null && (needsSystem.IsHungry || needsSystem.IsThirsty))
                return false;

            return true;
        }
    }
    public float ReproductionCooldown => reproductionCooldown;
    public bool IsAdult => isAdult;
    public bool IsNewborn => isNewborn;
    public bool IsLookingForMate => isLookingForMate;
    public Transform TargetMate => targetMate;
    
    // 事件
    public System.Action<Transform> OnMateFound;
    public System.Action OnReproductionStarted;
    public System.Action OnReproductionCompleted;
    public System.Action OnReproductionFailed;
    public System.Action OnBecameAdult;
    public System.Action OnNewbornWaitCompleted;
    
    void Start()
    {
        // 检查是否为新生幼体
        if (gameObject.name.Contains("Offspring"))
        {
            isNewborn = true;
            isWaiting = true;
            waitTimer = 0f;
            Debug.Log("新生幼年体开始等待3秒");
        }
    }
    
    void Update()
    {
        // 更新繁衍冷却时间
        if (reproductionCooldown > 0f)
        {
            reproductionCooldown -= Time.deltaTime;
        }
        
        // 新生幼体等待逻辑
        if (isNewborn && isWaiting)
        {
            HandleNewbornWaiting();
        }
    }
    
    public void TryFindMate()
    {
        // 检查是否可以繁衍
        if (!CanReproduce)
        {
            var needsSystem = GetComponent<AnimalNeedsSystem>();
            string reason = $"成年: {isAdult}, 正在繁衍: {isReproducing}, 冷却时间: {reproductionCooldown:F1}s";
            if (needsSystem != null)
            {
                reason += $", 饥饿: {needsSystem.IsHungry}, 口渴: {needsSystem.IsThirsty}";
            }
            Debug.Log($"动物不能繁衍 - {reason}");
            return;
        }

        // 获取动物总数
        int totalAnimalCount = GetTotalAnimalCount();
        if (totalAnimalCount < 2)
        {
            Debug.Log($"动物总数不足，无法繁衍 (当前: {totalAnimalCount})");
            return;
        }

        Debug.Log($"开始寻找配偶 - 总动物数: {totalAnimalCount}");
        
        // 查找所有其他成年动物
        AnimalItem[] allAnimals = FindObjectsOfType<AnimalItem>();
        AnimalItem suitableMate = null;
        float nearestDistance = float.MaxValue;
        int checkedAnimals = 0;
        int suitableAnimals = 0;

        foreach (AnimalItem animal in allAnimals)
        {
            // 排除自己
            if (animal == null || animal == GetComponent<AnimalItem>()) continue;

            checkedAnimals++;
            var animalReproduction = animal.GetComponent<AnimalReproductionSystem>();
            var animalNeeds = animal.GetComponent<AnimalNeedsSystem>();

            // 检查各项条件
            bool isAdult = animalReproduction != null && animalReproduction.IsAdult;
            bool notHungry = animalNeeds != null && !animalNeeds.IsHungry;
            bool noCooldown = animalReproduction != null && animalReproduction.reproductionCooldown <= 0f;
            bool hasTransform = animal.transform != null;

            if (isAdult && notHungry && noCooldown && hasTransform)
            {
                float distance = Vector3.Distance(transform.position, animal.transform.position);
                Debug.Log($"发现潜在配偶: {animal.name}, 距离: {distance:F2}m");

                if (distance <= mateSearchRadius && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    suitableMate = animal;
                    suitableAnimals++;
                }
            }
            else
            {
                string reason = "";
                if (!isAdult) reason += "非成年 ";
                if (!notHungry) reason += "饥饿 ";
                if (!noCooldown) reason += $"冷却中({animalReproduction?.reproductionCooldown:F1}s) ";
                if (!hasTransform) reason += "无Transform ";

                Debug.Log($"动物 {animal.name} 不适合繁衍: {reason}");
            }
        }

        Debug.Log($"配偶搜索结果 - 检查了{checkedAnimals}只动物，找到{suitableAnimals}只合适的");
        
        if (suitableMate != null && suitableMate.transform != null)
        {
            targetMate = suitableMate.transform;
            isLookingForMate = true;
            Debug.Log($"找到配偶，开始接近进行繁衍，距离: {nearestDistance:F2}");
            OnMateFound?.Invoke(targetMate);
        }
    }
    
    public void AttemptReproduction(Transform mate)
    {
        // 防止重复繁衍
        if (isReproducing || reproductionCooldown > 0f) return;
        
        if (mate == null)
        {
            Debug.Log("配偶为空，停止繁衍尝试");
            StopLookingForMate();
            return;
        }
        
        var mateAnimal = mate.GetComponent<AnimalItem>();
        var mateReproduction = mate.GetComponent<AnimalReproductionSystem>();
        var mateNeeds = mate.GetComponent<AnimalNeedsSystem>();
        
        // 确保配偶仍然符合所有条件
        if (mateAnimal != null && mateReproduction != null && mateReproduction.IsAdult && 
            mateNeeds != null && !mateNeeds.IsHungry && !isReproducing && 
            mateReproduction.reproductionCooldown <= 0f)
        {
            // 应用环境影响的繁衍成功率
            float finalReproductionChance = reproductionChance * environmentalReproductionModifier;
            if (Random.Range(0f, 1f) > finalReproductionChance)
            {
                Debug.Log($"繁衍失败，环境影响成功率: {finalReproductionChance:F2}");
                FailedMating();
                mateReproduction.FailedMating();
                return;
            }
            
            // 设置繁衍标志和冷却时间
            isReproducing = true;
            mateReproduction.isReproducing = true;
            reproductionCooldown = cooldownDuration;
            mateReproduction.reproductionCooldown = cooldownDuration;
            
            // 在两个动物中间位置生成幼年体
            Vector3 reproductionPosition = (transform.position + mate.position) / 2f;
            reproductionPosition.y += 2f;
            
            SpawnOffspring(reproductionPosition);
            
            // 繁衍完成
            CompleteMating();
            mateReproduction.CompleteMating();

            Debug.Log("繁衍成功，生成了幼年体");
            OnReproductionCompleted?.Invoke();
            mateReproduction.OnReproductionCompleted?.Invoke();
        }
        else
        {
            // 配偶状态不符合，停止寻找配偶
            StopLookingForMate();
            Debug.Log("配偶状态不符合繁衍条件，停止寻找配偶");
        }
    }
    
    public void GrowToAdult()
    {
        if (isAdult) return;
        
        StartCoroutine(GrowthProcess());
    }
    
    private IEnumerator GrowthProcess()
    {
        isAdult = true;
        
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material animalMaterial = meshRenderer.material;
        Color originalColor = animalMaterial.color;
        Color targetColor = new(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < growthTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthTime;
            
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            animalMaterial.color = Color.Lerp(originalColor, targetColor, progress);
            
            yield return null;
        }
        
        transform.localScale = targetScale;
        animalMaterial.color = targetColor;
        
        Debug.Log("成长为成年体：体型变大，颜色变暗");
        OnBecameAdult?.Invoke();
    }
    
    private void HandleNewbornWaiting()
    {
        waitTimer += Time.deltaTime;
        
        if (waitTimer >= waitDuration)
        {
            // 等待结束
            isWaiting = false;
            isNewborn = false;
            
            // 添加刚体让新生动物自然下落
            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = 1f;
            }
            
            Debug.Log("新生幼年体等待结束，开始自然下落");
            OnNewbornWaitCompleted?.Invoke();
        }
    }
    
    private void SpawnOffspring(Vector3 position)
    {
        // 创建新的幼年体
        GameObject offspring = new("AnimalItem_Offspring");
        offspring.transform.position = position;
        
        // 添加AnimalItem组件
        AnimalItem offspringComponent = offspring.AddComponent<AnimalItem>();
        
        Debug.Log($"在位置 {position} 繁衍了新的幼年体动物");
    }
    
    private void CompleteMating()
    {
        isLookingForMate = false;
        targetMate = null;
        isReproducing = false;
        
        Debug.Log("繁衍完成");
    }
    
    public void FailedMating()
    {
        isLookingForMate = false;
        targetMate = null;
        isReproducing = false;
        
        Debug.Log("繁衍失败");
        OnReproductionFailed?.Invoke();
    }
    
    public void StopLookingForMate()
    {
        isLookingForMate = false;
        targetMate = null;
        Debug.Log("停止寻找配偶");
    }
    
    public void SetEnvironmentalReproductionModifier(float modifier)
    {
        environmentalReproductionModifier = Mathf.Clamp01(modifier);
    }
    
    private int GetTotalAnimalCount()
    {
        // 这里应该从AnimalItem的静态变量获取，暂时用FindObjectsOfType
        return FindObjectsOfType<AnimalItem>().Length;
    }
    
    // 检查是否在等待状态（新生幼体）
    public bool IsWaiting()
    {
        return isWaiting;
    }
}
