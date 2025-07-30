using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResearchPoint
{
    void SpawnResearchPoint();
}

public class Actor_Plant : MonoBehaviour, IResearchPoint
{
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    private PlantGrowthSystem growthSystem;
    private PlantReproduction reproductionSystem;
    
    //Growth and Reproduction 
    public bool HasGrounded = false;
    public GameObject Ground;
    public GameObject FertileGround;
    public GameObject BarrenGround;
    public int GrowthStage = 0;
    public int GrowSpeed = 2;
    public bool EnvironmentalFactor = true;
    public int ReproductionCount = 0;
    public int MaxReproductionCount = 1;
    public bool SingleReproduction = false;
    public bool PluralReproduction = false;
    public bool ReproductionDone = false;
    
    private bool hasIncreasedPlantAmount = false; // 添加标志位

    public GameObject ResearchPoint;

    void Start()
    {
        SetupPlantAppearance();
        growthSystem = gameObject.GetComponent<PlantGrowthSystem>();
        reproductionSystem = gameObject.GetComponent<PlantReproduction>();
        growthSystem.OnFirstGrowthCompleted += OnFirstGrowthCompleted;
        growthSystem.OnSecondGrowthCompleted += OnSecondGrowthCompleted;

    }

    void Update()
    {
        
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
            
            // 优化地面类型检测 - 按优先级检测
            if (IsFertileGround(collision.gameObject))
            {
                GrowSpeed = 3;
                PluralReproduction = true;
                SingleReproduction = false;
                Debug.Log("接触到肥沃土地，生长速度设置为3");
            }
            else if (IsBarrenGround(collision.gameObject))
            {
                GrowSpeed = 1;
                SingleReproduction = true;
                PluralReproduction = false;
                Debug.Log("接触到贫瘠土地，生长速度设置为1");
            }
            else
            {
                GrowSpeed = 2;
                SingleReproduction = true;
                PluralReproduction = false;
                Debug.Log("接触到普通地面，生长速度设置为2");
            }
            
            FixPosition();
        }
    }

    // 分离地面类型检测方法
    bool IsFertileGround(GameObject obj)
    {
        return obj == FertileGround || 
               obj.name.Contains("FertileGround") ||
               obj.name.Equals("FertileGround") ||
               obj.GetComponent<Actor_FertileGround>() != null;
    }

    bool IsBarrenGround(GameObject obj)
    {
        return obj == BarrenGround || 
               obj.name.Contains("BarrenGround") ||
               obj.name.Equals("BarrenGround");
    }

    bool IsGroundObject(GameObject obj)
    {
        return obj == Ground || 
               obj == FertileGround || 
               obj == BarrenGround ||
               obj.name.Contains("Ground") || 
               obj.name.Contains("Plane") ||
               obj.CompareTag("Ground");
    }

    void FixPosition() // 固定植物位置
    {
        Rigidbody rb = ((MonoBehaviour)this).gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            Object.Destroy(rb);
        HasGrounded = true;
        Debug.Log("植物已固定在地面上");
        if(HasGrounded && !growthSystem.IsGrowing)
        {
            Sprout();
        }
    }

    void Sprout()
    {
        growthSystem.StartGrowth();
        // 生长开始后停止重复调用
        HasGrounded = false; // 或者使用其他标志位 
        // 只在第一次调用时增加植物数量
        if (!hasIncreasedPlantAmount)
        {
            ActorManager.PlantAmountIncrease();
            hasIncreasedPlantAmount = true;
        }
    }


    private void OnFirstGrowthCompleted()
    {
        Debug.Log("收到第一阶段生长完成通知");
        SecondGrowth();
    }

    void SecondGrowth()
    {
        if (EnvironmentalFactor == true && growthSystem.SecondGrowthDone == false)
        {
            growthSystem.SecondGrowth();
            
        }
    }

    private void OnSecondGrowthCompleted()
    {
        SpawnResearchPoint();
        Reproduction();
    }

    void Reproduction()
    {
        if (reproductionSystem == null)
            return;

        if (EnvironmentalFactor == true && growthSystem.SecondGrowthDone == true && ReproductionDone == false)
        {
            if (GrowSpeed == 1 && SingleReproduction == true){
            reproductionSystem.StartReproduction();
            ReproductionDone = true;
            }
            if (GrowSpeed == 2 && SingleReproduction == true ){
            reproductionSystem.StartReproduction();
            ReproductionDone = true;
            }
            if (GrowSpeed == 3 && PluralReproduction == true){
            reproductionSystem.StartDoubleReproduction();
            ReproductionDone = true;
            }
            
        }
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

    public void SpawnResearchPoint()
    {
        // 在植物上方生成蓝色研究点球体
        Vector3 spawnPosition = transform.position + Vector3.up * 3f;
        
        ResearchPoint = new GameObject("ResearchPoint");
        ResearchPoint.transform.position = spawnPosition;
        
        // 添加Actor_ResearchPoint组件
        ResearchPoint.AddComponent<Actor_ResearchPoint>();
    }
}

