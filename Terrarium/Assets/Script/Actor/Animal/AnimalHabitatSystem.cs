using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物居住地系统 - 处理居住地建立、管理、睡眠等
/// </summary>
public class AnimalHabitatSystem : MonoBehaviour
{
    [Header("居住地设置")]
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private int searchAttempts = 20;
    [SerializeField] private float buildDelay = 2f;
    
    // 居住地状态
    private bool hasHabitat = false;
    private bool isBuildingHabitat = false;
    private Transform myHabitat = null;
    
    // 睡眠相关
    private bool isSleeping = false;
    private bool isGoingToSleep = false;
    
    // 静态变量记录共享居住地
    public static Transform sharedHabitat = null;
    private static bool isCreatingHabitat = false;
    
    // 属性
    public bool HasHabitat => hasHabitat;
    public bool IsBuildingHabitat => isBuildingHabitat;
    public bool IsSleeping => isSleeping;
    public bool IsGoingToSleep => isGoingToSleep;
    public Transform MyHabitat => myHabitat;
    
    // 事件
    public System.Action OnHabitatBuilt;
    public System.Action OnHabitatEntered;
    public System.Action OnHabitatExited;
    public System.Action OnSleepStarted;
    public System.Action OnSleepEnded;
    
    public void TryEstablishHabitat()
    {
        // 成年体吃饱后优先建立或寻找居住地
        var reproduction = GetComponent<AnimalReproductionSystem>();
        if (reproduction == null || !reproduction.IsAdult || hasHabitat || isBuildingHabitat)
            return;
        
        // 检查是否已经有共享居住地
        if (sharedHabitat != null)
        {
            // 使用现有的共享居住地
            myHabitat = sharedHabitat;
            hasHabitat = true;
            Debug.Log("使用现有的共享居住地");
            OnHabitatBuilt?.Invoke();
        }
        else if (!isCreatingHabitat)
        {
            // 创建新的共享居住地（只有在没有其他动物正在创建时）
            StartCoroutine(BuildHabitatAfterDelay());
        }
        else
        {
            // 有其他动物正在创建居住地，等待一段时间后再检查
            StartCoroutine(WaitForHabitatCreation());
        }
    }
    
    private IEnumerator WaitForHabitatCreation()
    {
        // 等待其他动物创建居住地
        float waitTime = 0f;
        float maxWaitTime = 5f;
        
        while (sharedHabitat == null && isCreatingHabitat && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;
        }
        
        // 检查是否有可用的居住地
        if (sharedHabitat != null)
        {
            myHabitat = sharedHabitat;
            hasHabitat = true;
            Debug.Log("等待后使用了共享居住地");
            OnHabitatBuilt?.Invoke();
        }
        else if (!isCreatingHabitat)
        {
            // 如果没有居住地且没有其他动物在创建，自己创建
            StartCoroutine(BuildHabitatAfterDelay());
        }
    }
    
    private IEnumerator BuildHabitatAfterDelay()
    {
        // 设置创建标志，防止其他动物同时创建
        if (isCreatingHabitat)
        {
            yield break; // 如果已经有动物在创建，直接退出
        }
        
        isCreatingHabitat = true;
        isBuildingHabitat = true;
        
        // 等待后建立居住地
        yield return new WaitForSeconds(buildDelay);
        
        // 再次检查是否已经有其他动物创建了居住地
        if (sharedHabitat == null)
        {
            BuildHabitat();
        }
        else
        {
            // 使用现有的共享居住地
            myHabitat = sharedHabitat;
            hasHabitat = true;
            Debug.Log("在等待期间发现了共享居住地，直接使用");
            OnHabitatBuilt?.Invoke();
        }
        
        isCreatingHabitat = false;
        isBuildingHabitat = false;
    }
    
