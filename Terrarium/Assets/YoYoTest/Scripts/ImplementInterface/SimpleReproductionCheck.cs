using UnityEngine;

public class SimpleReproductionCheck : MonoBehaviour, IReproductionCheck
{
    [Header("预制体路径")]
    public string reproductionPrefabPath = "Prefabs/"; // Resources文件夹下的预制体路径

    [Header("检测参数")]
    public float checkDistance = 1f; // 检测距离
    public float sphereRadius = 0.5f; // 检测球体半径
    public bool drawDebugSphere = false; // 是否绘制调试球体

    [Header("繁殖控制")]
    private bool hasReproduced = false; // 是否已经繁殖过

    private GameObject reproductionPrefab; // 缓存加载的预制体
    private CreateManager createManager; // 引用CreateManager

    /// <summary>
    /// 初始化，获取CreateManager引用
    /// </summary>
    private void Start()
    {
        // 获取CreateManager实例
        createManager = FindObjectOfType<CreateManager>();
        if (createManager == null)
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
        // 检查是否已经繁殖过
        if (hasReproduced)
        {
            Debug.Log("该对象已经繁殖过，无法再次繁殖");
            return;
        }

        // 尝试加载预制体
        if (!LoadReproductionPrefab())
        {
            Debug.LogError("无法加载繁殖预制体，停止繁殖检测");
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
            if (createManager != null)
            {
                GameObject newObject = createManager.CreatePrefab(reproductionPrefab, emptyPosition);
                Debug.Log($"使用CreateManager在位置 {emptyPosition} 生成了预制体：{reproductionPrefab.name}");
            }
            else
            {
                Debug.LogError("CreateManager引用为空，无法生成预制体");
            }

            // 标记为已繁殖
            hasReproduced = true;
            Debug.Log("繁殖完成，该对象不能再次繁殖");
        }
        else
        {
            Debug.Log("没有找到合适的空位置进行繁殖");
        }
    }
}
