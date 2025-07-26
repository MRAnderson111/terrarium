using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Predator : MonoBehaviour
{
    private static bool keyProcessedThisFrame; // 静态变量控制每帧只处理一次
    private bool hasGrounded = false; // 是否已经接触地面
    private bool isMovingToAnimal = false; // 是否正在向动物移动
    private Transform targetAnimal = null; // 目标动物
    private readonly float moveSpeed = 12f; // 移动速度（比动物稍快）
    
    // 新增游荡相关变量
    private bool isWandering = false;
    private Vector3 wanderTarget;
    private readonly float wanderSpeed = 8f;
    private readonly float wanderRadius = 20f;
    private float wanderTimer = 0f;
    private readonly float wanderInterval = 4f; // 每4秒选择新的游荡目标
    
    // 新增饥饿状态相关变量
    private float hungerTimer = 0f;
    private readonly float hungerInterval = 10f; // 游荡10秒后重新饥饿
    private bool isHungry = true; // 捕食者默认饥饿
    
    // 新增成长相关变量
    private bool isAdult = false; // 是否为成年体
    private bool hasEaten = false; // 是否已经吃过动物
    
    // 静态变量记录所有捕食者数量
    public static int totalPredatorCount = 0;
    
    void Start()
    {
        // 捕食者生成时增加总数量
        totalPredatorCount++;
        
        InitializeComponents();
        SetupAppearance();
        
        // 添加刚体用于物理模拟
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
        
        Debug.Log($"Predator生成，总数量: {totalPredatorCount}");
    }
    
    void InitializeComponents()
    {
        // 确保有MeshRenderer和MeshFilter组件
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
        
        if (GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        
        // 添加碰撞器
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }
    
    void SetupAppearance()
    {
        // 设置为立方体网格
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建深紫色材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material predatorMaterial = new(Shader.Find("Standard"))
        {
            color = new Color(0.3f, 0f, 0.5f, 1f) // 深紫色
        };
        meshRenderer.material = predatorMaterial;
    }

    void Update()
    {
        HandleKeyInput();
        
        // 向动物移动
        if (isMovingToAnimal && targetAnimal != null)
        {
            MoveTowardsAnimal();
        }
        // 如果目标动物被销毁，寻找新目标
        else if (isMovingToAnimal && targetAnimal == null)
        {
            FindNearestAnimal();
            if (targetAnimal != null)
            {
                Debug.Log($"Predator目标动物已消失，找到新目标: {targetAnimal.name}");
            }
            else
            {
                isMovingToAnimal = false;
                StartWandering();
                Debug.Log("Predator未找到新的动物目标，开始游荡");
            }
        }
        // 游荡行为
        else if (isWandering)
        {
            HandleWandering();
            HandleHunger();
        }
    }
    
    void HandleWandering()
    {
        wanderTimer += Time.deltaTime;
        
        // 移动到游荡目标
        MoveTowardsWanderTarget();
        
        // 定期选择新的游荡目标
        if (wanderTimer >= wanderInterval)
        {
            ChooseNewWanderTarget();
            wanderTimer = 0f;
        }
    }
    
    void HandleHunger()
    {
        hungerTimer += Time.deltaTime;
        
        // 游荡10秒后重新饥饿，寻找动物
        if (hungerTimer >= hungerInterval && !isHungry)
        {
            isHungry = true;
            isWandering = false;
            hungerTimer = 0f;
            
            // 变成更鲜艳的紫色表示饥饿状态
            ChangeToHungryColor();
            
            Debug.Log("Predator重新进入饥饿状态，开始寻找动物");
            
            FindNearestAnimal();
            if (targetAnimal != null)
            {
                isMovingToAnimal = true;
                Debug.Log($"Predator找到新的猎物目标: {targetAnimal.name}");
            }
            else
            {
                // 如果没找到动物，继续游荡
                isWandering = true;
                isHungry = false;
                RestoreNormalColor();
            }
        }
    }
    
    void ChooseNewWanderTarget()
    {
        // 在当前位置周围随机选择一个目标点
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(
            transform.position.x + randomDirection.x,
            transform.position.y,
            transform.position.z + randomDirection.y
        );
        
        Debug.Log($"Predator选择新的游荡目标: {wanderTarget}");
    }
    
    void MoveTowardsWanderTarget()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 direction = (wanderTarget - transform.position).normalized;
        
        // 移动
        transform.position += Time.deltaTime * wanderSpeed * direction;
        
        // 检查是否接近目标点（距离小于2米时选择新目标）
        float distance = Vector3.Distance(transform.position, wanderTarget);
        if (distance < 2f)
        {
            ChooseNewWanderTarget();
        }
    }
    
    void StartWandering()
    {
        isWandering = true;
        isHungry = false;
        hungerTimer = 0f;
        
        RestoreNormalColor();
        ChooseNewWanderTarget();
        Debug.Log("Predator开始游荡行为");
    }
    
    void ChangeToHungryColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = new Color(0.6f, 0f, 0.8f, 1f); // 更鲜艳的紫色
            Debug.Log("Predator变成鲜艳紫色，表示饥饿状态");
        }
    }
    
    void RestoreNormalColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            if (isAdult)
            {
                // 成年体恢复为暗紫色
                meshRenderer.material.color = new Color(0.15f, 0f, 0.25f, 1f);
            }
            else
            {
                // 幼年体恢复为正常深紫色
                meshRenderer.material.color = new Color(0.3f, 0f, 0.5f, 1f);
            }
            Debug.Log("Predator恢复正常颜色");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 简化地面检测：只要碰撞到任何物体就视为落地
        if (!hasGrounded)
        {
            hasGrounded = true;

            // 完全固定捕食者位置，移除刚体组件
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                DestroyImmediate(rb);
            }

            Debug.Log("Predator落地，完全固定位置");

            // 2秒后开始寻找动物
            StartCoroutine(StartMovingToAnimalAfterDelay());
        }
    }

    System.Collections.IEnumerator StartMovingToAnimalAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);

        // 寻找最近的动物
        FindNearestAnimal();

        if (targetAnimal != null)
        {
            isMovingToAnimal = true;
            Debug.Log($"Predator开始向最近的动物移动: {targetAnimal.name}");
        }
        else
        {
            Debug.Log("Predator未找到附近的动物");
        }
    }
    
    void FindNearestAnimal()
    {
        // 使用反射查找所有带有AnimalItem组件的对象
        Component[] allAnimals = null;
        try
        {
            var animalItemType = System.Type.GetType("AnimalItem");
            if (animalItemType != null)
            {
                allAnimals = FindObjectsOfType(animalItemType) as Component[];
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"查找动物对象失败: {e.Message}");
            targetAnimal = null;
            return;
        }
        
        if (allAnimals == null || allAnimals.Length == 0)
        {
            targetAnimal = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestAnimal = null;
        
        foreach (Component animal in allAnimals)
        {
            if (animal != null && animal.transform != null)
            {
                float distance = Vector3.Distance(transform.position, animal.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestAnimal = animal.transform;
                }
            }
        }
        
        targetAnimal = nearestAnimal;
        if (targetAnimal != null)
        {
            Debug.Log($"找到最近的动物，距离: {nearestDistance:F2}");
        }
    }
    
    void MoveTowardsAnimal()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 targetPosition = new(targetAnimal.position.x, transform.position.y, targetAnimal.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 移动
        transform.position += moveSpeed * Time.deltaTime * direction;
        
        // 检查是否到达动物附近（距离小于2米）
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 2f)
        {
            isMovingToAnimal = false;
            EatAnimal();
            Debug.Log("Predator到达动物附近，停止移动");
        }
    }
    
    void EatAnimal()
    {
        if (targetAnimal != null)
        {
            // 使用反射获取AnimalItem组件
            Component animalComponent = null;
            try
            {
                var animalItemType = System.Type.GetType("AnimalItem");
                if (animalItemType != null)
                {
                    animalComponent = targetAnimal.GetComponent(animalItemType);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"获取动物组件失败: {e.Message}");
            }
            
            if (animalComponent != null)
            {
                Debug.Log($"Predator吃掉了动物: {targetAnimal.name}");
                Destroy(targetAnimal.gameObject);
                
                // 如果是幼年体，成长为成年体
                if (!isAdult && !hasEaten)
                {
                    hasEaten = true;
                    StartCoroutine(GrowToAdult());
                }
                else
                {
                    // 成年体进食后进入吃饱状态
                    isHungry = false;
                    hungerTimer = 0f;
                    
                    // 60%几率生成蓝色小球
                    if (Random.Range(0f, 1f) < 0.6f)
                    {
                        SpawnBlueSphere();
                    }
                    
                    // 吃饱后开始游荡
                    StartWandering();
                    Debug.Log("成年Predator进食完毕，进入吃饱状态，开始游荡10秒");
                }
            }
        }
    }
    
    System.Collections.IEnumerator GrowToAdult()
    {
        if (isAdult) yield break; // 如果已经是成年体，直接返回
        
        isAdult = true;
        
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f; // 体型变大到两倍
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material predatorMaterial = meshRenderer.material;
        Color originalColor = predatorMaterial.color;
        Color targetColor = new(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f); // 颜色变暗
        
        float growthTime = 1f; // 成长时间1秒
        float elapsedTime = 0f;
        
        while (elapsedTime < growthTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthTime;
            
            // 平滑插值缩放和颜色
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            predatorMaterial.color = Color.Lerp(originalColor, targetColor, progress);
            
            yield return null;
        }
        
        // 确保最终值准确
        transform.localScale = targetScale;
        predatorMaterial.color = targetColor;
        
        Debug.Log("Predator成长为成年体：体型变大，颜色变暗");
        
        // 成长为成年体后，60%几率生成蓝色小球
        if (Random.Range(0f, 1f) < 0.6f)
        {
            SpawnBlueSphere();
        }
        
        // 开始游荡行为
        StartWandering();
    }
    
    void SpawnBlueSphere()
    {
        // 在predator正上方3米处生成蓝色小球
        Vector3 spherePosition = transform.position + Vector3.up * 3f;
        
        // 创建球体对象
        GameObject blueSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        blueSphere.name = "BlueSphere";
        blueSphere.transform.position = spherePosition;
        blueSphere.transform.localScale = Vector3.one * 0.3f;
        
        // 设置蓝色材质
        MeshRenderer sphereRenderer = blueSphere.GetComponent<MeshRenderer>();
        Material blueMaterial = new(Shader.Find("Standard"))
        {
            color = Color.blue
        };
        sphereRenderer.material = blueMaterial;
        
        // 移除刚体组件（保留碰撞器用于点击检测）
        if (blueSphere.TryGetComponent<Rigidbody>(out var sphereRb))
            Destroy(sphereRb);
        
        // 保留碰撞器但设置为触发器
        if (blueSphere.TryGetComponent<Collider>(out var sphereCollider))
            sphereCollider.isTrigger = true;
        
        // 添加点击脚本
        blueSphere.AddComponent<PredatorBlueSphereClickHandler>();
        
        Debug.Log($"在位置 {spherePosition} 生成了浮空蓝色小球");
    }
    
    System.Collections.IEnumerator FindNextAnimalAfterDelay()
    {
        // 等待1秒后寻找下一个动物
        yield return new WaitForSeconds(1f);
        
        FindNearestAnimal();
        if (targetAnimal != null)
        {
            isMovingToAnimal = true;
            Debug.Log($"Predator找到下一个目标: {targetAnimal.name}");
        }
        else
        {
            Debug.Log("Predator没有找到更多动物");
        }
    }
    
    void HandleKeyInput()
    {
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha3))
            keyProcessedThisFrame = false;
        
        // 按下3键在指定位置生成黄色捕食者正方形
        if (Input.GetKeyDown(KeyCode.Alpha3) && !keyProcessedThisFrame)
        {
            keyProcessedThisFrame = true;
            SpawnPredatorAtPosition(new Vector3(-50, 15, -50));
        }
    }
    
    void SpawnPredatorAtPosition(Vector3 position)
    {
        // 创建新的Actor_Predator对象
        GameObject newPredator = new("PredatorItem");
        newPredator.transform.position = position;
        
        // 添加Actor_Predator组件
        newPredator.AddComponent<Actor_Predator>();
        
        Debug.Log($"在位置 {position} 生成了黄色捕食者正方形");
    }
    
    Mesh CreateCubeMesh()
    {
        // 使用Unity内置的立方体网格
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }
    
    void OnDestroy()
    {
        // 捕食者被销毁时减少总数量
        totalPredatorCount--;
        Debug.Log($"捕食者被销毁，剩余捕食者数量: {totalPredatorCount}");
    }
    
    // 静态方法供外部调用
    public static int GetTotalPredatorCount()
    {
        return totalPredatorCount;
    }
}

// 捕食者蓝色小球点击处理脚本
public class PredatorBlueSphereClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        // 使用反射增加研究点数
        try
        {
            var uiResearchPointType = System.Type.GetType("UI_ResearchPoint");
            if (uiResearchPointType != null)
            {
                var addResearchPointsMethod = uiResearchPointType.GetMethod("AddResearchPoints", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (addResearchPointsMethod != null)
                {
                    addResearchPointsMethod.Invoke(null, new object[] { 1 });
                    
                    // 获取当前研究点数
                    var researchPointsProperty = uiResearchPointType.GetProperty("ResearchPoints", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (researchPointsProperty != null)
                    {
                        int currentPoints = (int)researchPointsProperty.GetValue(null);
                        Debug.Log($"研究点数+1，当前研究点数: {currentPoints}");
                    }
                    else
                    {
                        Debug.Log("研究点数+1");
                    }
                }
                else
                {
                    Debug.LogWarning("未找到AddResearchPoints方法");
                }
            }
            else
            {
                Debug.LogWarning("未找到UI_ResearchPoint类");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"增加研究点数失败: {e.Message}");
        }
        
        // 销毁小球
        Destroy(gameObject);
    }
}