    private void BuildHabitat()
    {
        // 最后一次检查，确保没有重复创建
        if (sharedHabitat != null)
        {
            myHabitat = sharedHabitat;
            hasHabitat = true;
            Debug.Log("发现居住地已存在，直接使用");
            OnHabitatBuilt?.Invoke();
            return;
        }
        
        // 寻找最佳居住地位置
        Vector3 bestPosition = FindBestHabitatPosition();
        
        // 创建居住地对象
        GameObject habitat = new GameObject("AnimalHabitat_Shared");
        habitat.transform.position = bestPosition;
        
        // 添加Actor_AnimalHabitat组件
        try
        {
            var habitatType = System.Type.GetType("Actor_AnimalHabitat");
            if (habitatType != null)
            {
                habitat.AddComponent(habitatType);
                
                // 设置为共享居住地
                sharedHabitat = habitat.transform;
                myHabitat = sharedHabitat;
                hasHabitat = true;
                
                Debug.Log($"在最佳位置 {bestPosition} 建立了共享居住地");
                OnHabitatBuilt?.Invoke();
            }
            else
            {
                Debug.LogWarning("未找到Actor_AnimalHabitat类");
                Destroy(habitat);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"建立居住地失败: {e.Message}");
            Destroy(habitat);
        }
    }
    
    private Vector3 FindBestHabitatPosition()
    {
        Vector3 bestPosition = transform.position;
        float bestScore = float.MinValue;
        
        // 在当前位置周围搜索多个候选位置
        for (int i = 0; i < searchAttempts; i++)
        {
            // 生成随机候选位置
            Vector2 randomOffset = Random.insideUnitCircle * searchRadius;
            Vector3 candidatePosition = transform.position + new Vector3(
                randomOffset.x,
                0f,
                randomOffset.y
            );
            
            // 评估这个位置的适宜性
            float score = EvaluateHabitatPosition(candidatePosition);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = candidatePosition;
            }
        }
        
