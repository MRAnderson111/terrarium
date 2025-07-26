using System.Collections;
using UnityEngine;

/// <summary>
/// 重构后的动物主控制器 - 协调各个系统组件
/// </summary>
public class AnimalItem : MonoBehaviour
{
    [Header("动物基础设置")]
    [SerializeField] private bool debugMode = false;

    // 系统组件引用
    private AnimalMovementSystem movementSystem;
    private AnimalNeedsSystem needsSystem;
    private AnimalReproductionSystem reproductionSystem;
    private AnimalEnvironmentSystem environmentSystem;
    private AnimalLifecycleSystem lifecycleSystem;
    private AnimalVisualSystem visualSystem;
    private AnimalHabitatSystem habitatSystem;

    // 输入处理
    private static bool keyProcessedThisFrame = false;

    // 状态标记
    private bool hasGrounded = false;
    private bool isInitialized = false;

    void Start()
    {
        Debug.Log($"动物开始初始化，位置: {transform.position}");

        // 首先确保基础物理组件存在
        EnsurePhysicsComponents();

        InitializeSystems();
        SetupEventHandlers();

        Debug.Log($"动物初始化完成，总数量: {AnimalLifecycleSystem.GetTotalAnimalCount()}");
        Debug.Log($"动物组件状态 - 刚体: {GetComponent<Rigidbody>() != null}, 碰撞器: {GetComponent<Collider>() != null}");

        // 检查场景中的地面对象
        CheckGroundObjects();
    }

    private void EnsurePhysicsComponents()
    {
        // 确保有刚体组件（用于重力和碰撞）
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            Debug.Log("添加了刚体组件");
        }

