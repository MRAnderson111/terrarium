using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalItem : MonoBehaviour
{
    // 添加状态枚举，统一管理动物行为状态
    public enum AnimalState
    {
        Newborn,        // 新生等待
        Hungry,         // 饥饿寻食
        Wandering,      // 游荡
        SeekingMate,    // 寻找配偶
        GoingToSleep,   // 前往睡觉
        Sleeping,       // 睡觉中
        Dying           // 死亡中
    }
    
    [Header("动物状态")]
    [SerializeField] private AnimalState currentState = AnimalState.Newborn;

    private static bool keyProcessedThisFrame = false; // 静态变量控制每帧只处理一次
    private bool hasGrounded = false; // 是否已经接触地面
    private bool isMovingToPlant = false; // 是否正在向植物移动
    private Transform targetPlant = null; // 目标植物
    private float moveSpeed = 10f; // 移动速度
    private bool hasEaten = false; // 是否已经吃过植物
    private bool isAdult = false; // 是否为成年体
    
    // 新增游荡相关变量
    private bool isWandering = false;
    private Vector3 wanderTarget;
    private float wanderSpeed = 5f;
    private float wanderRadius = 15f;
    private float wanderTimer = 0f;
    private float wanderInterval = 3f; // 每3秒选择新的游荡目标
    
    // 新增饥饿状态相关变量
    private float hungerTimer = 0f;
    private float hungerInterval = 15f; // 游荡15秒后重新饥饿
    private bool isHungry = false;
    
    // 繁衍相关变量
    private bool isLookingForMate = false;
    private Transform targetMate = null;
    private float mateSearchRadius = 20f;
    private bool isReproducing = false; // 新增：防止重复繁衍的标志
    private float reproductionCooldown = 0f; // 繁衍冷却时间
    private float cooldownDuration = 2f; // 冷却持续时间

    // 新增等待相关变量
    private bool isWaiting = false; // 是否正在等待
    private float waitTimer = 0f; // 等待计时器
    private float waitDuration = 3f; // 等待时长
    private bool isNewborn = false; // 是否为新生幼体

    // 居住地相关变量
    private bool hasHabitat = false; // 是否已经建立居住地
    private Transform myHabitat = null; // 自己的居住地
    private bool isBuildingHabitat = false; // 是否正在建立居住地
    private bool isInHabitat = false; // 是否在居住地内

    // 静态变量记录所有动物数量和共享居住地
    public static int totalAnimalCount = 0;
    public static int environmentalAnimalValue = 0; // 环境动物数量（动物数量的两倍）
    public static Transform sharedHabitat = null; // 共享的居住地
    private static bool isCreatingHabitat = false; // 静态标志，防止同时创建多个居住地

    // 生命周期相关变量
    private float lifeTimer = 0f;
    private float maxLifeTime = 30f; // 最大生存时间30秒
    private bool isDying = false; // 是否正在死亡过程中
    
    // 新增环境适应性相关变量
    [Header("动物环境适应性")]
    [SerializeField] private float optimalTemperatureMin = 10f; // 适宜温度下限
    [SerializeField] private float optimalTemperatureMax = 35f; // 适宜温度上限
    [SerializeField] private float optimalHumidityMin = 30f;    // 适宜湿度下限
    [SerializeField] private float optimalHumidityMax = 90f;    // 适宜湿度上限
    
    // 环境适应性状态
    private bool isInOptimalEnvironment = true; // 是否处于适宜环境
    private float environmentalStress = 0f;     // 环境压力值 (0-1)
    private float extremeStressTimer = 0f;      // 极端环境压力计时器
    private bool isDyingFromEnvironment = false; // 是否因环境因素死亡
    
    [Header("环境死亡设置")]
    [SerializeField] private float extremeStressThreshold = 0.85f;  // 极端压力阈值
    [SerializeField] private float environmentalDeathTimeLimit = 15f; // 极端环境下存活时间限制
    
    // 环境影响的速度修正系数
    private float environmentalSpeedModifier = 1f;
    private float environmentalReproductionModifier = 1f;

    // 睡觉相关变量
    private bool isSleeping = false; // 是否正在睡觉
    private bool isGoingToSleep = false; // 是否正在前往睡觉地点
    private float sleepTimer = 0f; // 睡觉计时器
    private float sleepDuration = 5f; // 睡觉持续时间
    private float sleepInterval = 15f; // 睡觉间隔时间（成年后15秒）
    private float timeSinceAdult = 0f; // 成年后的时间计数

    void Start()
    {
        // 动物生成时增加总数量
        totalAnimalCount++;
        UpdateEnvironmentalAnimalValue();
        
        // 检查是否为新生幼体（通过名称判断）
        if (gameObject.name.Contains("Offspring"))
        {
            isNewborn = true;
            isWaiting = true;
            waitTimer = 0f;
            Debug.Log("新生幼年体开始等待3秒");
        }
        
        // 初始化组件...
        InitializeComponents();
        
        // 初始化生命计时器
        lifeTimer = 0f;
        Debug.Log($"AnimalItem开始生命周期，将在{maxLifeTime}秒后死亡");
        Debug.Log($"动物数量: {totalAnimalCount}, 环境动物数量: {environmentalAnimalValue}");
        
        // 开始环境监测
        StartCoroutine(MonitorEnvironmentalConditions());
    }
    
    void InitializeComponents()
    {
        // 确保有MeshRenderer和MeshFilter组件
        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }
        
        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
        }
        
        // 设置为立方体网格
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建红色材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material animalMaterial = new Material(Shader.Find("Standard"));
        animalMaterial.color = Color.red;
        meshRenderer.material = animalMaterial;
        
        // 添加碰撞器
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        
        // 只有非新生幼体才添加刚体（新生幼体等待期间不需要物理交互）
        if (!isNewborn && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            // 初始时不设置为运动学模式，让动物自然下落
        }
    }
    
    Mesh CreateCubeMesh()
    {
        // 使用Unity内置的立方体网格
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }

    void Update()
    {
        // 处理按键输入
        HandleKeyInput();

        // 生命周期检查
        if (!isDying && !isDyingFromEnvironment)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= maxLifeTime)
            {
                StartCoroutine(Die());
                return;
            }
        }

        // 如果正在死亡过程中，不执行其他逻辑
        if (isDying || isDyingFromEnvironment) return;

        // 新生幼体等待逻辑
        if (isNewborn && isWaiting)
        {
            HandleNewbornWaiting();
            return;
        }

        // 更新繁衍冷却时间
        if (reproductionCooldown > 0f)
        {
            reproductionCooldown -= Time.deltaTime;
        }

        // 核心行为逻辑
        HandleCoreBehavior();
    }

    void HandleNewbornWaiting()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitDuration)
        {
            // 等待结束，添加刚体让新生动物自然下落
            isWaiting = false;
            isNewborn = false;

            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = 1f;
                // 不设置为运动学模式，让新生动物自然下落
            }

            Debug.Log("新生幼年体等待结束，开始自然下落");
        }
    }

    void HandleCoreBehavior()
    {
        // 只有落地后才能执行移动行为
        if (!hasGrounded)
        {
            return; // 还没落地，不执行任何移动逻辑
        }

        // 睡觉行为（成年体专有）
        if (isAdult && isSleeping)
        {
            HandleSleeping();
            return;
        }

        // 前往睡觉
        if (isAdult && isGoingToSleep)
        {
            MoveToHabitat();
            return;
        }

        // 向植物移动（饥饿状态）
        if (isMovingToPlant && targetPlant != null)
        {
            MoveTowardsPlant();
        }
        // 如果目标植物被销毁且还是幼年体，寻找新目标
        else if (isMovingToPlant && targetPlant == null && !isAdult)
        {
            FindNearestPlant();
            if (targetPlant != null)
            {
                Debug.Log($"AnimalItem目标植物已消失，找到新目标: {targetPlant.name}");
            }
            else
            {
                isMovingToPlant = false;
                Debug.Log("AnimalItem未找到新的植物目标，停止移动");
            }
        }
        // 寻找配偶进行繁衍
        else if (isLookingForMate && targetMate != null)
        {
            MoveTowardsMate();
        }
        // 成年动物游荡行为
        else if (isWandering && isAdult)
        {
            HandleWandering();
            HandleHunger();
            HandleSleepBehavior();

            // 如果不饥饿且不在冷却期，尝试寻找配偶
            if (!isHungry && !isLookingForMate && reproductionCooldown <= 0f)
            {
                TryFindMate();
            }
        }
    }
    
    void HandleKeyInput()
    {
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyProcessedThisFrame = false;
        }
        
        // 按下2键在指定位置生成红色动物正方形
        if (Input.GetKeyDown(KeyCode.Alpha2) && !keyProcessedThisFrame)
        {
            keyProcessedThisFrame = true;
            SpawnAnimalAtPosition(new Vector3(-25, 15, -25));
            Debug.Log("按下2键，尝试生成动物");
        }
    }
    
    static void SpawnAnimalAtPosition(Vector3 position)
    {
        // 创建新的AnimalItem对象
        GameObject newAnimal = new GameObject("AnimalItem");
        newAnimal.transform.position = position;
        
        // 添加AnimalItem组件
        AnimalItem animalComponent = newAnimal.AddComponent<AnimalItem>();
        
        Debug.Log($"在位置 {position} 生成了红色动物正方形");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 改进的地面检测：检查碰撞是否来自下方
        if (!hasGrounded)
        {
            // 检查碰撞点是否在动物下方
            bool isGroundCollision = false;

            foreach (ContactPoint contact in collision.contacts)
            {
                // 如果碰撞点的法线向上（Y分量为正），说明是地面碰撞
                if (contact.normal.y > 0.5f)
                {
                    isGroundCollision = true;
                    break;
                }
            }

            // 也可以通过碰撞对象的标签或名称来判断
            if (!isGroundCollision)
            {
                GameObject collisionObject = collision.gameObject;
                isGroundCollision = collisionObject.CompareTag("Ground") ||
                                  collisionObject.name.Contains("Ground") ||
                                  collisionObject.name.Contains("Plane") ||
                                  collisionObject.name.Contains("Terrain");
            }

            if (isGroundCollision)
            {
                hasGrounded = true;

                // 固定动物位置，但保留刚体用于移动
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 停止物理运动，但保留刚体
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // 设置为运动学模式，不受物理影响但可以移动
                }

                Debug.Log($"AnimalItem落地在 {collision.gameObject.name}，固定位置但保留移动能力");

                // 2秒后开始寻找植物
                StartCoroutine(StartMovingToPlantAfterDelay());
            }
            else
            {
                Debug.Log($"AnimalItem碰撞到 {collision.gameObject.name}，但不是地面，继续下落");
            }
        }
    }

    System.Collections.IEnumerator StartMovingToPlantAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);

        // 寻找最近的植物
        FindNearestPlant();

        if (targetPlant != null)
        {
            isMovingToPlant = true;
            Debug.Log($"AnimalItem开始向最近的植物移动: {targetPlant.name}");
        }
        else
        {
            Debug.Log("AnimalItem未找到附近的植物");
        }
    }
    
    // 缓存搜索结果，避免频繁搜索
    private static PlantItem[] cachedPlants;
    private static float lastPlantCacheTime;
    private static readonly float cacheValidTime = 1f; // 缓存1秒有效
    
    void FindNearestPlant()
    {
        // 使用缓存的植物列表
        if (cachedPlants == null || Time.time - lastPlantCacheTime > cacheValidTime)
        {
            cachedPlants = FindObjectsOfType<PlantItem>();
            lastPlantCacheTime = Time.time;
        }
        
        if (cachedPlants.Length == 0)
        {
            targetPlant = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestPlant = null;
        
        foreach (PlantItem plant in cachedPlants)
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
        }
    }
    
    void MoveTowardsPlant()
    {
        // 安全检查：确保已经落地
        if (!hasGrounded)
        {
            Debug.LogWarning("AnimalItem尝试移动但还没有落地，停止移动");
            isMovingToPlant = false;
            return;
        }

        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 targetPosition = new Vector3(targetPlant.position.x, transform.position.y, targetPlant.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 应用环境影响的移动速度
        float adjustedMoveSpeed = moveSpeed * environmentalSpeedModifier;
        transform.position += direction * adjustedMoveSpeed * Time.deltaTime;

        // 检查是否到达植物附近（距离小于3米）
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 3f)
        {
            isMovingToPlant = false;

            // 幼年体：如果还没有吃过植物，则吃掉植物并成长
            if (!isAdult && !hasEaten)
            {
                EatPlant();
            }
            // 成年体：如果处于饥饿状态，则吃掉植物
            else if (isAdult && isHungry)
            {
                EatPlant();
            }

            Debug.Log("AnimalItem到达植物附近，停止移动");
        }
    }
    
    void EatPlant()
    {
        if (targetPlant != null)
        {
            // 销毁植物
            PlantItem plantComponent = targetPlant.GetComponent<PlantItem>();
            if (plantComponent != null)
            {
                Debug.Log($"AnimalItem吃掉了植物: {targetPlant.name}");
                Destroy(targetPlant.gameObject);
            }
            
            // 如果是幼年体，成长为成年体
            if (!isAdult)
            {
                hasEaten = true; // 只有幼年体需要设置hasEaten标记
                Debug.Log("幼年体AnimalItem吃掉植物，开始成长为成年体");
                StartCoroutine(GrowToAdult());
            }
            else
            {
                // 成年体进食后进入吃饱状态（不饥饿）
                isHungry = false;
                hungerTimer = 0f;
                isWandering = true;
                
                // 恢复正常颜色
                RestoreNormalColor();
                
                // 60%几率生成蓝色小球
                if (Random.Range(0f, 1f) < 0.6f)
                {
                    SpawnBlueSphere();
                }
                
                // 成年体吃饱后也应该调用StartWandering来检查居住地
                StartWandering();
                
                Debug.Log("成年AnimalItem进食完毕，进入吃饱状态，可以繁衍");
            }
        }
    }
    
    System.Collections.IEnumerator GrowToAdult()
    {
        if (isAdult) yield break;
        
        isAdult = true;
        timeSinceAdult = 0f; // 重置成年计时器
        
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material animalMaterial = meshRenderer.material;
        Color originalColor = animalMaterial.color;
        Color targetColor = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
        
        float growthTime = 1f;
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
        
        Debug.Log("AnimalItem成长为成年体：体型变大，颜色变暗，开始睡觉周期");
        
        // 成长完成后开始游荡
        StartWandering();
    }
    
    void StartWandering()
    {
        isWandering = true;
        isHungry = false;
        hungerTimer = 0f;
        
        // 恢复正常颜色
        RestoreNormalColor();
        
        // 成年体吃饱后优先建立或寻找居住地
        if (isAdult && !hasHabitat && !isBuildingHabitat)
        {
            // 检查是否已经有共享居住地
            if (sharedHabitat != null)
            {
                // 使用现有的共享居住地
                myHabitat = sharedHabitat;
                hasHabitat = true;
                Debug.Log("AnimalItem使用现有的共享居住地");
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
        
        if (!isBuildingHabitat)
        {
            ChooseNewWanderTarget();
        }
        
        Debug.Log("AnimalItem开始游荡行为");
    }
    
    System.Collections.IEnumerator WaitForHabitatCreation()
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
            Debug.Log("AnimalItem等待后使用了共享居住地");
        }
        else if (!isCreatingHabitat)
        {
            // 如果没有居住地且没有其他动物在创建，自己创建
            StartCoroutine(BuildHabitatAfterDelay());
        }
    }
    
    System.Collections.IEnumerator BuildHabitatAfterDelay()
    {
        // 设置创建标志，防止其他动物同时创建
        if (isCreatingHabitat)
        {
            yield break; // 如果已经有动物在创建，直接退出
        }
        
        isCreatingHabitat = true;
        isBuildingHabitat = true;
        
        // 等待2秒后建立居住地
        yield return new WaitForSeconds(2f);
        
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
            Debug.Log("AnimalItem在等待期间发现了共享居住地，直接使用");
        }
        
        isCreatingHabitat = false;
        isBuildingHabitat = false;
        ChooseNewWanderTarget();
    }
    
    void BuildHabitat()
    {
        // 最后一次检查，确保没有重复创建
        if (sharedHabitat != null)
        {
            myHabitat = sharedHabitat;
            hasHabitat = true;
            Debug.Log("AnimalItem发现居住地已存在，直接使用");
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
                
                Debug.Log($"AnimalItem在最佳位置 {bestPosition} 建立了共享居住地");
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
    
    Vector3 FindBestHabitatPosition()
    {
        Vector3 bestPosition = transform.position;
        float bestScore = float.MinValue;
        
        // 在当前位置周围搜索多个候选位置
        int searchAttempts = 20;
        float searchRadius = 30f;
        
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
    
    float EvaluateHabitatPosition(Vector3 position)
    {
        float score = 0f;
        
        // 1. 靠近植物（食物）加分
        float foodScore = CalculateFoodProximityScore(position);
        score += foodScore * 3f; // 食物权重为3
        
        // 2. 靠近水源加分（基于环境湿度和水源数据）
        float waterScore = CalculateWaterProximityScore(position);
        score += waterScore * 2f; // 水源权重为2
        
        // 3. 远离捕食者加分
        float predatorScore = CalculatePredatorDistanceScore(position);
        score += predatorScore * 4f; // 远离捕食者权重为4（最重要）
        
        return score;
    }
    
    float CalculateFoodProximityScore(Vector3 position)
    {
        // 查找所有植物
        PlantItem[] allPlants = FindObjectsOfType<PlantItem>();
        
        if (allPlants.Length == 0)
            return 0f;
        
        float totalScore = 0f;
        int plantCount = 0;
        
        foreach (PlantItem plant in allPlants)
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
    
    float CalculateWaterProximityScore(Vector3 position)
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
    
    float CalculatePredatorDistanceScore(Vector3 position)
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
    
    void HandleWandering()
    {
        wanderTimer += Time.deltaTime;
        
        // 每隔一定时间选择新的游荡目标
        if (wanderTimer >= wanderInterval)
        {
            ChooseNewWanderTarget();
            wanderTimer = 0f;
        }
        
        // 向游荡目标移动
        MoveTowardsWanderTarget();
    }
    
    void HandleHunger()
    {
        // 如果当前处于饥饿状态，不执行饥饿计时逻辑
        if (isHungry) return;
        
        hungerTimer += Time.deltaTime;
        
        // 游荡15秒后重新饥饿
        if (hungerTimer >= hungerInterval)
        {
            isHungry = true;
            isWandering = false; // 停止游荡
            isLookingForMate = false; // 停止寻找配偶
            targetMate = null;
            hungerTimer = 0f; // 重置计时器
            
            // 变成鲜红色表示饥饿状态
            ChangeToHungryColor();
            
            Debug.Log("AnimalItem重新进入饥饿状态，开始寻找食物");
            
            // 寻找最近的植物
            FindNearestPlant();
            if (targetPlant != null)
            {
                isMovingToPlant = true;
                Debug.Log($"AnimalItem找到新的食物目标: {targetPlant.name}");
            }
            else
            {
                Debug.Log("AnimalItem未找到食物，保持饥饿状态");
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
        
        Debug.Log($"AnimalItem选择新的游荡目标: {wanderTarget}");
    }
    
    void MoveTowardsWanderTarget()
    {
        // 安全检查：确保已经落地
        if (!hasGrounded)
        {
            Debug.LogWarning("AnimalItem尝试游荡但还没有落地，停止游荡");
            isWandering = false;
            return;
        }

        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 direction = (wanderTarget - transform.position).normalized;

        // 应用环境影响的游荡速度
        float adjustedWanderSpeed = wanderSpeed * environmentalSpeedModifier;
        transform.position += direction * adjustedWanderSpeed * Time.deltaTime;

        // 检查是否接近目标点（距离小于2米时选择新目标）
        float distance = Vector3.Distance(transform.position, wanderTarget);
        if (distance < 2f)
        {
            ChooseNewWanderTarget();
        }
    }

    void TryFindMate()
    {
        // 只有成年体且场景中至少有2个动物且不在繁衍冷却期才能繁衍
        if (!isAdult || totalAnimalCount < 2 || reproductionCooldown > 0f) return;

        // 查找所有其他成年动物
        AnimalItem[] allAnimals = FindObjectsOfType<AnimalItem>();
        AnimalItem suitableMate = null;
        float nearestDistance = float.MaxValue;

        foreach (AnimalItem animal in allAnimals)
        {
            // 排除自己，只找成年体且不饥饿且不在冷却期的动物
            if (animal != null && animal != this && animal.isAdult && !animal.isHungry &&
                animal.reproductionCooldown <= 0f && animal.transform != null)
            {
                float distance = Vector3.Distance(transform.position, animal.transform.position);
                if (distance <= mateSearchRadius && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    suitableMate = animal;
                }
            }
        }

        if (suitableMate != null && suitableMate.transform != null)
        {
            targetMate = suitableMate.transform;
            isLookingForMate = true;
            isWandering = false;
            Debug.Log($"AnimalItem找到配偶，开始接近进行繁衍，距离: {nearestDistance:F2}");
        }
    }

    void MoveTowardsMate()
    {
        if (targetMate == null) return;

        // 安全检查：确保已经落地
        if (!hasGrounded)
        {
            Debug.LogWarning("AnimalItem尝试寻找配偶但还没有落地，停止寻找配偶");
            isLookingForMate = false;
            targetMate = null;
            return;
        }

        // 计算移动方向
        Vector3 targetPosition = new Vector3(targetMate.position.x, transform.position.y, targetMate.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 移动
        float adjustedMoveSpeed = moveSpeed * environmentalSpeedModifier;
        transform.position += direction * adjustedMoveSpeed * Time.deltaTime;

        // 检查是否到达配偶附近（距离小于2米）
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 2f)
        {
            // 尝试繁衍
            AttemptReproduction();
        }
    }

    void AttemptReproduction()
    {
        // 防止重复繁衍 - 多重检查
        if (isReproducing || reproductionCooldown > 0f) return;

        // 检查targetMate是否为空
        if (targetMate == null)
        {
            Debug.Log("targetMate为空，停止繁衍尝试");
            isLookingForMate = false;
            isWandering = true;
            return;
        }

        AnimalItem mate = targetMate.GetComponent<AnimalItem>();

        // 确保配偶仍然符合所有条件
        if (mate != null && mate.isAdult && !mate.isHungry && !isHungry &&
            !mate.isReproducing && mate.reproductionCooldown <= 0f)
        {
            // 应用环境影响的繁衍成功率
            float reproductionChance = 0.8f * environmentalReproductionModifier;
            if (Random.Range(0f, 1f) > reproductionChance)
            {
                Debug.Log($"繁衍失败，环境影响成功率: {reproductionChance:F2}");
                // 繁衍失败时不消耗体力，只是结束寻找配偶状态
                FailedMating();
                mate.FailedMating();
                return;
            }

            // 设置繁衍标志和冷却时间，防止重复触发
            isReproducing = true;
            mate.isReproducing = true;
            reproductionCooldown = cooldownDuration;
            mate.reproductionCooldown = cooldownDuration;

            // 在两个动物中间位置生成幼年体
            Vector3 reproductionPosition = (transform.position + targetMate.position) / 2f;
            reproductionPosition.y += 2f;

            SpawnOffspring(reproductionPosition);

            // 繁衍完成，双方结束寻找配偶状态
            CompleteMating();
            mate.CompleteMating();

            Debug.Log("AnimalItem繁衍成功，生成了幼年体");
        }
        else
        {
            // 配偶状态不符合，停止寻找配偶
            isLookingForMate = false;
            targetMate = null;
            isWandering = true;
            Debug.Log("配偶状态不符合繁衍条件，停止寻找配偶");
        }
    }

    void SpawnOffspring(Vector3 position)
    {
        // 创建新的幼年体AnimalItem
        GameObject offspring = new GameObject("AnimalItem_Offspring");
        offspring.transform.position = position;

        // 添加AnimalItem组件
        AnimalItem offspringComponent = offspring.AddComponent<AnimalItem>();

        Debug.Log($"在位置 {position} 繁衍了新的幼年体动物");
    }

    void CompleteMating()
    {
        isLookingForMate = false;
        targetMate = null;
        isReproducing = false;

        // 繁衍消耗体力，动物需要重新进食
        isHungry = true;
        isWandering = false; // 停止游荡，开始寻找食物
        hungerTimer = 0f; // 重置饥饿计时器

        // 变成鲜红色表示饥饿状态
        ChangeToHungryColor();

        Debug.Log("AnimalItem繁衍完成，消耗体力，需要重新进食");

        // 立即寻找最近的植物
        FindNearestPlant();
        if (targetPlant != null)
        {
            isMovingToPlant = true;
            Debug.Log($"AnimalItem繁衍后寻找食物目标: {targetPlant.name}");
        }
        else
        {
            Debug.Log("AnimalItem繁衍后未找到食物，保持饥饿状态");
        }
    }

    void FailedMating()
    {
        isLookingForMate = false;
        targetMate = null;
        isReproducing = false;

        // 繁衍失败不消耗体力，继续游荡
        isWandering = true;
        // 不重置hungerTimer，保持原有的饥饿进度

        // 恢复正常颜色
        RestoreNormalColor();

        Debug.Log("AnimalItem繁衍失败，继续游荡");
    }
    
    void SpawnBlueSphere()
    {
        // 在animal正上方3米处生成蓝色小球
        Vector3 spherePosition = transform.position + Vector3.up * 3f;
        
        // 创建球体对象
        GameObject blueSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        blueSphere.name = "BlueSphere";
        blueSphere.transform.position = spherePosition;
        blueSphere.transform.localScale = Vector3.one * 0.3f;
        
        // 设置蓝色材质
        MeshRenderer sphereRenderer = blueSphere.GetComponent<MeshRenderer>();
        Material blueMaterial = new Material(Shader.Find("Standard"));
        blueMaterial.color = Color.blue;
        sphereRenderer.material = blueMaterial;
        
        // 移除刚体组件（保留碰撞器用于点击检测）
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
            Destroy(sphereRb); // 改为Destroy
        
        // 保留碰撞器但设置为触发器
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
            sphereCollider.isTrigger = true;
        
        // 添加点击脚本
        blueSphere.AddComponent<AnimalBlueSphereClickHandler>();
        
        Debug.Log($"在位置 {spherePosition} 生成了浮空蓝色小球");
    }
    
    void OnDestroy()
    {
        // 停止所有协程，防止内存泄漏
        StopAllCoroutines();
        
        // 清理引用
        targetPlant = null;
        targetMate = null;
        myHabitat = null;
        
        // 如果是共享居住地的最后一个动物，清理共享居住地
        if (sharedHabitat != null && totalAnimalCount <= 1)
        {
            if (sharedHabitat.gameObject != null)
                Destroy(sharedHabitat.gameObject);
            sharedHabitat = null;
        }
        
        // 减少总数量
        totalAnimalCount--;
        UpdateEnvironmentalAnimalValue();
        
        Debug.Log($"动物被销毁，剩余动物数量: {totalAnimalCount}, 环境动物数值: {environmentalAnimalValue}");
    }

    void ChangeToHungryColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            if (isAdult)
            {
                // 成年体饥饿时变成鲜红色
                meshRenderer.material.color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                // 幼年体饥饿时也是鲜红色
                meshRenderer.material.color = new Color(1f, 0f, 0f, 1f);
            }
            Debug.Log("AnimalItem变成鲜红色，表示饥饿状态");
        }
    }

    void RestoreNormalColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            if (isAdult)
            {
                // 成年体恢复为暗红色（保持大体型）
                meshRenderer.material.color = new Color(0.5f, 0f, 0f, 1f);
                Debug.Log("成年体AnimalItem恢复暗红色");
            }
            else
            {
                // 幼年体恢复为正常红色（保持小体型）
                meshRenderer.material.color = Color.red;
                Debug.Log("幼年体AnimalItem恢复红色");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否进入了自己的居住地
        if (myHabitat != null && other.transform == myHabitat)
        {
            isInHabitat = true;
            Debug.Log("AnimalItem进入了自己的居住地");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // 检查是否离开了自己的居住地
        if (myHabitat != null && other.transform == myHabitat)
        {
            isInHabitat = false;
            Debug.Log("AnimalItem离开了自己的居住地");
        }
    }

    System.Collections.IEnumerator Die()
    {
        if (isDying) yield break; // 防止重复执行死亡过程
        
        isDying = true;
        
        Debug.Log($"AnimalItem开始死亡过程，存活时间: {lifeTimer:F1}秒");
        
        // 停止所有行为
        isMovingToPlant = false;
        isWandering = false;
        isLookingForMate = false;
        targetPlant = null;
        targetMate = null;
        
        // 获取渲染器和材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material animalMaterial = meshRenderer.material;
            Color originalColor = animalMaterial.color;
            
            // 1秒内逐渐变透明（消散效果）
            float fadeTime = 1f;
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
        
        Debug.Log("AnimalItem死亡消散完成");
        
        // 销毁对象
        Destroy(gameObject);
    }

    static void UpdateEnvironmentalAnimalValue()
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

    IEnumerator MonitorEnvironmentalConditions()
    {
        while (gameObject != null && !isDying && !isDyingFromEnvironment)
        {
            // 每3秒检测一次环境条件
            yield return new WaitForSeconds(3f);
            
            CheckEnvironmentalConditions();
        }
    }
    
    void CheckEnvironmentalConditions()
    {
        // 如果已经在死亡过程中，不再检查环境条件
        if (isDying || isDyingFromEnvironment) return;
        
        // 获取当前环境数据
        float currentTemperature = GetCurrentTemperature();
        float currentHumidity = GetCurrentHumidity();
        
        // 计算环境适应性
        float tempStress = CalculateTemperatureStress(currentTemperature);
        float humidityStress = CalculateHumidityStress(currentHumidity);
        
        // 综合环境压力
        environmentalStress = (tempStress + humidityStress) / 2f;
        
        // 判断是否处于适宜环境
        bool wasOptimal = isInOptimalEnvironment;
        isInOptimalEnvironment = environmentalStress < 0.3f; // 压力小于30%认为是适宜环境
        
        // 计算环境影响修正系数
        CalculateEnvironmentalModifiers();
        
        // 如果环境状态发生变化，调整动物状态
        if (wasOptimal != isInOptimalEnvironment)
        {
            OnEnvironmentalStatusChanged();
        }
        
        // 检查极端环境压力
        if (environmentalStress >= extremeStressThreshold)
        {
            extremeStressTimer += 3f; // 每次检测间隔3秒
            
            if (extremeStressTimer >= environmentalDeathTimeLimit)
            {
                Debug.Log($"动物在极端环境下存活{environmentalDeathTimeLimit}秒后开始死亡");
                StartCoroutine(DieFromEnvironment());
                return; // 开始死亡后不再执行后续逻辑
            }
            else
            {
                Debug.Log($"动物承受极端环境压力 {extremeStressTimer:F1}/{environmentalDeathTimeLimit}秒");
            }
        }
        else
        {
            // 环境改善时重置计时器
            if (extremeStressTimer > 0f)
            {
                extremeStressTimer = 0f;
                Debug.Log("环境条件改善，动物脱离极端压力状态");
            }
        }
        
        Debug.Log($"动物环境检测 - 温度: {currentTemperature:F1}°C, 湿度: {currentHumidity:F1}%, 环境压力: {environmentalStress:F2}");
    }
    
    // 缓存环境数据，避免重复反射调用
    private static System.Type envFactoryType;
    private static System.Reflection.PropertyInfo temperatureProperty;
    private static System.Reflection.PropertyInfo humidityProperty;
    
    static AnimalItem()
    {
        try
        {
            envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                temperatureProperty = envFactoryType.GetProperty("Temperature");
                humidityProperty = envFactoryType.GetProperty("Humidity");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"初始化环境数据访问失败: {e.Message}");
        }
    }
    
    float GetCurrentTemperature()
    {
        try
        {
            if (temperatureProperty != null)
                return (float)temperatureProperty.GetValue(null);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取温度数据失败: {e.Message}");
        }
        return 25f; // 默认温度
    }
    
    float GetCurrentHumidity()
    {
        try
        {
            if (humidityProperty != null)
                return (float)humidityProperty.GetValue(null);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取湿度数据失败: {e.Message}");
        }
        return 60f; // 默认湿度
    }
    
    float CalculateTemperatureStress(float temperature)
    {
        if (temperature >= optimalTemperatureMin && temperature <= optimalTemperatureMax)
            return 0f; // 适宜温度，无压力
        
        if (temperature < optimalTemperatureMin)
        {
            // 温度过低
            float deviation = optimalTemperatureMin - temperature;
            return Mathf.Clamp01(deviation / 20f); // 偏离20度为最大压力
        }
        else
        {
            // 温度过高
            float deviation = temperature - optimalTemperatureMax;
            return Mathf.Clamp01(deviation / 20f); // 偏离20度为最大压力
        }
    }
    
    float CalculateHumidityStress(float humidity)
    {
        if (humidity >= optimalHumidityMin && humidity <= optimalHumidityMax)
            return 0f; // 适宜湿度，无压力
        
        if (humidity < optimalHumidityMin)
        {
            // 湿度过低
            float deviation = optimalHumidityMin - humidity;
            return Mathf.Clamp01(deviation / 40f); // 偏离40%为最大压力
        }
        else
        {
            // 湿度过高
            float deviation = humidity - optimalHumidityMax;
            return Mathf.Clamp01(deviation / 40f); // 偏离40%为最大压力
        }
    }
    
    void CalculateEnvironmentalModifiers()
    {
        // 根据环境压力计算速度和繁衍修正系数
        environmentalSpeedModifier = Mathf.Lerp(1f, 0.3f, environmentalStress); // 最低降到30%
        environmentalReproductionModifier = Mathf.Lerp(1f, 0.1f, environmentalStress); // 最低降到10%
        
        Debug.Log($"环境修正系数 - 速度: {environmentalSpeedModifier:F2}, 繁衍: {environmentalReproductionModifier:F2}");
    }
    
    void OnEnvironmentalStatusChanged()
    {
        if (isInOptimalEnvironment)
        {
            Debug.Log("动物进入适宜环境，状态良好");
            // 恢复正常颜色
            RestoreNormalColor();
        }
        else
        {
            Debug.Log("动物离开适宜环境，开始承受环境压力");
            // 颜色变暗表示压力
            ChangeToStressedColor();
        }
    }
    
    void ChangeToStressedColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            Color currentColor = meshRenderer.material.color;
            float stressFactor = 1f - (environmentalStress * 0.4f);
            meshRenderer.material.color = new Color(currentColor.r * stressFactor, currentColor.g * stressFactor, currentColor.b * stressFactor, currentColor.a);
            Debug.Log("动物因环境压力颜色变暗");
        }
    }
    
    IEnumerator DieFromEnvironment()
    {
        if (isDyingFromEnvironment) yield break; // 防止重复执行死亡过程
        
        isDyingFromEnvironment = true;
        
        Debug.Log($"动物开始因环境因素死亡，承受压力时间: {extremeStressTimer:F1}秒");
        
        // 停止所有行为
        isMovingToPlant = false;
        isWandering = false;
        isLookingForMate = false;
        targetPlant = null;
        targetMate = null;
        
        // 获取渲染器和材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material animalMaterial = meshRenderer.material;
            Color originalColor = animalMaterial.color;
            
            // 2秒内逐渐变透明（环境死亡比自然死亡更快）
            float fadeTime = 2f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeTime);
                
                // 设置透明度，同时颜色变成灰色表示环境死亡
                Color newColor = new Color(0.5f, 0.5f, 0.5f, alpha);
                animalMaterial.color = newColor;
                
                yield return null;
            }
        }
        
        Debug.Log("动物因极端环境死亡");
        
        // 销毁对象
        Destroy(gameObject);
    }
    // 睡觉行为相关方法
    void HandleSleepBehavior()
    {
        timeSinceAdult += Time.deltaTime;
        
        if (timeSinceAdult >= sleepInterval && !isSleeping && !isGoingToSleep && 
            !isHungry && !isMovingToPlant && !isLookingForMate && hasHabitat)
        {
            Debug.Log("AnimalItem需要睡觉，开始前往居住地");
            StartGoingToSleep();
        }
    }
    
    void StartGoingToSleep()
    {
        isGoingToSleep = true;
        isWandering = false;
        isLookingForMate = false;
        targetMate = null;
        Debug.Log("AnimalItem开始前往居住地睡觉");
    }
    
    void MoveToHabitat()
    {
        if (myHabitat == null)
        {
            Debug.Log("AnimalItem没有居住地，取消睡觉");
            isGoingToSleep = false;
            isWandering = true;
            return;
        }

        // 安全检查：确保已经落地
        if (!hasGrounded)
        {
            Debug.LogWarning("AnimalItem尝试前往居住地但还没有落地，取消睡觉");
            isGoingToSleep = false;
            isWandering = true;
            return;
        }

        Vector3 targetPosition = new Vector3(myHabitat.position.x, transform.position.y, myHabitat.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        float adjustedMoveSpeed = moveSpeed * environmentalSpeedModifier;
        transform.position += direction * adjustedMoveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 3f)
        {
            StartSleeping();
        }
    }
    
    void StartSleeping()
    {
        isGoingToSleep = false;
        isSleeping = true;
        sleepTimer = 0f;
        timeSinceAdult = 0f;
        
        ChangeToSleepingColor();
        Debug.Log("AnimalItem开始睡觉，持续5秒");
    }
    
    void HandleSleeping()
    {
        sleepTimer += Time.deltaTime;
        
        if (sleepTimer >= sleepDuration)
        {
            WakeUp();
        }
    }
    
    void WakeUp()
    {
        isSleeping = false;
        sleepTimer = 0f;
        
        // 恢复正常颜色
        RestoreNormalColor();
        
        // 醒来后开始游荡
        isWandering = true;
        ChooseNewWanderTarget();
        
        Debug.Log("AnimalItem睡醒了，开始游荡");
    }
    
    void ChangeToSleepingColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            if (isAdult)
            {
                // 成年体睡觉时变成更暗的红色
                meshRenderer.material.color = new Color(0.3f, 0f, 0f, 1f);
            }
            Debug.Log("AnimalItem变成深暗红色，表示睡觉状态");
        }
    }
}

// 动物蓝色小球点击处理脚本
public class AnimalBlueSphereClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        // 增加研究点数
        UI_ResearchPoint.AddResearchPoints(1);
        
        Debug.Log($"研究点数+1，当前研究点数: {UI_ResearchPoint.ResearchPoints}");
        
        // 销毁小球
        Destroy(gameObject);
    }
}
