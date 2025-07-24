using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantItem : MonoBehaviour
{
    private static bool keyProcessedThisFrame = false; // 静态变量控制每帧只处理一次一次
    private bool hasGrounded = false; // 是否已经接触地面
    private bool isGrowing = false;   // 是否正在生长
    private bool hasReplicated = false; // 是否已经复制过
    private bool canReplicate = false; // 是否可以复制（只有1键生成的才能复制）
    private bool isOnBarrenGround = false; // 记录是否在贫瘠土地上
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    
    // 静态变量记录所有植物数量和环境食物数值
    public static int totalPlantCount = 0;
    public static int environmentalFood = 0;
    
    void Start()
    {
        // 植物生成时增加总数量
        totalPlantCount++;
        UpdateEnvironmentalFood();
        
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
        
        // 创建绿色材质
        meshRenderer = GetComponent<MeshRenderer>();
        plantMaterial = new Material(Shader.Find("Standard"));
        plantMaterial.color = Color.green;
        meshRenderer.material = plantMaterial;
        
        // 添加碰撞器
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        
        // 可选：添加刚体使其可以物理交互
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
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyProcessedThisFrame = false;
        }
        
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
        
        // 设置为可以复制（只有1键生成的才能复制）
        plantComponent.canReplicate = true;
        
        Debug.Log($"在位置 {position} 生成了绿色植物正方形");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 检查是否接触到地面（通过标签或名称判断）
        if (!hasGrounded && (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.name.Contains("Ground") || 
            collision.gameObject.name.Contains("Plane") ||
            collision.gameObject.name.Contains("BarrenGround")))
        {
            hasGrounded = true;
            
            // 检测是否在贫瘠土地上
            isOnBarrenGround = collision.gameObject.name.Contains("BarrenGround");
            
            // 完全固定植物位置，移除刚体组件
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                DestroyImmediate(rb);
            }
            
            StartCoroutine(GrowAfterDelay());
            Debug.Log($"PlantItem接触到地面，完全固定位置，2秒后开始生长 (贫瘠土地: {isOnBarrenGround})");
        }
    }
    
    System.Collections.IEnumerator GrowAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);
        
        if (!isGrowing)
        {
            isGrowing = true;
            StartCoroutine(GrowPlant());
        }
    }
    
    System.Collections.IEnumerator GrowPlant()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f; // 膨胀到两倍
        Color originalColor = plantMaterial.color;
        Color targetColor;
        
        if (isOnBarrenGround)
        {
            // 在贫瘠土地上，颜色变得更深
            targetColor = new Color(originalColor.r * 0.3f, originalColor.g * 0.3f, originalColor.b * 0.3f);
        }
        else
        {
            // 正常土地，颜色变深
            targetColor = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
        }
        
        float growthTime = isOnBarrenGround ? 10f : 1f; // 贫瘠土地生长时间10秒，正常1秒
        float elapsedTime = 0f;
        
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
        
        Debug.Log($"PlantItem生长完成：体积膨胀到两倍，颜色变深 (贫瘠土地: {isOnBarrenGround})");
        Debug.Log($"植物总数量: {totalPlantCount}, 环境食物数值: {environmentalFood}");
        
        // 60%几率生成蓝色小球
        if (Random.Range(0f, 1f) < 0.6f)
        {
            SpawnBlueSphere();
        }
        
        // 生长完成后，5秒后复制
        StartCoroutine(ReplicateAfterDelay());
    }
    
    System.Collections.IEnumerator ReplicateAfterDelay()
    {
        // 等待5秒
        yield return new WaitForSeconds(5f);
        
        // 只有可以复制且没有复制过的plant才会复制
        if (canReplicate && !hasReplicated)
        {
            hasReplicated = true;
            ReplicatePlant();
        }
    }
    
    void ReplicatePlant()
    {
        // 在附近随机位置生成新的PlantItem
        Vector3 replicatePosition = transform.position + new Vector3(
            Random.Range(-8f, 8f), // X轴随机偏移
            3f,                    // Y轴稍微抬高
            Random.Range(-8f, 8f)  // Z轴随机偏移
        );
        
        // 创建新的PlantItem对象
        GameObject newPlant = new GameObject("PlantItem");
        newPlant.transform.position = replicatePosition;
        
        // 添加PlantItem组件（不设置canReplicate，默认为false）
        newPlant.AddComponent<PlantItem>();
        
        Debug.Log($"PlantItem在位置 {replicatePosition} 复制了一个新的plant");
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
        
        // 移除所有物理组件
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
        {
            DestroyImmediate(sphereRb);
        }
        
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            DestroyImmediate(sphereCollider);
        }
        
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
        // 植物被销毁时减少总数量
        totalPlantCount--;
        UpdateEnvironmentalFood();
        Debug.Log($"植物被销毁，剩余植物数量: {totalPlantCount}, 环境食物数值: {environmentalFood}");
    }
}

// 蓝色小球点击处理脚本
public class BlueSphereClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        // 增加研究点数
        ResearchManager.researchPoints++;
        
        Debug.Log($"研究点数+1，当前研究点数: {ResearchManager.researchPoints}");
        
        // 销毁小球
        Destroy(gameObject);
    }
}
