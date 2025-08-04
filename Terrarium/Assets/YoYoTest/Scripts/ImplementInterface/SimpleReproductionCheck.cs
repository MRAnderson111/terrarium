using UnityEngine;
using System.Collections;

public class SimpleReproductionCheck : MonoBehaviour, IReproductionCheck
{
    [Header("预制体路径")]
    public string reproductionPrefabPath = "Prefabs/"; // Resources文件夹下的预制体路径

    [Header("检测参数")]
    public float checkDistance = 1f; // 检测距离
    public float sphereRadius = 0.5f; // 检测球体半径
    public bool drawDebugSphere = false; // 是否绘制调试球体

    [Header("繁殖控制")]
    public float reproductionInterval = 3f; // 繁殖检测间隔（秒）
    public bool enableReproductionLogging = false; // 是否启用繁殖日志
    private bool isReproductionActive = false; // 是否正在进行繁殖检测循环

    private GameObject reproductionPrefab; // 缓存加载的预制体

    /// <summary>
    /// 初始化，检查CreateManager实例
    /// </summary>
    private void Start()
    {
        // 检查CreateManager实例是否存在
        if (CreateManager.Instance == null)
        {
            Debug.LogError("场景中未找到CreateManager，无法使用其生成方法");
        }
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
        // 如果繁殖检测循环还没有开始，则启动它
        if (!isReproductionActive)
        {
            StartCoroutine(ReproductionLoop());
        }
    }

    /// <summary>
    /// 繁殖检测循环，每隔指定时间进行一次检测
    /// </summary>
    private IEnumerator ReproductionLoop()
    {
        isReproductionActive = true;
        if (enableReproductionLogging)
        {
            Debug.Log("开始繁殖检测循环，每隔" + reproductionInterval + "秒检测一次");
        }

        while (true)
        {
            // 执行一次繁殖检测
            PerformSingleReproductionCheck();

            // 等待指定的时间间隔
            yield return new WaitForSeconds(reproductionInterval);
        }
    }

    /// <summary>
    /// 执行单次繁殖检测
    /// </summary>
    private void PerformSingleReproductionCheck()
    {
        // 尝试加载预制体
        if (!LoadReproductionPrefab())
        {
            if (enableReproductionLogging)
            {
                Debug.LogError("无法加载繁殖预制体，跳过本次检测");
            }
            return;
        }

        // 调用静态工具类进行球形检测
        if (SphereDetectionUtility.PerformDirectionalSphereDetection(
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
            }
            else
            {
                if (enableReproductionLogging)
                {
                    Debug.LogError("CreateManager引用为空，无法生成预制体");
                }
            }
        }
        else
        {
            if (enableReproductionLogging)
            {
                Debug.Log("没有找到合适的空位置进行繁殖，将在" + reproductionInterval + "秒后重新检测");
            }
        }
    }

    /// <summary>
    /// 停止繁殖检测循环
    /// </summary>
    public void StopReproductionLoop()
    {
        if (isReproductionActive)
        {
            StopAllCoroutines();
            isReproductionActive = false;
            Debug.Log("繁殖检测循环已停止");
        }
    }

    /// <summary>
    /// 当对象被销毁时停止协程
    /// </summary>
    private void OnDestroy()
    {
        StopReproductionLoop();
    }
}
