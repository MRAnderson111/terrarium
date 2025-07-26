using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物生存需求系统 - 处理饥饿、口渴、进食、喝水等需求
/// </summary>
public class AnimalNeedsSystem : MonoBehaviour, IAnimalNeeds
{
    [Header("生存需求设置")]
    [SerializeField] private float hungerInterval = 15f; // 游荡15秒后重新饥饿
    [SerializeField] private float thirstInterval = 12f; // 口渴间隔
    
    // 需求状态
    private bool isHungry = false;
    private bool isThirsty = false;
    private bool hasEaten = false; // 幼年体是否已经吃过植物
    private bool hasDrunk = false; // 幼年体是否已经喝过水
    
    // 计时器
    private float hungerTimer = 0f;
    private float thirstTimer = 0f;
    
    // 目标缓存
    private Transform targetPlant = null;
    private Transform targetWater = null;
    
    // 缓存搜索结果，避免频繁搜索
    private static GameObject[] cachedPlants;
    private static GameObject[] cachedWaterSources;
    private static float lastPlantCacheTime;
    private static float lastWaterCacheTime;
    private static readonly float cacheValidTime = 1f; // 缓存1秒有效
    
    // 属性实现
    public bool IsHungry => isHungry;
    public bool IsThirsty => isThirsty;
    public bool HasEaten => hasEaten;
    public bool HasDrunk => hasDrunk;
    public Transform TargetPlant => targetPlant;
    public Transform TargetWater => targetWater;
    
    // 事件
    public System.Action<Transform> OnFoodFound;
    public System.Action<Transform> OnWaterFound;
    public System.Action OnBecameHungry;
    public System.Action OnBecameThirsty;
    public System.Action OnAteFood;
    public System.Action OnDrankWater;
    
    void Update()
    {
        UpdateNeeds();
    }
    
    public void UpdateNeeds()
    {
        // 更新饥饿状态
        if (!isHungry)
        {
            hungerTimer += Time.deltaTime;
            if (hungerTimer >= hungerInterval)
            {
                BecomeHungry();
            }
        }
        
        // 更新口渴状态
        if (!isThirsty)
        {
            thirstTimer += Time.deltaTime;
            if (thirstTimer >= thirstInterval)
            {
                BecomeThirsty();
            }
        }
    }
    
    public void Eat(Transform food)
    {
        if (food == null)
        {
            Debug.LogWarning("尝试进食但食物目标为null");
            return;
        }

        Debug.Log($"动物开始进食: {food.name}");
        Debug.Log($"进食前状态 - 饥饿: {isHungry}, 已进食: {hasEaten}");

        // 检查是否是植物（通过组件或名称）
        var plantComponent = food.GetComponent<MonoBehaviour>();
        bool isPlant = (plantComponent != null && plantComponent.GetType().Name.Contains("Plant")) ||
                       food.name.Contains("Plant") ||
                       food.name.Contains("plant") ||
                       food.name.Contains("Tree") ||
                       food.name.Contains("Grass") ||
                       food.name.Contains("Vegetation");

        if (isPlant)
        {
            Debug.Log($"确认为植物，动物开始进食: {food.name}");

            // 销毁植物
            Destroy(food.gameObject);

            // 更新状态
            isHungry = false;
            hasEaten = true;
            hungerTimer = 0f;
            targetPlant = null;

            Debug.Log($"进食后状态 - 饥饿: {isHungry}, 已进食: {hasEaten}");

            // 触发事件
            OnAteFood?.Invoke();

            Debug.Log("动物进食完毕，不再饥饿，触发OnAteFood事件");
        }
        else
        {
            Debug.LogWarning($"目标 {food.name} 不是有效的植物，无法进食");
        }
    }
    
    public void Drink(Transform water)
    {
        if (water == null)
        {
            Debug.LogWarning("尝试喝水但水源目标为null");
            return;
        }

        Debug.Log($"动物开始喝水: {water.name}");
        Debug.Log($"喝水前状态 - 口渴: {isThirsty}, 已喝水: {hasDrunk}");

        // 更新状态
        isThirsty = false;
        hasDrunk = true;
        thirstTimer = 0f;
        targetWater = null;

        Debug.Log($"喝水后状态 - 口渴: {isThirsty}, 已喝水: {hasDrunk}");

        // 触发事件
        OnDrankWater?.Invoke();

        Debug.Log("动物喝水完毕，不再口渴，触发OnDrankWater事件");
    }
    
