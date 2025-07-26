using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalItem : MonoBehaviour
{
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
        }

        // 初始化生命计时器
        lifeTimer = 0f;
        Debug.Log($"AnimalItem开始生命周期，将在{maxLifeTime}秒后死亡");
        Debug.Log($"动物数量: {totalAnimalCount}, 环境动物数量: {environmentalAnimalValue}");
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
        // 生命周期检查
        if (!isDying)
        {
            lifeTimer += Time.deltaTime;
            
            // 检查是否到达生命终点
            if (lifeTimer >= maxLifeTime)
            {
                StartCoroutine(Die());
                return; // 开始死亡过程后不执行其他逻辑
            }
        }
        
        // 新生幼体等待逻辑
        if (isWaiting && isNewborn)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
                
                // 等待结束后添加刚体，开始物理交互
                if (GetComponent<Rigidbody>() == null)
                {
                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.mass = 1f;
                }
                
                Debug.Log("新生幼年体等待结束，开始正常行为");
            }
            return; // 等待期间不执行其他逻辑
        }
        
        // 更新繁衍冷却时间
        if (reproductionCooldown > 0f)
        {
            reproductionCooldown -= Time.deltaTime;
        }
        
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyProcessedThisFrame = false;
        }
        
        // 按下2键在指定位置生成红色动物正方形
        if (Input.GetKeyDown(KeyCode.Alpha2) && !keyProcessedThisFrame)
        {
            keyProcessedThisFrame = true;
            SpawnAnimalAtPosition(new Vector3(-50, 50, 50));
        }
        
        // 向植物移动
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
            
            // 如果不饥饿且不在冷却期，尝试寻找配偶
            if (!isHungry && !isLookingForMate && reproductionCooldown <= 0f)
            {
                TryFindMate();
            }
        }
    }
    
    void TryFindMate()
    {
        // 只有成年体且有居住地且场景中至少有2个动物且不在繁衍冷却期才能繁衍
        if (!isAdult || !hasHabitat || totalAnimalCount < 2 || reproductionCooldown > 0f) return;
        
        // 查找所有其他成年动物
        AnimalItem[] allAnimals = FindObjectsOfType<AnimalItem>();
        AnimalItem suitableMate = null;
        float nearestDistance = float.MaxValue;
        
        foreach (AnimalItem animal in allAnimals)
        {
            // 排除自己，只找成年体且不饥饿且不在冷却期且有居住地的动物
            if (animal != null && animal != this && animal.isAdult && !animal.isHungry &&
                animal.reproductionCooldown <= 0f && animal.hasHabitat && animal.transform != null)
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
        
        // 计算移动方向
        Vector3 targetPosition = new Vector3(targetMate.position.x, transform.position.y, targetMate.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 移动
        transform.position += direction * moveSpeed * Time.deltaTime;
        
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

        // 确保配偶仍然符合所有条件（包括有居住地）
        if (mate != null && mate.isAdult && !mate.isHungry && !isHungry &&
            !mate.isReproducing && mate.reproductionCooldown <= 0f && mate.hasHabitat)
        {
            // 设置繁衍标志和冷却时间，防止重复触发
            isReproducing = true;
            mate.isReproducing = true;
            reproductionCooldown = cooldownDuration;
            mate.reproductionCooldown = cooldownDuration;

            // 在居住地内生成幼年体（优先选择自己的居住地）
            Vector3 reproductionPosition;
            if (myHabitat != null)
            {
                reproductionPosition = myHabitat.position + Vector3.up * 2f;
            }
            else if (mate.myHabitat != null)
            {
                reproductionPosition = mate.myHabitat.position + Vector3.up * 2f;
            }
            else
            {
                // 如果都没有居住地，在两个动物中间位置生成
                reproductionPosition = (transform.position + targetMate.position) / 2f;
                reproductionPosition.y += 2f;
            }

            SpawnOffspring(reproductionPosition);

            // 繁衍完成，双方结束寻找配偶状态
            CompleteMating();
            mate.CompleteMating();

            Debug.Log("AnimalItem在居住地内繁衍成功，生成了幼年体");
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
        isWandering = true;
        
        RestoreNormalColor();
    }
    
    void SpawnAnimalAtPosition(Vector3 position)
    {
        // 创建新的AnimalItem对象
        GameObject newAnimal = new GameObject("AnimalItem");
        newAnimal.transform.position = position;
        
        // 添加AnimalItem组件，这会自动设置为红色正方形
        newAnimal.AddComponent<AnimalItem>();
        
        Debug.Log($"在位置 {position} 生成了红色动物正方形");
    }
    

    
    void OnCollisionEnter(Collision collision)
    {
        // 简化地面检测：只要碰撞到任何物体就视为落地
        if (!hasGrounded)
        {
            hasGrounded = true;

            // 完全固定动物位置，移除刚体组件
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                DestroyImmediate(rb);
            }

            Debug.Log("AnimalItem落地，完全固定位置");

            // 2秒后开始寻找植物
            StartCoroutine(StartMovingToPlantAfterDelay());
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
    
    void FindNearestPlant()
    {
        // 查找所有PlantItem对象
        PlantItem[] allPlants = FindObjectsOfType<PlantItem>();
        
        if (allPlants.Length == 0)
        {
            targetPlant = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestPlant = null;
        
        foreach (PlantItem plant in allPlants)
        {
            float distance = Vector3.Distance(transform.position, plant.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlant = plant.transform;
            }
        }
        
        targetPlant = nearestPlant;
        Debug.Log($"找到最近的植物，距离: {nearestDistance:F2}");
    }
    
    void MoveTowardsPlant()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 targetPosition = new Vector3(targetPlant.position.x, transform.position.y, targetPlant.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 移动
        transform.position += direction * moveSpeed * Time.deltaTime;
        
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
        Debug.Log("AnimalItem开始成长为成年体");
        
        // 等待2秒成长时间
        yield return new WaitForSeconds(2f);
        
        // 设置为成年体
        isAdult = true;
        
        // 改变颜色为暗红色表示成年
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = new Color(0.5f, 0f, 0f, 1f); // 暗红色
        }
        
        Debug.Log("AnimalItem成长为成年体，开始游荡和寻找配偶");
        
        // 成年后开始游荡（这里会触发居住地创建逻辑）
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
        hungerTimer += Time.deltaTime;
        
        // 游荡15秒后重新饥饿
        if (hungerTimer >= hungerInterval && !isHungry)
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
                Debug.Log("AnimalItem未找到食物，继续游荡");
                // 如果没找到食物，继续游荡
                isWandering = true;
                isHungry = false;
                RestoreNormalColor(); // 恢复正常颜色
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
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 direction = (wanderTarget - transform.position).normalized;
        
        // 移动植物多颜色bug
        transform.position += direction * wanderSpeed * Time.deltaTime;
        
        // 检查是否接近目标点（距离小于2米时选择新目标）
        float distance = Vector3.Distance(transform.position, wanderTarget);
        if (distance < 2f)
        {
            ChooseNewWanderTarget();
        }
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
        // 动物被销毁时减少总数量
        totalAnimalCount--;
        UpdateEnvironmentalAnimalValue();
        
        // 如果这是最后一个动物且有共享居住地，清理居住地引用
        if (totalAnimalCount <= 0 && sharedHabitat != null)
        {
            if (sharedHabitat.gameObject != null)
            {
                Destroy(sharedHabitat.gameObject);
            }
            sharedHabitat = null;
            isCreatingHabitat = false; // 重置创建标志
            Debug.Log("最后一个动物被销毁，清理共享居住地");
        }
        
        Debug.Log($"动物被销毁，剩余动物数量: {totalAnimalCount}, 环境动物数量: {environmentalAnimalValue}");
    }

    void ChangeToHungryColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = new Color(1f, 0f, 0f, 1f); // 鲜红色
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
                // 成年体恢复为暗红色
                meshRenderer.material.color = new Color(0.5f, 0f, 0f, 1f);
            }
            else
            {
                // 幼年体恢复为正常红色
                meshRenderer.material.color = Color.red;
            }
            Debug.Log("AnimalItem恢复正常颜色");
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
