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
    
    void Start()
    {
        InitializePlant();
    }
    
    void InitializePlant()
    {
        // 植物生成时增加总数量
        totalPlantCount++;
        UpdateEnvironmentalFood();
        
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
        Vector3 targetScale = originalScale * 2f; // 膨胀到两倍
        Color originalColor = plantMaterial.color;
        Color targetColor = DetermineTargetColor(originalColor);
        
        float growthTime = isOnBarrenGround ? 10f : 1f; // 只有贫瘠土地生长时间10秒，肥沃土地和正常土地都是1秒
        float elapsedTime = 0f;
        
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
        
        // 设置蓝色材质
        MeshRenderer sphereRenderer = blueSphere.GetComponent<MeshRenderer>();
        Material blueMaterial = new Material(Shader.Find("Standard"));
        blueMaterial.color = Color.blue;
        sphereRenderer.material = blueMaterial;
        
        // 移除刚体组件（保留碰撞器用于点击检测）
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
            DestroyImmediate(sphereRb);
        
        // 保留碰撞器但设置为触发器
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
            sphereCollider.isTrigger = true;
        
        // 添加点击脚本
        blueSphere.AddComponent<BlueSphereClickHandler>();
        
        Debug.Log($"在位置 {spherePosition} 生成了浮空蓝色小球");
    }
    
    static void UpdateEnvironmentalFood()
    {
        environmentalFood = totalPlantCount * 2;
    }
    
    void OnDestroy()
    {
        // 如果是子株被销毁，通知主植株
        if (plantType == PlantType.Sub && parentPlant != null)
            parentPlant.OnChildDestroyed(this);
        
        // 植物被销毁时减少总数量
        totalPlantCount--;
        UpdateEnvironmentalFood();
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
