using UnityEngine;

/// <summary>
/// 性能管理器 - 全局控制各种性能相关的设置
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    [Header("全局性能设置")]
    [Tooltip("启用详细的调试日志输出（会显著影响性能）")]
    public bool enableDetailedLogging = false;
    
    [Tooltip("启用调试球体显示（会影响性能和内存）")]
    public bool enableDebugSpheres = false;
    
    [Tooltip("启用繁殖系统日志")]
    public bool enableReproductionLogging = false;
    
    [Header("性能监控")]
    [Tooltip("显示性能统计信息")]
    public bool showPerformanceStats = true;
    
    [Tooltip("性能统计更新间隔（秒）")]
    public float statsUpdateInterval = 1f;
    
    // 性能统计
    private float frameTime;
    private float fps;
    private int reproductionObjectCount;
    private float lastStatsUpdate;
    
    // 单例实例
    private static PerformanceManager instance;
    public static PerformanceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PerformanceManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PerformanceManager");
                    instance = go.AddComponent<PerformanceManager>();
                    Debug.Log("创建了新的 PerformanceManager 实例");
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        // 确保只有一个实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyPerformanceSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        ApplyPerformanceSettings();
    }
    
    void Update()
    {
        // 更新性能统计
        if (showPerformanceStats && Time.time - lastStatsUpdate >= statsUpdateInterval)
        {
            UpdatePerformanceStats();
            lastStatsUpdate = Time.time;
        }
        
        // 检测设置变化并应用
        if (Application.isEditor)
        {
            ApplyPerformanceSettings();
        }
    }
    
    /// <summary>
    /// 应用性能设置到各个系统
    /// </summary>
    public void ApplyPerformanceSettings()
    {
        // 应用到球形检测工具
        SphereDetectionUtility.enableDetailedLogging = enableDetailedLogging;
        SphereDetectionUtility.enableDebugSpheres = enableDebugSpheres;
        
        // 应用到所有繁殖检测组件
        SimpleReproductionCheck[] reproductionChecks = FindObjectsOfType<SimpleReproductionCheck>();
        foreach (var check in reproductionChecks)
        {
            check.enableReproductionLogging = enableReproductionLogging;
        }
        
        reproductionObjectCount = reproductionChecks.Length;
    }
    
    /// <summary>
    /// 更新性能统计信息
    /// </summary>
    private void UpdatePerformanceStats()
    {
        frameTime = Time.deltaTime * 1000f; // 转换为毫秒
        fps = 1f / Time.deltaTime;
        
        // 更新繁殖对象数量
        reproductionObjectCount = FindObjectsOfType<SimpleReproductionCheck>().Length;
    }
    
    /// <summary>
    /// 获取性能统计信息
    /// </summary>
    public string GetPerformanceStats()
    {
        return $"FPS: {fps:F1} | Frame Time: {frameTime:F1}ms | Reproduction Objects: {reproductionObjectCount}";
    }
    
    /// <summary>
    /// 优化建议
    /// </summary>
    public void LogPerformanceRecommendations()
    {
        Debug.Log("=== 性能优化建议 ===");
        
        if (enableDetailedLogging)
        {
            Debug.LogWarning("建议：关闭详细日志以提高性能");
        }
        
        if (enableDebugSpheres)
        {
            Debug.LogWarning("建议：关闭调试球体以节省内存和提高性能");
        }
        
        if (reproductionObjectCount > 50)
        {
            Debug.LogWarning($"建议：繁殖对象数量较多({reproductionObjectCount})，考虑增加繁殖间隔时间");
        }
        
        if (fps < 30)
        {
            Debug.LogWarning($"性能警告：当前FPS较低({fps:F1})，建议优化设置");
        }
        
        Debug.Log("=== 优化建议结束 ===");
    }
    
    void OnGUI()
    {
        if (!showPerformanceStats) return;
        
        // 在屏幕左上角显示性能信息
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 400, 20), GetPerformanceStats());
        
        // 性能警告
        if (fps < 30)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 30, 400, 20), "性能警告：FPS过低！");
        }
        else if (fps < 60)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, 30, 400, 20), "性能提示：FPS偏低");
        }
        
        // 快捷按钮
        GUI.color = Color.white;
        if (GUI.Button(new Rect(10, 60, 120, 30), "优化建议"))
        {
            LogPerformanceRecommendations();
        }
        
        if (GUI.Button(new Rect(140, 60, 120, 30), "应用设置"))
        {
            ApplyPerformanceSettings();
        }
    }
}