        Debug.Log($"居住地选址完成，最佳评分: {bestScore:F2}");
        return bestPosition;
    }
    
    private float EvaluateHabitatPosition(Vector3 position)
    {
        float score = 0f;
        
        // 1. 靠近植物（食物）加分
        float foodScore = CalculateFoodProximityScore(position);
        score += foodScore * 3f; // 食物权重为3
        
        // 2. 靠近水源加分
        float waterScore = CalculateWaterProximityScore(position);
        score += waterScore * 2f; // 水源权重为2
        
        // 3. 远离捕食者加分
        float predatorScore = CalculatePredatorDistanceScore(position);
        score += predatorScore * 4f; // 远离捕食者权重为4（最重要）
        
        return score;
    }
    
    private float CalculateFoodProximityScore(Vector3 position)
    {
        // 查找所有植物
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> plants = new List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Plant") || obj.GetComponent<MonoBehaviour>()?.GetType().Name.Contains("Plant") == true)
            {
                plants.Add(obj);
            }
        }
        
        if (plants.Count == 0)
            return 0f;
        
        float totalScore = 0f;
        int plantCount = 0;
        
        foreach (GameObject plant in plants)
        {
            if (plant != null && plant.transform != null)
            {
                float distance = Vector3.Distance(position, plant.transform.position);
                // 距离越近分数越高，最大有效距离为50米
                float plantScore = Mathf.Max(0f, (50f - distance) / 50f);
                totalScore += plantScore;
                plantCount++;
            }
        }
        
        return plantCount > 0 ? totalScore / plantCount : 0f;
    }
    
    private float CalculateWaterProximityScore(Vector3 position)
    {
        // 基于环境工厂的水源和湿度数据
        try
        {
            var envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                var humidityProperty = envFactoryType.GetProperty("Humidity");
                var waterSourceProperty = envFactoryType.GetProperty("WaterSource");
                
                if (humidityProperty != null && waterSourceProperty != null)
                {
                    float humidity = (float)humidityProperty.GetValue(null);
                    float waterSource = (float)waterSourceProperty.GetValue(null);
                    
                    // 湿度和水源越高，分数越高
                    return (humidity + waterSource) / 200f; // 归一化到0-1
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取水源数据失败: {e.Message}");
        }
        
        return 0.5f; // 默认中等分数
    }
    
    private float CalculatePredatorDistanceScore(Vector3 position)
    {
        // 查找所有捕食者
        try
        {
            var predatorType = System.Type.GetType("Actor_Predator");
            if (predatorType != null)
            {
                Component[] allPredators = FindObjectsOfType(predatorType) as Component[];
                
                if (allPredators == null || allPredators.Length == 0)
                    return 1f; // 没有捕食者，满分
                
                float minDistance = float.MaxValue;
                
                foreach (Component predator in allPredators)
                {
                    if (predator != null && predator.transform != null)
                    {
                        float distance = Vector3.Distance(position, predator.transform.position);
                        minDistance = Mathf.Min(minDistance, distance);
                    }
                }
                
                // 距离捕食者越远分数越高，安全距离为30米
                return Mathf.Min(1f, minDistance / 30f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取捕食者数据失败: {e.Message}");
        }
        
        return 1f; // 默认满分（假设没有捕食者）
    }
    
    public void HandleSleepBehavior()
    {
        // 检查是否是夜晚
        bool isNight = false;
        try
        {
            var daylightType = System.Type.GetType("Actor_Daylight");
            if (daylightType != null)
            {
                var isNightProperty = daylightType.GetProperty("IsNight");
                if (isNightProperty != null)
                {
                    isNight = (bool)isNightProperty.GetValue(null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取昼夜状态失败: {e.Message}");
        }
        
        if (isNight && hasHabitat && !isSleeping && !isGoingToSleep)
        {
            // 夜晚且有居住地，前往睡觉
            isGoingToSleep = true;
            Debug.Log("夜晚到来，前往居住地睡觉");
        }
        else if (!isNight && isSleeping)
        {
            // 白天且正在睡觉，醒来
            WakeUp();
        }
    }
    
    public void MoveToHabitat()
    {
        if (myHabitat == null) return;
        
        var movement = GetComponent<AnimalMovementSystem>();
        if (movement != null)
        {
            movement.MoveTo(myHabitat.position);
            movement.OnTargetReached += OnReachedHabitat;
        }
    }
    
    private void OnReachedHabitat()
    {
        var movement = GetComponent<AnimalMovementSystem>();
        if (movement != null)
        {
            movement.OnTargetReached -= OnReachedHabitat;
        }
        
        if (isGoingToSleep)
        {
            StartSleeping();
        }
    }
    
    private void StartSleeping()
    {
        isSleeping = true;
        isGoingToSleep = false;
        
        Debug.Log("开始睡觉");
        OnSleepStarted?.Invoke();
    }
    
    private void WakeUp()
    {
        isSleeping = false;
        isGoingToSleep = false;
        
        Debug.Log("醒来");
        OnSleepEnded?.Invoke();
    }
    
    void OnTriggerEnter(Collider other)
    {
        // 检查是否进入了自己的居住地
        if (myHabitat != null && other.transform == myHabitat)
        {
            Debug.Log("进入了自己的居住地");
            OnHabitatEntered?.Invoke();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // 检查是否离开了自己的居住地
        if (myHabitat != null && other.transform == myHabitat)
        {
            Debug.Log("离开了自己的居住地");
            OnHabitatExited?.Invoke();
        }
    }
    
    void OnDestroy()
    {
        // 如果是共享居住地的最后一个动物，清理共享居住地
        var lifecycle = GetComponent<AnimalLifecycleSystem>();
        if (sharedHabitat != null && lifecycle != null && AnimalLifecycleSystem.totalAnimalCount <= 1)
        {
            if (sharedHabitat.gameObject != null)
                Destroy(sharedHabitat.gameObject);
            sharedHabitat = null;
        }
    }
}
