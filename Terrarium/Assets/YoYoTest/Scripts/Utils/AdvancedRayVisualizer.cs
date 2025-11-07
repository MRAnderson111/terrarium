using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 高级射线可视化器，使用Graphics.DrawMesh绘制双层射线效果
/// </summary>
public class AdvancedRayVisualizer : MonoBehaviour
{
    [Header("射线设置")]
    [SerializeField] private float innerRayWidth = 0.01f; // 内层射线宽度
    [SerializeField] private float outerRayWidth = 0.03f; // 外层射线宽度
    [SerializeField] private Color hitColor = Color.green; // 击中时的颜色
    [SerializeField] private Color missColor = Color.red; // 未击中时的颜色
    [SerializeField] private float maxRayDistance = 10f; // 最大射线距离
    
    [Header("动画设置")]
    [SerializeField] private bool enableAnimation = true; // 是否启用动画
    [SerializeField] private float animationSpeed = 2f; // 动画速度
    [SerializeField] private float pulseIntensity = 0.3f; // 脉冲强度
    
    [Header("材质设置")]
    [SerializeField] private Material innerRayMaterial; // 内层射线材质
    [SerializeField] private Material outerRayMaterial; // 外层射线材质
    
    private Mesh rayMesh; // 射线网格
    private Matrix4x4[] rayMatrices = new Matrix4x4[2]; // 射线变换矩阵（内层和外层）
    private Material[] rayMaterials = new Material[2]; // 射线材质数组
    
    private bool isInitialized = false;
    private float animationTime = 0f;
    
    void Awake()
    {
        InitializeRayVisualizer();
    }
    
    /// <summary>
    /// 初始化射线可视化器
    /// </summary>
    private void InitializeRayVisualizer()
    {
        // 创建射线网格
        rayMesh = CreateRayMesh();
        
        // 如果没有指定材质，创建默认材质
        if (innerRayMaterial == null)
        {
            innerRayMaterial = CreateDefaultRayMaterial();
        }
        
        if (outerRayMaterial == null)
        {
            outerRayMaterial = CreateDefaultRayMaterial();
            // 设置外层材质为半透明
            Color outerColor = outerRayMaterial.color;
            outerColor.a = 0.5f;
            outerRayMaterial.color = outerColor;
        }
        
        rayMaterials[0] = innerRayMaterial;
        rayMaterials[1] = outerRayMaterial;
        
        isInitialized = true;
    }
    
    /// <summary>
    /// 创建射线网格
    /// </summary>
    private Mesh CreateRayMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "AdvancedRayMesh";
        
        // 创建一个圆柱体网格作为射线
        mesh = CreateCylinderMesh(0.5f, 8);
        
