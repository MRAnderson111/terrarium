using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Plant : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material plantMaterial;

    void Start()
    {
        SetupPlantAppearance();
        
       
    }

    void Update()
    {
        
    }

    void SetupPlantAppearance()
    {
        // 添加MeshFilter和MeshRenderer组件
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // 设置为立方体网格
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建绿色材质
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
}
