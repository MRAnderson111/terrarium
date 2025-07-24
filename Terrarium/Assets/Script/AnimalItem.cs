using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalItem : MonoBehaviour
{
    private static bool keyProcessedThisFrame = false; // 静态变量控制每帧只处理一次
    private bool hasGrounded = false; // 是否已经接触地面
    
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
            
            // 立即生成蓝色小球
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
