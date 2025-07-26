using System.Collections;
using UnityEngine;

/// <summary>
/// 动物生命周期系统 - 处理生命计时、自然死亡等
/// </summary>
public class AnimalLifecycleSystem : MonoBehaviour, IAnimalLifecycle
{
    [Header("生命周期设置")]
    [SerializeField] private float maxLifeTime = 60f; // 最大生存时间60秒（1分钟）
    [SerializeField] private float fadeTime = 1f; // 死亡消散时间
    
    // 生命周期状态
    private float lifeTimer = 0f;
    private bool isDying = false;
    
    // 静态变量记录所有动物数量
    public static int totalAnimalCount = 0;
    public static int environmentalAnimalValue = 0; // 环境动物数量（动物数量的两倍）
    
    // 属性实现
    public float LifeTimer => lifeTimer;
    public float MaxLifeTime => maxLifeTime;
    public bool IsDying => isDying;
    
    // 事件
    public System.Action OnDeathStarted;
    public System.Action OnDeathCompleted;
    public System.Action<float> OnLifeTimerUpdated;
    
    void Start()
    {
        // 动物生成时增加总数量
        totalAnimalCount++;
        UpdateEnvironmentalAnimalValue();
        
        // 初始化生命计时器
        lifeTimer = 0f;
        Debug.Log($"动物开始生命周期，将在{maxLifeTime}秒后死亡");
        Debug.Log($"动物数量: {totalAnimalCount}, 环境动物数量: {environmentalAnimalValue}");
    }
    
    void Update()
    {
        // 生命周期检查
        if (!isDying)
        {
            lifeTimer += Time.deltaTime;
            OnLifeTimerUpdated?.Invoke(lifeTimer);
            
            if (lifeTimer >= maxLifeTime)
            {
                Die();
            }
        }
    }
    
    public void Die()
    {
        if (isDying) return; // 防止重复执行死亡过程
        
        StartCoroutine(DeathProcess());
    }
    
    private IEnumerator DeathProcess()
    {
        isDying = true;
        
        Debug.Log($"动物开始死亡过程，存活时间: {lifeTimer:F1}秒");
        OnDeathStarted?.Invoke();
        
        // 停止所有行为
        StopAllBehaviors();
        
        // 获取渲染器和材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material animalMaterial = meshRenderer.material;
            Color originalColor = animalMaterial.color;
            
            // 逐渐变透明（消散效果）
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeTime);
                
                // 设置透明度
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                animalMaterial.color = newColor;
                
                yield return null;
            }
        }
        
        Debug.Log("动物死亡消散完成");
        OnDeathCompleted?.Invoke();
        
        // 销毁对象
        Destroy(gameObject);
    }
    
    private void StopAllBehaviors()
    {
        // 停止移动系统
        var movement = GetComponent<AnimalMovementSystem>();
        if (movement != null)
        {
            movement.StopMovement();
        }
        
        // 停止繁衍系统
        var reproduction = GetComponent<AnimalReproductionSystem>();
        if (reproduction != null)
        {
            reproduction.StopLookingForMate();
        }
        
        // 停止其他行为组件
        var behaviors = GetComponents<IAnimalBehavior>();
        foreach (var behavior in behaviors)
        {
            behavior.StopBehavior();
        }
    }
    
    void OnDestroy()
    {
        // 停止所有协程，防止内存泄漏
        StopAllCoroutines();
        
        // 减少总数量
        totalAnimalCount--;
        UpdateEnvironmentalAnimalValue();
        
        Debug.Log($"动物被销毁，剩余动物数量: {totalAnimalCount}, 环境动物数值: {environmentalAnimalValue}");
    }
    
    private static void UpdateEnvironmentalAnimalValue()
    {
        environmentalAnimalValue = totalAnimalCount * 2;
    }
    
    // 公共静态方法供UI访问
    public static int GetTotalAnimalCount()
    {
        return totalAnimalCount;
    }
    
    public static int GetEnvironmentalAnimalValue()
    {
        return environmentalAnimalValue;
    }
    
    // 设置最大生命时间（用于不同种类的动物）
    public void SetMaxLifeTime(float newMaxLifeTime)
    {
        maxLifeTime = newMaxLifeTime;
        Debug.Log($"设置动物最大生命时间为: {maxLifeTime}秒");
    }
    
    // 获取生命进度百分比
    public float GetLifeProgress()
    {
        return Mathf.Clamp01(lifeTimer / maxLifeTime);
    }
    
    // 获取剩余生命时间
    public float GetRemainingLifeTime()
    {
        return Mathf.Max(0f, maxLifeTime - lifeTimer);
    }
    
    // 延长生命时间（用于特殊情况，如医疗等）
    public void ExtendLifeTime(float additionalTime)
    {
        maxLifeTime += additionalTime;
        Debug.Log($"延长动物生命时间 {additionalTime}秒，新的最大生命时间: {maxLifeTime}秒");
    }
    
    // 立即死亡（用于被捕食等情况）
    public void InstantDeath()
    {
        if (!isDying)
        {
            lifeTimer = maxLifeTime; // 设置为最大值触发死亡
            Debug.Log("动物立即死亡");
        }
    }
    
    // 检查是否接近死亡
    public bool IsNearDeath(float threshold = 0.8f)
    {
        return GetLifeProgress() >= threshold;
    }
    
    // 重置生命计时器（用于特殊情况，如重生等）
    public void ResetLifeTimer()
    {
        lifeTimer = 0f;
        isDying = false;
        Debug.Log("重置动物生命计时器");
    }
}
