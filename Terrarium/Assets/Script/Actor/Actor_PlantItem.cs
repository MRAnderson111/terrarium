using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantItem : MonoBehaviour
{
    // 状态标志
    private static bool keyProcessedThisFrame = false; // 静态变量控制每帧只处理一次
    private bool hasGrounded = false; // 是否已经接触地面
    private bool isGrowing = false;   // 是否正在生长
    private bool canReplicate = false; // 是否可以复制（只有1键生成的才能复制）
    private bool isOnBarrenGround = false; // 记录是否在贫瘠土地上
    private bool isOnFertileGround = false; // 记录是否在肥沃土地上
    
    // 繁殖相关
    private int replicationCount = 0; // 已复制次数
    private int maxReplications = 1; // 最大复制次数
    
    // 渲染相关
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    
    // 植物分类
    public enum PlantType
    {
        Main,  // 主植株（按键生成）
        Sub    // 子株（繁殖生成）
    }
    
    public PlantType plantType = PlantType.Main; // 默认为主植株
    
    // 主植株引用（子株需要记录其主植株）
    private PlantItem parentPlant = null;
    private List<PlantItem> childPlants = new List<PlantItem>(); // 主植株记录其子株
    
    // 静态变量记录所有植物数量和环境食物数值
    public static int totalPlantCount = 0;
    public static int environmentalFood = 0;

    // 植物适宜生存的环境条件
    [Header("植物生存环境条件")]
    [SerializeField] private float optimalTemperatureMin = 15f; // 适宜温度下限
    [SerializeField] private float optimalTemperatureMax = 30f; // 适宜温度上限
    [SerializeField] private float optimalHumidityMin = 40f;    // 适宜湿度下限
    [SerializeField] private float optimalHumidityMax = 80f;    // 适宜湿度上限
    [SerializeField] private float optimalSunlightMin = 30f;    // 适宜光照下限
    [SerializeField] private float optimalSunlightMax = 90f;    // 适宜光照上限
    
    // 环境适应性状态
    private bool isInOptimalEnvironment = true; // 是否处于适宜环境
    private float environmentalStress = 0f;     // 环境压力值 (0-1)
    private float extremeStressTimer = 0f;      // 极端环境压力计时器
    private bool isWithering = false;           // 是否正在枯萎
    
    [Header("枯萎死亡设置")]
    [SerializeField] private float extremeStressThreshold = 0.8f;  // 极端压力阈值
    [SerializeField] private float witherTimeLimit = 10f;          // 极端环境下存活时间限制

    void Awake()
    {
        // 在场景开始时重置静态变量（只在第一个实例时执行）
        if (FindObjectsOfType<PlantItem>().Length == 1)
        {
            totalPlantCount = 0;
            environmentalFood = 0;
            Debug.Log("PlantItem静态变量已重置");
        }
    }

    void Start()
    {
        InitializePlant();
        
        // 开始环境监测
        StartCoroutine(MonitorEnvironmentalConditions());
    }
    
    void InitializePlant()
    {
        Debug.Log($"PlantItem初始化前 - 总数量: {totalPlantCount}, 环境食物: {environmentalFood}");
        
        // 只有不是"PlantFirst"的植物才计入总数量
        if (gameObject.name != "PlantFirst")
        {
            totalPlantCount++;
            UpdateEnvironmentalFood();
        }
        
        Debug.Log($"PlantItem初始化后 - 总数量: {totalPlantCount}, 环境食物: {environmentalFood}");
        
        // 初始化组件
        InitializeComponents();
        
        // 设置外观
        SetupAppearance();
    }
    
    void InitializeComponents()
    {
        // 确保有MeshRenderer和MeshFilter组件
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
        
        if (GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        
        // 添加碰撞器（确保不是触发器）
        if (GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = false; // 保持物理碰撞
        }
        
        // 添加刚体使其可以物理交互
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
    }
    
    void SetupAppearance()
    {
        // 设置为立方体网格
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建绿色材质
        meshRenderer = GetComponent<MeshRenderer>();
        plantMaterial = new Material(Shader.Find("Standard"));
        plantMaterial.color = Color.green;
        meshRenderer.material = plantMaterial;
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
        HandleKeyInput();
    }
    
    void HandleKeyInput()
    {
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha1))
            keyProcessedThisFrame = false;
        
        // 按下1键在指定位置生成绿色植物正方形
        if (Input.GetKeyDown(KeyCode.Alpha1) && !keyProcessedThisFrame)
        {
            keyProcessedThisFrame = true;
            SpawnPlantAtPosition(new Vector3(0, 20, 0));
        }
    }
    
    void SpawnPlantAtPosition(Vector3 position)
    {
        // 创建新的PlantItem对象
        GameObject newPlant = new GameObject("PlantItem");
        newPlant.transform.position = position;
        
        // 添加PlantItem组件，这会自动设置为绿色正方形
        PlantItem plantComponent = newPlant.AddComponent<PlantItem>();
        
        // 设置为主植株，可以复制
        plantComponent.plantType = PlantType.Main;
        plantComponent.canReplicate = true;
        
        Debug.Log($"在位置 {position} 生成了绿色植物正方形 (主植株)");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleGroundCollision(collision);
    }
    
    void HandleGroundCollision(Collision collision)
    {
        // 检查是否接触到地面（通过标签或名称判断）
        if (!hasGrounded && IsGroundObject(collision.gameObject))
        {
            hasGrounded = true;
            
            // 检测地面类型
            DetectGroundType(collision.gameObject);
            
            // 固定植物位置
            FixPosition();
            
            // 开始生长
            StartCoroutine(GrowAfterDelay());
            
            Debug.Log($"PlantItem接触到地面，完全固定位置，2秒后开始生长 (贫瘠土地: {isOnBarrenGround}, 肥沃土地: {isOnFertileGround})");
        }
    }
    
    bool IsGroundObject(GameObject obj)
    {
        return obj.CompareTag("Ground") || 
               obj.name.Contains("Ground") || 
               obj.name.Contains("Plane") ||
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround");
    }
    
    void DetectGroundType(GameObject ground)
    {
        // 检测是否在贫瘠土地上
        isOnBarrenGround = ground.name.Contains("BarrenGround");
        
        // 检测是否在肥沃土地上
        isOnFertileGround = ground.name.Contains("FertileGround");
        
        // 如果在肥沃土地上，设置可以复制两次
        if (isOnFertileGround)
        {
            maxReplications = 2;
            Debug.Log("PlantItem落在肥沃土地上，可复制2次");
        }
    }
    
    void FixPosition()
    {
        // 移除刚体组件但保留碰撞器
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);
        
        // 确保碰撞器存在且不是触发器（保持物理碰撞）
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = false; // 确保不是触发器，保持物理碰撞
            Debug.Log("植物碰撞器已保留，可以被动物碰撞");
        }
        else
        {
            // 如果碰撞器不存在，重新添加一个
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = false;
            Debug.Log("植物重新添加了碰撞器");
        }
    }
    
    IEnumerator GrowAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);
        
        if (!isGrowing)
        {
            isGrowing = true;
            StartCoroutine(GrowPlant());
        }
    }
    
    IEnumerator GrowPlant()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f;
        Color originalColor = plantMaterial.color;
        Color targetColor = DetermineTargetColor(originalColor);
        
        // 根据环境条件调整生长时间
        float baseGrowthTime = isOnBarrenGround ? 10f : 1f;
        float environmentalMultiplier = 1f + environmentalStress; // 环境压力越大，生长越慢
        float growthTime = baseGrowthTime * environmentalMultiplier;
        
        float elapsedTime = 0f;
        
        Debug.Log($"植物开始生长，预计时间: {growthTime:F1}秒 (环境压力影响: {environmentalMultiplier:F2}x)");
        
        // 生长过程
        while (elapsedTime < growthTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthTime;
            
            // 平滑插值缩放和颜色
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            plantMaterial.color = Color.Lerp(originalColor, targetColor, progress);
            
            yield return null;
        }
        
        // 确保最终值准确
        transform.localScale = targetScale;
        plantMaterial.color = targetColor;
        
        Debug.Log($"PlantItem生长完成：体积膨胀到两倍，成年后颜色变化 (贫瘠土地: {isOnBarrenGround}, 肥沃土地: {isOnFertileGround})");
        Debug.Log($"植物总数量: {totalPlantCount}, 环境食物数值: {environmentalFood}");
        
        // 生长完成后处理
        HandleGrowthCompletion();
    }
    
    Color DetermineTargetColor(Color originalColor)
    {
        if (isOnFertileGround)
            return new Color(0.5f, 1f, 0.5f); // 亮绿色
        else if (isOnBarrenGround)
            return new Color(originalColor.r * 0.3f, originalColor.g * 0.3f, originalColor.b * 0.3f);
        else
            return new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
    }
    
    void HandleGrowthCompletion()
    {
        // 60%几率生成蓝色小球
        if (Random.Range(0f, 1f) < 0.6f)
            SpawnBlueSphere();
        
        // 生长完成后，5秒后复制
        StartCoroutine(ReplicateAfterDelay());
    }
    
    IEnumerator ReplicateAfterDelay()
    {
        // 等待5秒
        yield return new WaitForSeconds(5f);
        
        // 只有可以复制且没有达到复制上限的plant才会复制
        if (canReplicate && replicationCount < maxReplications)
        {
            replicationCount++;
            ReplicatePlant();
            
            // 如果还没达到复制上限，继续等待下次复制
            if (replicationCount < maxReplications)
                StartCoroutine(ReplicateAfterDelay());
        }
    }
    
    void ReplicatePlant()
    {
        // 检查是否还能继续繁殖
        if (childPlants.Count >= maxReplications)
        {
            Debug.Log("已达到最大子株数量，停止繁殖");
            return;
        }
        
        // 在附近随机位置生成新的PlantItem
        Vector3 replicatePosition = transform.position + new Vector3(
            Random.Range(-8f, 8f), // X轴随机偏移
            0f,
            Random.Range(-8f, 8f)  // Z轴随机偏移
        );
        
        // 创建新的PlantItem对象
        GameObject newPlant = new GameObject("PlantItem");
        newPlant.transform.position = replicatePosition;
        
        // 添加PlantItem组件，设置为子株（不能复制）
        PlantItem plantComponent = newPlant.AddComponent<PlantItem>();
        plantComponent.plantType = PlantType.Sub;
        plantComponent.canReplicate = false;
        plantComponent.parentPlant = this; // 设置父植株引用
        
        // 主植株记录子株
        childPlants.Add(plantComponent);
        
        Debug.Log($"主植株重新繁殖了子株 (当前子株数量: {childPlants.Count}/{maxReplications})");
    }
    
    void SpawnBlueSphere()
    {
        // 在plant正上方3米处生成蓝色小球
        Vector3 spherePosition = transform.position + Vector3.up * 3f;
        
        // 创建球体对象
        GameObject blueSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        blueSphere.name = "BlueSphere";
        blueSphere.transform.position = spherePosition;
        blueSphere.transform.localScale = Vector3.one * 0.3f;
        
        // 重要：将蓝色小球设置为独立对象，不作为植物的子对象
        blueSphere.transform.SetParent(null);
        
        // 设置蓝色材质
        MeshRenderer sphereRenderer = blueSphere.GetComponent<MeshRenderer>();
        Material blueMaterial = new Material(Shader.Find("Standard"));
        blueMaterial.color = Color.blue;
        sphereRenderer.material = blueMaterial;
        
        // 移除刚体组件（保留碰撞器用于点击检测）
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
            Destroy(sphereRb);
        
        // 保留碰撞器但设置为触发器
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
            sphereCollider.isTrigger = true;
        
        // 添加点击脚本
        blueSphere.AddComponent<BlueSphereClickHandler>();
        
        // 添加自动销毁组件，防止蓝色小球永久存在
        blueSphere.AddComponent<AutoDestroy>();
        
        Debug.Log($"在位置 {spherePosition} 生成了独立的浮空蓝色小球");
    }
    
    static void UpdateEnvironmentalFood()
    {
        environmentalFood = totalPlantCount * 2;
    }

    // 公共静态方法供UI访问
    public static int GetTotalPlantCount()
    {
        return totalPlantCount;
    }

    public static int GetEnvironmentalFood()
    {
        return environmentalFood;
    }
    
    void OnDestroy()
    {
        // 如果是子株被销毁，通知主植株
        if (plantType == PlantType.Sub && parentPlant != null)
            parentPlant.OnChildDestroyed(this);
        
        // 只有不是"PlantFirst"的植物被销毁时才减少总数量
        if (gameObject.name != "PlantFirst")
        {
            totalPlantCount--;
            UpdateEnvironmentalFood();
        }
        
        Debug.Log($"植物被销毁，剩余植物数量: {totalPlantCount}, 环境食物数值: {environmentalFood}");
    }
    
    void OnChildDestroyed(PlantItem childPlant)
    {
        // 从子株列表中移除
        childPlants.Remove(childPlant);
        
        // 如果当前子株数量少于最大复制数量，且主植株还活着，则重新繁殖
        if (childPlants.Count < maxReplications && gameObject != null && canReplicate)
        {
            Debug.Log($"子株被吃掉，主植株开始重新繁殖 (当前子株数量: {childPlants.Count}/{maxReplications})");
            // 等待5秒后再繁殖
            StartCoroutine(ReplicateAfterChildLoss());
        }
    }
    
    IEnumerator ReplicateAfterChildLoss()
    {
        // 等待5秒
        yield return new WaitForSeconds(5f);
        
        // 再次检查是否可以繁殖（防止在等待期间主植株被销毁）
        if (gameObject != null && childPlants.Count < maxReplications && canReplicate)
            ReplicatePlant();
    }
    
    IEnumerator MonitorEnvironmentalConditions()
    {
        while (gameObject != null)
        {
            // 每2秒检测一次环境条件
            yield return new WaitForSeconds(2f);
            
            CheckEnvironmentalConditions();
        }
    }
    
    void CheckEnvironmentalConditions()
    {
        // 如果已经在枯萎过程中，不再检查环境条件
        if (isWithering) return;
        
        // 获取当前环境数据
        float currentTemperature = GetCurrentTemperature();
        float currentHumidity = GetCurrentHumidity();
        float currentSunlight = GetCurrentSunlight();
        
        // 计算环境适应性
        float tempStress = CalculateTemperatureStress(currentTemperature);
        float humidityStress = CalculateHumidityStress(currentHumidity);
        float sunlightStress = CalculateSunlightStress(currentSunlight);
        
        // 综合环境压力
        environmentalStress = (tempStress + humidityStress + sunlightStress) / 3f;
        
        // 判断是否处于适宜环境
        bool wasOptimal = isInOptimalEnvironment;
        isInOptimalEnvironment = environmentalStress < 0.3f; // 压力小于30%认为是适宜环境
        
        // 如果环境状态发生变化，调整植物状态
        if (wasOptimal != isInOptimalEnvironment)
        {
            OnEnvironmentalStatusChanged();
        }
        
        // 检查极端环境压力
        if (environmentalStress >= extremeStressThreshold)
        {
            extremeStressTimer += 2f; // 每次检测间隔2秒
            
            if (extremeStressTimer >= witherTimeLimit)
            {
                Debug.Log($"植物在极端环境下存活{witherTimeLimit}秒后开始枯萎死亡");
                StartCoroutine(WitherAndDie());
                return; // 开始枯萎后不再执行后续逻辑
            }
            else
            {
                Debug.Log($"植物承受极端环境压力 {extremeStressTimer:F1}/{witherTimeLimit}秒");
            }
        }
        else
        {
            // 环境改善时重置计时器
            if (extremeStressTimer > 0f)
            {
                extremeStressTimer = 0f;
                Debug.Log("环境条件改善，植物脱离极端压力状态");
            }
        }
        
        // 如果环境压力过大，可能影响生长和繁殖
        if (environmentalStress > 0.7f)
        {
            HandleHighEnvironmentalStress();
        }
        
        Debug.Log($"植物环境检测 - 温度: {currentTemperature:F1}°C, 湿度: {currentHumidity:F1}%, 光照: {currentSunlight:F1}, 环境压力: {environmentalStress:F2}");
    }
    
    float GetCurrentTemperature()
    {
        try
        {
            var envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                var temperatureProperty = envFactoryType.GetProperty("Temperature");
                if (temperatureProperty != null)
                {
                    return (float)temperatureProperty.GetValue(null);
                }
            }
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
            var envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                var humidityProperty = envFactoryType.GetProperty("Humidity");
                if (humidityProperty != null)
                {
                    return (float)humidityProperty.GetValue(null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取湿度数据失败: {e.Message}");
        }
        return 50f; // 默认湿度
    }
    
    float GetCurrentSunlight()
    {
        try
        {
            var envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                var sunlightProperty = envFactoryType.GetProperty("Sunlight");
                if (sunlightProperty != null)
                {
                    return (float)sunlightProperty.GetValue(null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取光照数据失败: {e.Message}");
        }
        return 50f; // 默认光照
    }
    
    float CalculateTemperatureStress(float temperature)
    {
        if (temperature >= optimalTemperatureMin && temperature <= optimalTemperatureMax)
            return 0f; // 适宜温度，无压力
        
        if (temperature < optimalTemperatureMin)
        {
            // 温度过低
            float deviation = optimalTemperatureMin - temperature;
            return Mathf.Clamp01(deviation / 15f); // 偏离15度为最大压力
        }
        else
        {
            // 温度过高
            float deviation = temperature - optimalTemperatureMax;
            return Mathf.Clamp01(deviation / 15f); // 偏离15度为最大压力
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
            return Mathf.Clamp01(deviation / 20f); // 偏离20%为最大压力
        }
    }
    
    float CalculateSunlightStress(float sunlight)
    {
        if (sunlight >= optimalSunlightMin && sunlight <= optimalSunlightMax)
            return 0f; // 适宜光照，无压力
        
        if (sunlight < optimalSunlightMin)
        {
            // 光照不足
            float deviation = optimalSunlightMin - sunlight;
            return Mathf.Clamp01(deviation / 30f); // 偏离30为最大压力
        }
        else
        {
            // 光照过强
            float deviation = sunlight - optimalSunlightMax;
            return Mathf.Clamp01(deviation / 10f); // 偏离10为最大压力
        }
    }
    
    void OnEnvironmentalStatusChanged()
    {
        if (isInOptimalEnvironment)
        {
            Debug.Log("植物进入适宜环境，生长状态良好");
            // 恢复正常颜色
            if (plantMaterial != null && !isWithering)
            {
                Color currentColor = plantMaterial.color;
                plantMaterial.color = new Color(currentColor.r, Mathf.Max(currentColor.g, 0.8f), currentColor.b, currentColor.a);
            }
        }
        else
        {
            Debug.Log("植物离开适宜环境，开始承受环境压力");
            // 颜色变暗表示压力
            if (plantMaterial != null && !isWithering)
            {
                Color currentColor = plantMaterial.color;
                float stressFactor = 1f - (environmentalStress * 0.5f);
                plantMaterial.color = new Color(currentColor.r * stressFactor, currentColor.g * stressFactor, currentColor.b * stressFactor, currentColor.a);
            }
        }
    }
    
    void HandleHighEnvironmentalStress()
    {
        // 高环境压力下的处理
        Debug.Log("植物承受高环境压力，生长和繁殖受到影响");
        
        // 延长生长时间
        if (isGrowing)
        {
            // 可以在这里调整生长速度
        }
        
        // 降低繁殖几率
        if (canReplicate && Random.Range(0f, 1f) < environmentalStress)
        {
            Debug.Log("由于环境压力，植物跳过本次繁殖");
            return; // 跳过繁殖
        }
    }
    
    IEnumerator WitherAndDie()
    {
        if (isWithering) yield break; // 防止重复执行枯萎过程
        
        isWithering = true;
        
        Debug.Log("植物开始枯萎死亡过程");
        
        // 停止其他协程但不停止当前枯萎协程
        StopCoroutine(nameof(MonitorEnvironmentalConditions));
        StopCoroutine(nameof(GrowAfterDelay));
        StopCoroutine(nameof(GrowPlant));
        StopCoroutine(nameof(ReplicateAfterDelay));
        
        // 获取当前材质和颜色
        if (plantMaterial != null)
        {
            Color originalColor = plantMaterial.color;
            Vector3 originalScale = transform.localScale;
            
            Debug.Log($"开始枯萎动画 - 原始颜色: {originalColor}, 原始尺寸: {originalScale}");
            
            // 3秒内逐渐枯萎（颜色变成棕色，体积缩小）
            float witherTime = 3f;
            float elapsedTime = 0f;
            
            Color witherColor = new Color(0.4f, 0.2f, 0.1f, 1f); // 棕色
            Vector3 witherScale = originalScale * 0.3f; // 缩小到30%
            
            while (elapsedTime < witherTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / witherTime;
                
                // 平滑插值到枯萎状态
                plantMaterial.color = Color.Lerp(originalColor, witherColor, progress);
                transform.localScale = Vector3.Lerp(originalScale, witherScale, progress);
                
                Debug.Log($"枯萎进度: {progress:F2}, 当前颜色: {plantMaterial.color}, 当前尺寸: {transform.localScale}");
                
                yield return null;
            }
            
            // 确保最终状态
            plantMaterial.color = witherColor;
            transform.localScale = witherScale;
            
            Debug.Log("植物枯萎完成，1秒后开始消失");
            
            // 等待1秒后开始消失
            yield return new WaitForSeconds(1f);
            
            // 最后1秒内逐渐变透明
            float fadeTime = 1f;
            elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                
                Color fadeColor = plantMaterial.color;
                fadeColor.a = alpha;
                plantMaterial.color = fadeColor;
                
                Debug.Log($"消失进度: {elapsedTime / fadeTime:F2}, 透明度: {alpha:F2}");
                
                yield return null;
            }
        }
        
        Debug.Log("植物因极端环境死亡");
        
        // 在枯萎位置留下小型贫瘠土地
        CreateBarrenGroundAtWitherLocation();
        
        // 销毁植物对象
        Destroy(gameObject);
    }
    
    void CreateBarrenGroundAtWitherLocation()
    {
        // 在植物枯萎位置创建小型贫瘠土地
        Vector3 barrenGroundPosition = transform.position;
        barrenGroundPosition.y = 0f; // 设置到地面高度
        
        // 创建贫瘠土地对象
        GameObject barrenGround = new GameObject("SmallBarrenGround");
        barrenGround.transform.position = barrenGroundPosition;
        barrenGround.transform.localScale = new Vector3(0.3f, 1f, 0.3f); // 更小的尺寸
        
        // 添加基本组件
        MeshRenderer meshRenderer = barrenGround.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = barrenGround.AddComponent<MeshFilter>();
        
        // 创建平面网格
        meshFilter.mesh = CreateSmallGroundMesh();
        
        // 创建棕色材质表示贫瘠土地
        Material barrenMaterial = new Material(Shader.Find("Standard"));
        barrenMaterial.color = new Color(0.3f, 0.2f, 0.1f, 1f); // 棕色
        meshRenderer.material = barrenMaterial;
        
        // 添加碰撞器
        BoxCollider boxCollider = barrenGround.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(3f, 0.1f, 3f);
        boxCollider.isTrigger = false;
        
        Debug.Log($"在植物枯萎位置 {barrenGroundPosition} 创建了小型贫瘠土地");
    }
    
    Mesh CreateSmallGroundMesh()
    {
        // 创建一个小型平面网格
        Mesh mesh = new Mesh();
        
        // 顶点坐标（3x3的小平面）
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-1.5f, 0f, -1.5f),
            new Vector3(1.5f, 0f, -1.5f),
            new Vector3(-1.5f, 0f, 1.5f),
            new Vector3(1.5f, 0f, 1.5f)
        };
        
        // 三角形索引
        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        
        // UV坐标
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        
        return mesh;
    }
}

// 蓝色小球自动销毁组件
public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 30f; // 30秒后自动销毁
    
    void Start()
    {
        // 30秒后自动销毁，防止场景中积累过多蓝色小球
        Destroy(gameObject, lifeTime);
    }
}

// 蓝色小球点击处理脚本
public class BlueSphereClickHandler : MonoBehaviour
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