    public void FindNearestPlant()
    {
        // 使用缓存的植物列表
        if (cachedPlants == null || Time.time - lastPlantCacheTime > cacheValidTime)
        {
            // 查找所有植物对象
            cachedPlants = FindObjectsOfType<GameObject>();
            List<GameObject> plants = new List<GameObject>();
            
            foreach (GameObject obj in cachedPlants)
            {
                if (obj.name.Contains("Plant") || obj.GetComponent<MonoBehaviour>()?.GetType().Name.Contains("Plant") == true)
                {
                    plants.Add(obj);
                }
            }
            
            cachedPlants = plants.ToArray();
            lastPlantCacheTime = Time.time;
        }
        
        if (cachedPlants.Length == 0)
        {
            targetPlant = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestPlant = null;
        
        foreach (GameObject plant in cachedPlants)
        {
            if (plant != null && plant.transform != null)
            {
                float distance = Vector3.Distance(transform.position, plant.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlant = plant.transform;
                }
            }
        }
        
        targetPlant = nearestPlant;
        if (targetPlant != null)
        {
            Debug.Log($"找到最近的植物，距离: {nearestDistance:F2}");
            OnFoodFound?.Invoke(targetPlant);
        }
    }
    
    public void FindNearestWater()
    {
        // 使用缓存的水源列表
        if (cachedWaterSources == null || Time.time - lastWaterCacheTime > cacheValidTime)
        {
            // 直接通过名称查找水源对象（避免使用不存在的标签）
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            List<GameObject> waterObjects = new();

            foreach (GameObject obj in allObjects)
            {
                // 检查对象名称是否包含水源相关关键词
                if (obj.name.Contains("Water") ||
                    obj.name.Contains("water") ||
                    obj.name.Contains("Lake") ||
                    obj.name.Contains("River") ||
                    obj.name.Contains("Pond"))
                {
                    waterObjects.Add(obj);
                }
            }

            cachedWaterSources = waterObjects.ToArray();
            lastWaterCacheTime = Time.time;
        }
        
        if (cachedWaterSources.Length == 0)
        {
            targetWater = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestWater = null;
        
        foreach (GameObject water in cachedWaterSources)
        {
            if (water != null && water.transform != null)
            {
                float distance = Vector3.Distance(transform.position, water.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWater = water.transform;
                }
            }
        }
        
        targetWater = nearestWater;
        if (targetWater != null)
        {
            Debug.Log($"找到最近的水源，距离: {nearestDistance:F2}");
            OnWaterFound?.Invoke(targetWater);
        }
    }
    
    private void BecomeHungry()
    {
        isHungry = true;
        hungerTimer = 0f;
        Debug.Log("动物变得饥饿");
        OnBecameHungry?.Invoke();
    }
    
    private void BecomeThirsty()
    {
        isThirsty = true;
        thirstTimer = 0f;
        Debug.Log("动物变得口渴");
        OnBecameThirsty?.Invoke();
    }
    
    // 重置需求状态（用于新生动物或特殊情况）
    public void ResetNeeds()
    {
        isHungry = false;
        isThirsty = false;
        hasEaten = false;
        hasDrunk = false;
        hungerTimer = 0f;
        thirstTimer = 0f;
        targetPlant = null;
        targetWater = null;
        Debug.Log("重置动物需求状态");
    }
    
    // 设置需求状态（用于特殊情况，如繁衍后消耗体力）
    public void SetHungryAndThirsty()
    {
        isHungry = true;
        isThirsty = true;
        hungerTimer = 0f;
        thirstTimer = 0f;
        Debug.Log("设置动物为饥饿和口渴状态");
        OnBecameHungry?.Invoke();
        OnBecameThirsty?.Invoke();
    }
    
    // 获取需求优先级（用于决定先寻找食物还是水源）
    public bool ShouldPrioritizeWater()
    {
        // 如果都需要，优先水源
        return isThirsty && isHungry;
    }
}
