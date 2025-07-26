using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_FertileGround : MonoBehaviour
{
    void Start()
    {
        SetupFertileGround();
    }
    
    void SetupFertileGround()
    {
        // 设置对象名称，确保被其他脚本识别为FertileGround
        gameObject.name = "FertileGround";
        
        // 添加标签（如果存在Ground标签）
        if (GameObject.FindWithTag("Ground") != null)
        {
            gameObject.tag = "Ground";
        }
        
        // 确保有MeshRenderer和MeshFilter组件
        if (!TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        if (!TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        
        // 创建平面网格
        meshFilter.mesh = CreateGroundMesh();
        
        // 加载并应用FertileGround材质
        Material fertileMaterial = Resources.Load<Material>("FertileGround");
        if (fertileMaterial == null)
        {
            // 如果无法从Resources加载，尝试查找现有材质
            fertileMaterial = FindFertileGroundMaterial();
        }
        
        if (fertileMaterial != null)
        {
            meshRenderer.material = fertileMaterial;
            Debug.Log("成功应用FertileGround材质");
        }
        else
        {
            // 如果找不到材质，创建一个绿色材质作为替代
            Material fallbackMaterial = new(Shader.Find("Standard"))
            {
                color = new Color(0.7347665f, 1f, 0f, 1f) // 与FertileGround.mat相同的颜色
            };
            meshRenderer.material = fallbackMaterial;
            Debug.LogWarning("未找到FertileGround材质，使用替代绿色材质");
        }
        
        // 设置物理碰撞器
        SetupCollider();
        
        Debug.Log("FertileGround富饶土地设置完成");
    }
    
    Mesh CreateGroundMesh()
    {
        // 创建一个平面网格
        Mesh mesh = new();
        
        // 顶点坐标（10x10的平面）
        Vector3[] vertices = new Vector3[4]
        {
            new(-5f, 0f, -5f),
            new(5f, 0f, -5f),
            new(-5f, 0f, 5f),
            new(5f, 0f, 5f)
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
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    Material FindFertileGroundMaterial()
    {
        // 尝试在场景中查找现有的FertileGround材质
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
        
        foreach (MeshRenderer renderer in allRenderers)
        {
            if (renderer.material != null && renderer.material.name.Contains("FertileGround"))
            {
                return renderer.material;
            }
        }
        
        return null;
    }
    
    void SetupCollider()
    {
        // 添加Box碰撞器
        if (!TryGetComponent<BoxCollider>(out var boxCollider))
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // 设置碰撞器大小匹配地面
        boxCollider.size = new Vector3(10f, 0.1f, 10f);
        boxCollider.center = new Vector3(0f, 0f, 0f);
        boxCollider.isTrigger = false; // 保持物理碰撞
        
        Debug.Log("FertileGround碰撞器设置完成");
    }
}