        // 确保有碰撞器
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            Debug.Log("添加了碰撞器组件");
        }
    }

    private void CheckGroundObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int groundCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (IsGroundObject(obj))
            {
                groundCount++;
                Debug.Log($"发现地面对象: {obj.name}, 位置: {obj.transform.position}");
            }
        }

        Debug.Log($"场景中共发现 {groundCount} 个地面对象");
    }
    
    private void InitializeSystems()
    {
        // 获取或添加系统组件
        movementSystem = GetComponent<AnimalMovementSystem>();
        if (movementSystem == null) movementSystem = gameObject.AddComponent<AnimalMovementSystem>();

        needsSystem = GetComponent<AnimalNeedsSystem>();
        if (needsSystem == null) needsSystem = gameObject.AddComponent<AnimalNeedsSystem>();

        reproductionSystem = GetComponent<AnimalReproductionSystem>();
        if (reproductionSystem == null) reproductionSystem = gameObject.AddComponent<AnimalReproductionSystem>();

        environmentSystem = GetComponent<AnimalEnvironmentSystem>();
        if (environmentSystem == null) environmentSystem = gameObject.AddComponent<AnimalEnvironmentSystem>();

        lifecycleSystem = GetComponent<AnimalLifecycleSystem>();
        if (lifecycleSystem == null) lifecycleSystem = gameObject.AddComponent<AnimalLifecycleSystem>();

        visualSystem = GetComponent<AnimalVisualSystem>();
        if (visualSystem == null) visualSystem = gameObject.AddComponent<AnimalVisualSystem>();

        habitatSystem = GetComponent<AnimalHabitatSystem>();
        if (habitatSystem == null) habitatSystem = gameObject.AddComponent<AnimalHabitatSystem>();

        isInitialized = true;
    }
    
    private void SetupEventHandlers()
    {
        // 设置系统间的事件连接
        if (needsSystem != null)
        {
            needsSystem.OnBecameHungry += OnBecameHungry;
            needsSystem.OnBecameThirsty += OnBecameThirsty;
            needsSystem.OnFoodFound += OnFoodFound;
            needsSystem.OnWaterFound += OnWaterFound;
            needsSystem.OnAteFood += OnAteFood;
            needsSystem.OnDrankWater += OnDrankWater;
        }
        
        if (reproductionSystem != null)
        {
            reproductionSystem.OnBecameAdult += OnBecameAdult;
            reproductionSystem.OnReproductionCompleted += OnReproductionCompleted;
            reproductionSystem.OnNewbornWaitCompleted += OnNewbornWaitCompleted;
        }
        
        if (movementSystem != null)
        {
            movementSystem.OnTargetReached += OnTargetReached;
        }
        
        if (environmentSystem != null)
        {
            environmentSystem.OnEnvironmentalStatusChanged += OnEnvironmentalStatusChanged;
        }
        
        if (habitatSystem != null)
        {
            habitatSystem.OnHabitatBuilt += OnHabitatBuilt;
        }
    }
    
    void Update()
    {
        if (!isInitialized) return;
        
        // 处理按键输入
        HandleKeyInput();
        
        // 如果正在死亡过程中，不执行其他逻辑
        if (lifecycleSystem != null && lifecycleSystem.IsDying) return;
        if (environmentSystem != null && environmentSystem.IsDyingFromEnvironment) return;
        
        // 新生幼体等待逻辑
        if (reproductionSystem != null && reproductionSystem.IsNewborn && reproductionSystem.IsWaiting())
        {
            return; // 等待期间不执行其他逻辑
        }
        
        // 核心行为逻辑
        HandleCoreBehavior();
    }
    
    private void HandleCoreBehavior()
    {
        // 只有落地后才能执行移动行为
        if (!hasGrounded)
        {
            if (Time.frameCount % 300 == 0) // 每5秒输出一次
            {
                Debug.Log("动物尚未落地，等待落地中...");
            }
            return;
        }

        // 检查睡眠行为（成年体专有）
        if (habitatSystem != null && reproductionSystem != null && reproductionSystem.IsAdult)
        {
            habitatSystem.HandleSleepBehavior();

            if (habitatSystem.IsSleeping)
            {
                return; // 睡觉时不执行其他行为
            }

            if (habitatSystem.IsGoingToSleep)
            {
                habitatSystem.MoveToHabitat();
                return;
            }
        }

        // 处理需求相关的移动
        if (needsSystem != null)
        {
            // 如果饥饿或口渴，优先满足需求
            if (needsSystem.IsHungry || needsSystem.IsThirsty)
            {
                if (Time.frameCount % 300 == 0) // 每5秒输出一次
                {
                    Debug.Log($"动物正在处理需求 - 饥饿: {needsSystem.IsHungry}, 口渴: {needsSystem.IsThirsty}");
                }
                HandleNeedsMovement();
                return;
            }
        }

        // 处理繁衍行为
        if (reproductionSystem != null && reproductionSystem.IsLookingForMate)
        {
            if (Time.frameCount % 300 == 0) // 每5秒输出一次
            {
                Debug.Log("动物正在寻找配偶");
            }
            HandleMateMovement();
            return;
        }

        // 成年动物游荡行为
        if (reproductionSystem != null && reproductionSystem.IsAdult &&
            movementSystem != null && movementSystem.IsWandering)
        {
            // 尝试寻找配偶（如果不在冷却期且不是夜晚）
            if (reproductionSystem.CanReproduce && !IsNight())
            {
                if (Time.frameCount % 300 == 0) // 每5秒输出一次
                {
                    Debug.Log("成年动物尝试寻找配偶进行繁衍");
                }
                reproductionSystem.TryFindMate();
            }
        }

        // 调试：输出当前状态
        if (Time.frameCount % 600 == 0) // 每10秒输出一次状态
        {
            string status = "动物状态: ";
            status += $"落地={hasGrounded}, ";
            status += $"成年={reproductionSystem?.IsAdult}, ";
            status += $"游荡={movementSystem?.IsWandering}, ";
            status += $"饥饿={needsSystem?.IsHungry}, ";
            status += $"口渴={needsSystem?.IsThirsty}, ";
            status += $"寻找配偶={reproductionSystem?.IsLookingForMate}, ";
            status += $"可繁衍={reproductionSystem?.CanReproduce}";
            Debug.Log(status);
        }
    }
    
    private void HandleNeedsMovement()
    {
        if (needsSystem == null || movementSystem == null) return;
        
        // 优先处理口渴
        if (needsSystem.IsThirsty && needsSystem.TargetWater != null)
        {
            if (!movementSystem.IsMoving)
            {
                movementSystem.MoveTo(needsSystem.TargetWater.position);
            }
        }
        // 然后处理饥饿
        else if (needsSystem.IsHungry && needsSystem.TargetPlant != null)
        {
            if (!movementSystem.IsMoving)
            {
                movementSystem.MoveTo(needsSystem.TargetPlant.position);
            }
        }
        // 如果目标丢失，重新寻找
        else
        {
            if (needsSystem.IsThirsty)
            {
                needsSystem.FindNearestWater();
            }
            else if (needsSystem.IsHungry)
            {
                needsSystem.FindNearestPlant();
            }
        }
    }
    
    private void HandleMateMovement()
    {
        if (reproductionSystem == null || movementSystem == null) return;
        
        if (reproductionSystem.TargetMate != null)
        {
            if (!movementSystem.IsMoving)
            {
                movementSystem.MoveTo(reproductionSystem.TargetMate.position);
            }
            
            // 检查是否到达配偶附近
            float distance = Vector3.Distance(transform.position, reproductionSystem.TargetMate.position);
            if (distance < 2f)
            {
                reproductionSystem.AttemptReproduction(reproductionSystem.TargetMate);
            }
        }
        else
        {
            reproductionSystem.StopLookingForMate();
        }
    }
    
    private void StartWandering()
    {
        if (movementSystem != null)
        {
            movementSystem.StartWandering();
        }
        
        // 成年体尝试建立居住地
        if (habitatSystem != null && reproductionSystem != null && reproductionSystem.IsAdult)
        {
            habitatSystem.TryEstablishHabitat();
        }
        
        if (debugMode)
        {
            Debug.Log("开始游荡行为");
        }
    }
    
    private bool IsNight()
    {
        try
        {
            var daylightType = System.Type.GetType("Actor_Daylight");
            if (daylightType != null)
            {
                var isNightProperty = daylightType.GetProperty("IsNight");
                if (isNightProperty != null)
                {
                    return (bool)isNightProperty.GetValue(null);
                }
            }
        }
        catch (System.Exception e)
        {
            if (debugMode)
            {
                Debug.LogWarning($"获取昼夜状态失败: {e.Message}");
            }
        }
        return false;
    }

    private void HandleKeyInput()
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
            if (debugMode)
            {
                Debug.Log("按下2键，生成新动物");
            }
        }
    }

    private static void SpawnAnimalAtPosition(Vector3 position)
    {
        // 创建新的AnimalItem对象
        GameObject newAnimal = new("AnimalItem");
        newAnimal.transform.position = position;

        // 添加AnimalItem组件
        newAnimal.AddComponent<AnimalItem>();

        Debug.Log($"在位置 {position} 生成了新动物");
    }

    // 事件处理方法
    private void OnBecameHungry()
    {
        if (visualSystem != null)
        {
            visualSystem.SetHungryVisual(true);
        }

        // 寻找食物
        if (needsSystem != null)
        {
            needsSystem.FindNearestPlant();
        }

        if (debugMode)
        {
            Debug.Log("动物变得饥饿，开始寻找食物");
        }
    }

    private void OnBecameThirsty()
    {
        if (visualSystem != null)
        {
            visualSystem.SetThirstyVisual(true);
        }

        // 寻找水源
        if (needsSystem != null)
        {
            needsSystem.FindNearestWater();
        }

        if (debugMode)
        {
            Debug.Log("动物变得口渴，开始寻找水源");
        }
    }

    private void OnFoodFound(Transform food)
    {
        if (movementSystem != null && food != null)
        {
            movementSystem.MoveTo(food.position);
        }
    }

    private void OnWaterFound(Transform water)
    {
        if (movementSystem != null && water != null)
        {
            movementSystem.MoveTo(water.position);
        }
    }

    private void OnAteFood()
    {
        if (visualSystem != null)
        {
            visualSystem.SetHungryVisual(false);

            // 60%几率生成蓝色小球
            if (Random.Range(0f, 1f) < 0.6f)
            {
                visualSystem.SpawnBlueSphere();
            }
        }

        // 如果是幼年体，成长为成年体
        if (reproductionSystem != null && !reproductionSystem.IsAdult)
        {
            reproductionSystem.GrowToAdult();
        }
        else
        {
            // 成年体进食后开始游荡
            StartWandering();
        }
    }

    private void OnDrankWater()
    {
        if (visualSystem != null)
        {
            visualSystem.SetThirstyVisual(false);
        }

        StartWandering();
    }

    private void OnBecameAdult()
    {
        if (visualSystem != null)
        {
            visualSystem.SetAdultVisual(true);
        }

        StartWandering();
    }

    private void OnReproductionCompleted()
    {
        // 繁衍消耗体力，需要重新进食和喝水
        if (needsSystem != null)
        {
            needsSystem.SetHungryAndThirsty();
        }
    }

    private void OnNewbornWaitCompleted()
    {
        // 新生幼体等待结束，添加刚体
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
    }

    private void OnTargetReached()
    {
        Debug.Log("动物到达目标位置");

        // 到达目标后的处理
        if (needsSystem != null)
        {
            // 检查是否到达食物或水源
            if (needsSystem.IsHungry && needsSystem.TargetPlant != null)
            {
                Debug.Log($"动物到达植物目标，准备进食: {needsSystem.TargetPlant.name}");
                needsSystem.Eat(needsSystem.TargetPlant);
            }
            else if (needsSystem.IsThirsty && needsSystem.TargetWater != null)
            {
                Debug.Log($"动物到达水源目标，准备喝水: {needsSystem.TargetWater.name}");
                needsSystem.Drink(needsSystem.TargetWater);
            }
            else
            {
                Debug.Log($"动物到达目标但状态不匹配 - 饥饿: {needsSystem.IsHungry}, 口渴: {needsSystem.IsThirsty}, 植物目标: {needsSystem.TargetPlant?.name}, 水源目标: {needsSystem.TargetWater?.name}");
            }
        }
        else
        {
            Debug.LogWarning("动物到达目标但needsSystem为null");
        }
    }

    private void OnEnvironmentalStatusChanged()
    {
        // 环境状态变化时更新移动和繁衍修正系数
        if (environmentSystem != null)
        {
            if (movementSystem != null)
            {
                movementSystem.SetEnvironmentalSpeedModifier(environmentSystem.EnvironmentalSpeedModifier);
            }

            if (reproductionSystem != null)
            {
                reproductionSystem.SetEnvironmentalReproductionModifier(environmentSystem.EnvironmentalReproductionModifier);
            }
        }
    }

    private void OnHabitatBuilt()
    {
        if (debugMode)
        {
            Debug.Log("居住地建立完成");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"动物碰撞检测：碰撞到 {collision.gameObject.name}，当前落地状态: {hasGrounded}");

        // 改进的地面检测：检查碰撞是否来自下方
        if (!hasGrounded)
        {
            // 首先检查碰撞对象是否为其他动物，如果是则忽略
            if (IsAnimalObject(collision.gameObject))
            {
                Debug.Log($"碰撞到其他动物 {collision.gameObject.name}，忽略碰撞");
                return;
            }

            // 检查碰撞点是否在动物下方
            bool isGroundCollision = false;

            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.Log($"碰撞点法线: {contact.normal}, Y分量: {contact.normal.y}");
                // 如果碰撞点的法线向上（Y分量为正），说明是地面碰撞
                if (contact.normal.y > 0.5f)
                {
                    isGroundCollision = true;
                    Debug.Log("通过法线检测确认为地面碰撞");
                    break;
                }
            }

            // 也可以通过碰撞对象的标签或名称来判断
            if (!isGroundCollision)
            {
                isGroundCollision = IsGroundObject(collision.gameObject);
                if (isGroundCollision)
                {
                    Debug.Log("通过名称/标签检测确认为地面碰撞");
                }
            }

            if (isGroundCollision)
            {
                hasGrounded = true;
                Debug.Log($"动物成功落地在 {collision.gameObject.name}");

                // 通知移动系统动物已落地
                if (movementSystem != null)
                {
                    movementSystem.SetGrounded(true);
                    Debug.Log("已通知移动系统动物落地");
                }

                // 固定动物位置，但保留刚体用于移动
                if (TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    // 停止物理运动，但保留刚体
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // 设置为运动学模式，不受物理影响但可以移动
                    Debug.Log("刚体已设置为运动学模式");
                }

                // 2秒后开始寻找食物和水源
                StartCoroutine(StartBehaviorAfterDelay());
            }
            else
            {
                Debug.Log($"碰撞到 {collision.gameObject.name}，但不是地面，继续下落");
            }
        }
        else
        {
            Debug.Log("动物已经落地，忽略后续碰撞");
        }
    }

    private bool IsAnimalObject(GameObject obj)
    {
        // 检查对象是否有AnimalItem组件（最可靠的方法）
        if (obj.GetComponent<AnimalItem>() != null)
        {
            return true;
        }

        // 检查对象名称是否包含Animal相关字符串
        if (obj.name.Contains("Animal") || obj.name.Contains("AnimalItem") || obj.name.Contains("Offspring"))
        {
            return true;
        }

        // 不再依赖标签，因为标签可能不存在
        // 通过组件和名称检查已经足够可靠

        return false;
    }

    private bool IsGroundObject(GameObject obj)
    {
        // 通过名称检查地面对象（最可靠的方法）
        return obj.name.Contains("Ground") ||
               obj.name.Contains("Plane") ||
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround") ||
               obj.name.Contains("Terrain") ||
               obj.name.Contains("Floor") ||
               obj.name.Contains("Surface");
    }

    private IEnumerator StartBehaviorAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);

        Debug.Log("动物落地后开始初始行为");

        // 所有动物落地后都应该开始寻找食物（设置为饥饿状态）
        if (needsSystem != null)
        {
            // 设置初始饥饿状态，让动物开始寻找食物
            needsSystem.SetHungryAndThirsty();
            Debug.Log("设置动物为饥饿和口渴状态，开始寻找食物和水源");
        }
    }
}
