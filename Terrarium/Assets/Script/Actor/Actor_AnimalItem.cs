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
        // 检查是否接触到地面（通过标签或名称判断）
        if (!hasGrounded && (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.name.Contains("Ground") || 
            collision.gameObject.name.Contains("Plane") ||
            collision.gameObject.name.Contains("BarrenGround")))
        {
            hasGrounded = true;
            
            // 完全固定动物位置，移除刚体组件
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                DestroyImmediate(rb);
            }
            
            Debug.Log("AnimalItem接触到地面，完全固定位置");
            
            // 移除落地时生成蓝色小球的代码
            
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
            
            // 如果还没有吃过植物，则吃掉植物并成长
            if (!hasEaten)
            {
                EatPlant();
            }
            
            Debug.Log("AnimalItem到达植物附近，停止移动");
        }
    }
    
    void EatPlant()
    {
        if (targetPlant != null && !hasEaten)
        {
            hasEaten = true;
            
            // 销毁植物
            PlantItem plantComponent = targetPlant.GetComponent<PlantItem>();
            if (plantComponent != null)
            {
                Debug.Log($"AnimalItem吃掉了植物: {targetPlant.name}");
                Destroy(targetPlant.gameObject);
            }
            
            // 动物成长为成年体
            StartCoroutine(GrowToAdult());
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
        
        // 移除刚体组件
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
        {
            DestroyImmediate(sphereRb);
        }
        
        // 移除碰撞器组件
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            DestroyImmediate(sphereCollider);
        }
        
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
}

// 动物蓝色小球点击处理脚本
public class AnimalBlueSphereClickHandler : MonoBehaviour
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
