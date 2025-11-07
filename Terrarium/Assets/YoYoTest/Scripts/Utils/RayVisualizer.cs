using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用Graphics.DrawMesh绘制射线的可视化组件
/// </summary>
public class RayVisualizer : MonoBehaviour
{
    [Header("射线设置")]
    [SerializeField] private float rayWidth = 0.02f; // 射线宽度
    [SerializeField] private Color hitColor = Color.green; // 击中时的颜色
    [SerializeField] private Color missColor = Color.red; // 未击中时的颜色
    [SerializeField] private float maxRayDistance = 10f; // 最大射线距离
    
    [Header("材质设置")]
    [SerializeField] private Material rayMaterial; // 射线材质
    
    private Mesh rayMesh; // 射线网格
    private Matrix4x4[] rayMatrices = new Matrix4x4[1]; // 射线变换矩阵
    private Material[] rayMaterials = new Material[1]; // 射线材质数组
    
    private bool isInitialized = false;
    
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
        if (rayMaterial == null)
        {
            rayMaterial = CreateDefaultRayMaterial();
        }
        
        rayMaterials[0] = rayMaterial;
        
        isInitialized = true;
    }
    
    /// <summary>
    /// 创建射线网格
    /// </summary>
    private Mesh CreateRayMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "RayMesh";
        
        // 创建一个简单的线段网格
        Vector3[] vertices = new Vector3[2];
        vertices[0] = Vector3.zero; // 起点
        vertices[1] = Vector3.forward; // 终点（前方1单位）
        
        mesh.vertices = vertices;
        
        // 创建索引
        int[] indices = new int[2];
        indices[0] = 0;
        indices[1] = 1;
        
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        
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
        
        // 创建变换矩阵
        // 位置：起点
        // 旋转：朝向终点
        // 缩放：长度为distance，宽度和高度为rayWidth
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 scale = new Vector3(rayWidth, rayWidth, distance);
        
        rayMatrices[0] = Matrix4x4.TRS(startPoint, rotation, scale);
        
        // 设置颜色
        Color rayColor = isHit ? hitColor : missColor;
        rayMaterials[0].color = rayColor;
        
        // 绘制网格
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
    /// <param name="width">射线宽度</param>
    public void SetRayWidth(float width)
    {
        this.rayWidth = width;
    }
    
    void OnDestroy()
    {
        // 清理资源
        if (rayMesh != null)
        {
            Destroy(rayMesh);
        }
        
        if (rayMaterial != null && rayMaterial != null)
        {
            Destroy(rayMaterial);
        }
    }
}