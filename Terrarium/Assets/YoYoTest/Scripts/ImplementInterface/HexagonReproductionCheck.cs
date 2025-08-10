using UnityEngine;
using System.Collections;

public class HexagonReproductionCheck : MonoBehaviour, IReproductionCheck
{
    [Header("预制体路径")]
    public string reproductionPrefabPath = "Prefabs/"; // Resources文件夹下的预制体路径

    [Header("检测参数")]
    public float checkDistance = 1f; // 检测距离
    public float sphereRadius = 0.5f; // 检测球体半径
    public bool drawDebugSphere = false; // 是否绘制调试球体

    [Header("繁殖控制")]
    public string smallClass = null;
    public float coolDownTime = 3f; // 冷却时间
    public bool enableReproductionLogging = false; // 是否启用繁殖日志

    private GameObject reproductionPrefab; // 缓存加载的预制体

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        smallClass = GetComponent<IGetObjectClass>().SmallClass;
    }

    private void Update()
    {
        ReproductionCheck();
    }
    
    /// <summary>
    /// 从Resources文件夹加载预制体
    /// </summary>
    /// <returns>是否成功加载预制体</returns>
    private bool LoadReproductionPrefab()
    {
        if (reproductionPrefab != null)
        {
            return true; // 已经加载过了
        }

        if (string.IsNullOrEmpty(reproductionPrefabPath))
        {
            Debug.LogError("预制体路径为空，无法加载预制体");
            return false;
        }

        reproductionPrefab = Resources.Load<GameObject>(reproductionPrefabPath);

        if (reproductionPrefab == null)
        {
            Debug.LogError($"无法从路径 '{reproductionPrefabPath}' 加载预制体，请检查路径是否正确且预制体存在于Resources文件夹中");
            return false;
        }

        Debug.Log($"成功加载预制体：{reproductionPrefab.name} 从路径：{reproductionPrefabPath}");
        return true;
    }

    public void ReproductionCheck()
    {
        // 检查全局冷却时间是否就绪
        if (ObjectStatisticsManager.Instance.IsGlobalCoolDownReady(smallClass, coolDownTime))
        {
            // 检查当前数量是否已经达到限制
            ObjectStatisticsManager statsManager = FindObjectOfType<ObjectStatisticsManager>();
            if (statsManager != null)
            {
                int currentCount = 0;
                if (statsManager.smallClassCount.ContainsKey(smallClass))
                {
                    currentCount = statsManager.smallClassCount[smallClass];
                }
                
                // 获取最新的数量限制值
                int currentQuantityLimit = CreateManager.Instance.GetCurrentQuantityLimit(smallClass);
                
                // 检查是否超过数量限制
                if (currentCount >= currentQuantityLimit)
                {
                    if (enableReproductionLogging)
                    {
                        Debug.Log($"小类 {smallClass} 当前数量 {currentCount} 已达到限制 {currentQuantityLimit}，无法繁殖");
                    }
                    return;
                }
            }
            
            // 执行繁殖检测
            if (PerformSingleReproductionCheck())
            {
                // 如果成功，则重置全局冷却时间
                ObjectStatisticsManager.Instance.ResetGlobalCoolDown(smallClass);
                if (enableReproductionLogging)
                {
                    Debug.Log($"小类 {smallClass} 的全局冷却时间已重置为0");
                }
            }
        }
    }

    /// <summary>
    /// 执行单次繁殖检测
    /// </summary>
    /// <returns>是否成功繁殖</returns>
    private bool PerformSingleReproductionCheck()
    {
        // 尝试加载预制体
        if (!LoadReproductionPrefab())
        {
            if (enableReproductionLogging)
            {
                Debug.LogError("无法加载繁殖预制体，跳过本次检测");
            }
            return false;
        }

        // 调用静态工具类进行六边形球形检测
        if (SphereDetectionUtility.PerformHexagonSphereDetection(
            transform.position,
            out Vector3 emptyPosition,
            checkDistance,
            sphereRadius,
            drawDebugSphere
        ))
        {
            // 找到空位置，使用CreateManager生成预制体
            if (CreateManager.Instance != null)
            {
                CreateManager.Instance.CreatePrefab(reproductionPrefab, emptyPosition);
                if (enableReproductionLogging)
                {
                    Debug.Log($"使用CreateManager在位置 {emptyPosition} 生成了预制体：{reproductionPrefab.name}");
                }
                return true; // 成功繁殖
            }
            else
            {
                if (enableReproductionLogging)
                {
                    Debug.LogError("CreateManager引用为空，无法生成预制体");
                }
                return false; // 未成功繁殖
            }
        }
        else
        {
            if (enableReproductionLogging)
            {
                Debug.Log("没有找到合适的空位置进行繁殖");
            }
            return false; // 未成功繁殖
        }
    }

    /// <summary>
    /// 停止繁殖检测循环
    /// </summary>
    public void StopReproductionLoop()
    {
        // 由于不再使用协程，此方法可以保留为空或移除
    }

    /// <summary>
    /// 当对象被销毁时停止协程
    /// </summary>
    private void OnDestroy()
    {
        // 由于不再使用协程，此方法可以保留为空或移除
    }
}
