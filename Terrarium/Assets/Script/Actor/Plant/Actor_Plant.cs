using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Plant : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    private PlantGrowthSystem growthSystem;
    private PlantReproduction reproductionSystem;
    
    public bool HasGrounded = false;
    public GameObject Ground;
    public GameObject FertileGround;
    public GameObject BarrenGround;
    public int GrowthStage = 0;
    public int GrowSpeed = 2;
    public bool EnvironmentalFactor = true;

    void Start()
    {
        SetupPlantAppearance();
        growthSystem = gameObject.AddComponent<PlantGrowthSystem>();
        reproductionSystem = gameObject.AddComponent<PlantReproduction>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        HandleGroundCollision(collision);
    }

    public void HandleGroundCollision(Collision collision) // 处理地面碰撞
    {
        if (!HasGrounded && IsGroundObject(collision.gameObject))
        {
            Debug.Log($"植物接触到地面: {collision.gameObject.name}");
            
            // 根据地面类型设置生长速度
            if (collision.gameObject.name.Contains("FertileGround") || collision.gameObject == FertileGround)
            {
                GrowSpeed = 3;
                Debug.Log("接触到肥沃土地，生长速度设置为3");
            }
            else if (collision.gameObject.name.Contains("BarrenGround") || collision.gameObject == BarrenGround)
            {
                GrowSpeed = 1;
                Debug.Log("接触到贫瘠土地，生长速度设置为1");
            }
            else
            {
                GrowSpeed = 2;
                Debug.Log("接触到普通地面，生长速度设置为2");
            }
            
            FixPosition();
        }
    }

    bool IsGroundObject(GameObject obj) // 判断是否是地面物体
    {
        return obj == Ground || 
               obj == FertileGround || 
               obj == BarrenGround ||
               obj.name.Contains("Ground") || 
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround");
    }

    void FixPosition() // 固定植物位置
    {
        Rigidbody rb = ((MonoBehaviour)this).gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            Object.Destroy(rb);
        HasGrounded = true;
        Debug.Log("植物已固定在地面上");
    }

    void Update()
    {
        if(HasGrounded && growthSystem != null && !growthSystem.IsGrowing)
        {
            Sprout();
        }
        if (EnvironmentalFactor == true && growthSystem.FirstGrowthDone == true && growthSystem.SecondGrowthDone == false)
            {
                growthSystem.SecondGrowth();
            }
        if (EnvironmentalFactor == true && growthSystem.SecondGrowthDone == true)
        {

        }
    }

    void Sprout()
    {
            growthSystem.StartGrowth();
            // 生长开始后停止重复调用
            HasGrounded = false; // 或者使用其他标志位      
        
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
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>(); // 添加碰撞器组件
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }
}