        return mesh;
    }
    
    /// <summary>
    /// 创建一个沿Z轴的单位长度圆柱体网格
    /// </summary>
    private Mesh CreateCylinderMesh(float radius, int segments)
    {
        Mesh mesh = new Mesh();

        int vertexCount = segments * 2 + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 12];

        // 顶点
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // 起点环
            vertices[i] = new Vector3(x, y, 0);
            // 终点环
            vertices[i + segments] = new Vector3(x, y, 1);
        }
        // 顶点 - 中心点
        vertices[segments * 2] = Vector3.zero; // 起点中心
        vertices[segments * 2 + 1] = Vector3.forward; // 终点中心

        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            // 侧面
            triangles[triIndex++] = i;
            triangles[triIndex++] = next;
            triangles[triIndex++] = i + segments;

            triangles[triIndex++] = next;
            triangles[triIndex++] = next + segments;
            triangles[triIndex++] = i + segments;

            // 封盖 - 起点
            triangles[triIndex++] = i;
            triangles[triIndex++] = segments * 2;
            triangles[triIndex++] = next;

            // 封盖 - 终点
            triangles[triIndex++] = next + segments;
            triangles[triIndex++] = segments * 2 + 1;
            triangles[triIndex++] = i + segments;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    
    /// <summary>
    /// 创建默认射线材质
    /// </summary>
    private Material CreateDefaultRayMaterial()
    {
        // 使用Unity内置的粒子着色器创建材质
        Material mat = new Material(Shader.Find("Sprites/Default"));
        return mat;
    }
    
    void Update()
    {
        if (enableAnimation)
        {
            animationTime += Time.deltaTime * animationSpeed;
        }
    }
    
    /// <summary>
    /// 绘制射线
    /// </summary>
    /// <param name="startPoint">射线起点</param>
    /// <param name="endPoint">射线终点</param>
    /// <param name="isHit">是否击中目标</param>
    public void DrawRay(Vector3 startPoint, Vector3 endPoint, bool isHit)
    {
        if (!isInitialized)
        {
            InitializeRayVisualizer();
        }
        
        // 计算射线的方向和距离
        Vector3 direction = (endPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, endPoint);
        
        // 设置颜色
        Color rayColor = isHit ? hitColor : missColor;
        
        // 动画效果
        float pulseFactor = 1f;
        if (enableAnimation)
        {
            pulseFactor = 1f + Mathf.Sin(animationTime) * pulseIntensity;
        }
        
        // 绘制内层射线
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 innerScale = new Vector3(innerRayWidth * pulseFactor, innerRayWidth * pulseFactor, distance);
        rayMatrices[0] = Matrix4x4.TRS(startPoint, rotation, innerScale);
        
        // 绘制外层射线
        Vector3 outerScale = new Vector3(outerRayWidth * pulseFactor, outerRayWidth * pulseFactor, distance);
        rayMatrices[1] = Matrix4x4.TRS(startPoint, rotation, outerScale);
        
        // 设置材质颜色
        rayMaterials[0].color = rayColor;
        
        Color outerColor = rayColor;
        outerColor.a = 0.5f; // 外层半透明
        rayMaterials[1].color = outerColor;
        
        // 绘制内层射线
        Graphics.DrawMesh(
            rayMesh, 
            rayMatrices[0], 
            rayMaterials[0], 
            0, // layer
            null, // camera (null表示所有相机)
            0, // submeshIndex
            null, // properties
            true, // castShadows
            true  // receiveShadows
        );
        
        // 绘制外层射线
        Graphics.DrawMesh(
            rayMesh, 
            rayMatrices[1], 
            rayMaterials[1], 
            0, // layer
            null, // camera (null表示所有相机)
            0, // submeshIndex
            null, // properties
            true, // castShadows
            true  // receiveShadows
        );
    }
    
    /// <summary>
    /// 绘制从起点到指定方向的射线
    /// </summary>
    /// <param name="startPoint">射线起点</param>
    /// <param name="direction">射线方向</param>
    /// <param name="distance">射线距离</param>
    /// <param name="isHit">是否击中目标</param>
    public void DrawRay(Vector3 startPoint, Vector3 direction, float distance, bool isHit)
    {
        Vector3 endPoint = startPoint + direction * distance;
        DrawRay(startPoint, endPoint, isHit);
    }
    
    /// <summary>
    /// 设置射线颜色
    /// </summary>
    /// <param name="hitColor">击中时的颜色</param>
    /// <param name="missColor">未击中时的颜色</param>
    public void SetRayColors(Color hitColor, Color missColor)
    {
        this.hitColor = hitColor;
        this.missColor = missColor;
    }
    
    /// <summary>
    /// 设置射线宽度
    /// </summary>
    /// <param name="innerWidth">内层射线宽度</param>
    /// <param name="outerWidth">外层射线宽度</param>
    public void SetRayWidths(float innerWidth, float outerWidth)
    {
        this.innerRayWidth = innerWidth;
        this.outerRayWidth = outerWidth;
    }
    
    /// <summary>
    /// 设置动画参数
    /// </summary>
    /// <param name="enable">是否启用动画</param>
    /// <param name="speed">动画速度</param>
    /// <param name="intensity">脉冲强度</param>
    public void SetAnimationParameters(bool enable, float speed, float intensity)
    {
        this.enableAnimation = enable;
        this.animationSpeed = speed;
        this.pulseIntensity = intensity;
    }
    
    void OnDestroy()
    {
        // 清理资源
        if (rayMesh != null)
        {
            Destroy(rayMesh);
        }
        
        if (innerRayMaterial != null && innerRayMaterial != null)
        {
            Destroy(innerRayMaterial);
        }
        
        if (outerRayMaterial != null && outerRayMaterial != null)
        {
            Destroy(outerRayMaterial);
        }
    }
}