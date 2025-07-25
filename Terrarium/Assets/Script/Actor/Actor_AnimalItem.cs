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
    private float hungerInterval = 5f; // 游荡5秒后重新饥饿
    private bool isHungry = false;
    
    // 繁衍相关变量
    private bool isLookingForMate = false;
    private Transform targetMate = null;
    private float mateSearchRadius = 20f;
    private bool isReproducing = false; // 新增：防止重复繁衍的标志
    private float reproductionCooldown = 0f; // 繁衍冷却时间
    private float cooldownDuration = 2f; // 冷却持续时间

    // 静态变量记录所有动物数量
    public static int totalAnimalCount = 0;
    
    void Start()
    {
        // 动物生成时增加总数量
        totalAnimalCount++;
        
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
        
        // 添加刚体使其可以物理交互
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
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
        // 只有成年体且场景中至少有2个动物且不在繁衍冷却期才能繁衍
        if (!isAdult || totalAnimalCount < 2 || reproductionCooldown > 0f) return;
        
        // 查找所有其他成年动物
        AnimalItem[] allAnimals = FindObjectsOfType<AnimalItem>();
        AnimalItem suitableMate = null;
        float nearestDistance = float.MaxValue;
        
        foreach (AnimalItem animal in allAnimals)
        {
            // 排除自己，只找成年体且不饥饿且不在冷却期的动物
            // 添加空引用检查
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

        // 确保配偶仍然符合所有条件
        if (mate != null && mate.isAdult && !mate.isHungry && !isHungry &&
            !mate.isReproducing && mate.reproductionCooldown <= 0f)
        {
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
                
                Debug.Log("成年AnimalItem进食完毕，进入吃饱状态，可以繁衍");
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
        Material animalMaterial = meshRenderer.material;
        Color originalColor = animalMaterial.color;
        Color targetColor = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f); // 颜色变暗
        
        float growthTime = 1f; // 成长时间1秒
        float elapsedTime = 0f;
        
        while (elapsedTime < growthTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthTime;
            
            // 平滑插值缩放和颜色
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            animalMaterial.color = Color.Lerp(originalColor, targetColor, progress);
            
            yield return null;
        }
        
        // 确保最终值准确
        transform.localScale = targetScale;
        animalMaterial.color = targetColor;
        
        Debug.Log("AnimalItem成长为成年体：体型变大，颜色变暗");
        
        // 成长为成年体后，60%几率生成蓝色小球
        if (Random.Range(0f, 1f) < 0.6f)
        {
            SpawnBlueSphere();
        }
        
        // 开始游荡行为
        StartWandering();
    }
    
    void StartWandering()
    {
        isWandering = true;
        isHungry = false;
        hungerTimer = 0f;
        
        // 恢复正常颜色
        RestoreNormalColor();
        
        ChooseNewWanderTarget();
        Debug.Log("AnimalItem开始游荡行为");
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
        
        // 游荡5秒后重新饥饿
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
        Debug.Log($"动物被销毁，剩余动物数量: {totalAnimalCount}");
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
