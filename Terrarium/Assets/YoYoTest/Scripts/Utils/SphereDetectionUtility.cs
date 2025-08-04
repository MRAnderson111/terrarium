using UnityEngine;

/// <summary>
/// 球形检测工具类，提供静态方法进行球形碰撞检测和可视化
/// </summary>
public static class SphereDetectionUtility
{
    private static readonly Collider[] colliderBuffer = new Collider[10]; // 预分配缓冲区
    private static Material debugMaterial; // 缓存调试材质，避免重复创建

    // 性能优化开关
    public static bool enableDetailedLogging = false; // 默认关闭详细日志
    public static bool enableDebugSpheres = false; // 默认关闭调试球体
    
    /// <summary>
    /// 执行四个方向的球形检测，找到第一个空位置
    /// </summary>
    /// <param name="centerPosition">检测中心位置</param>
    /// <param name="checkDistance">检测距离</param>
    /// <param name="sphereRadius">检测球体半径</param>
    /// <param name="drawDebugSphere">是否绘制调试球体</param>
    /// <param name="emptyPosition">输出参数：找到的空位置</param>
    /// <returns>是否找到空位置</returns>
    public static bool PerformDirectionalSphereDetection(Vector3 centerPosition, out Vector3 emptyPosition, float checkDistance = 1f, float sphereRadius = 0.5f, bool drawDebugSphere = false)
    {
        // 初始化输出参数
        emptyPosition = Vector3.zero;

        // 定义四个方向：前后左右
        Vector3[] directions = {
            Vector3.forward,  // 前
            Vector3.back,     // 后
            Vector3.left,     // 左
            Vector3.right     // 右
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 checkPosition = centerPosition + direction * checkDistance;

            // 只在启用详细日志时输出
            if (enableDetailedLogging)
            {
                Debug.Log($"=== 开始检测 {direction} 方向 ===");
                Debug.Log($"检测位置: {checkPosition}, 检测半径: {sphereRadius}");
            }

            // 使用OverlapSphereNonAlloc避免内存分配
            int colliderCount = Physics.OverlapSphereNonAlloc(checkPosition, sphereRadius, colliderBuffer);

            if (enableDetailedLogging)
            {
                Debug.Log($"在 {direction} 方向检测到 {colliderCount} 个碰撞体");
                // 详细记录每个检测到的碰撞体信息
                LogColliderDetails(colliderCount);
            }

            // 根据布尔值决定是否生成可视化球体
            if (drawDebugSphere && enableDebugSpheres)
            {
                CreateDebugSphere(checkPosition, direction);
            }

            // 检查是否有实现IGetObjectClass接口的对象
            if (!CheckForValidObjects(colliderCount))
            {
                if (enableDetailedLogging)
                {
                    Debug.Log($"=== 在 {direction} 方向没有检测到实现IGetObjectClass接口的对象，找到空位置 ===");
                }
                emptyPosition = checkPosition;
                return true;
            }
        }

        if (enableDetailedLogging)
        {
            Debug.Log("四个方向都有物体但没有找到空位置");
        }
        return false;
    }
    
    /// <summary>
    /// 记录碰撞体详细信息（仅在详细日志模式下使用）
    /// </summary>
    private static void LogColliderDetails(int colliderCount)
    {
        if (!enableDetailedLogging) return;

        for (int i = 0; i < colliderCount; i++)
        {
            Collider col = colliderBuffer[i];
            if (col != null)
            {
                string objectName = col.gameObject.name;
                string colliderType = col.GetType().Name;

                Debug.Log($"  碰撞体 {i + 1}: 名称='{objectName}', 类型={colliderType}");

                // 简化的组件检查，避免性能开销
                if (col.attachedRigidbody != null)
                {
                    Debug.Log($"    - 附加刚体: {col.attachedRigidbody.name}");
                }
                else
                {
                    Debug.Log($"    - 无附加刚体");
                }
            }
            else
            {
                Debug.LogWarning($"  碰撞体 {i + 1}: null");
            }
        }
    }
    
    /// <summary>
    /// 创建调试球体（使用缓存材质避免内存泄漏）
    /// </summary>
    private static void CreateDebugSphere(Vector3 position, Vector3 direction)
    {
        if (!enableDebugSpheres) return;

        GameObject visualSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualSphere.name = $"DebugSphere_{direction}";
        visualSphere.transform.position = position;
        visualSphere.transform.localScale = Vector3.one * 1f;

        if (enableDetailedLogging)
        {
            Debug.Log($"生成可视化球体: {visualSphere.name} 在位置 {position}");
        }

        // 使用缓存的材质，避免重复创建
        Renderer renderer = visualSphere.GetComponent<Renderer>();
        if (debugMaterial == null)
        {
            debugMaterial = new Material(Shader.Find("Standard"));
            debugMaterial.color = new Color(1f, 0f, 0f, 0.5f); // 半透明红色
            debugMaterial.SetFloat("_Mode", 3); // 设置为透明模式
            debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            debugMaterial.SetInt("_ZWrite", 0);
            debugMaterial.DisableKeyword("_ALPHATEST_ON");
            debugMaterial.EnableKeyword("_ALPHABLEND_ON");
            debugMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            debugMaterial.renderQueue = 3000;
        }
        renderer.material = debugMaterial;

        // 使用Unity的Destroy方法在10秒后自动销毁，防止调试球体积累
        Object.Destroy(visualSphere, 10f);
    }

    /// <summary>
    /// 检查是否有实现IGetObjectClass接口的对象（优化版本）
    /// </summary>
    private static bool CheckForValidObjects(int colliderCount)
    {
        for (int i = 0; i < colliderCount; i++)
        {
            Collider col = colliderBuffer[i];
            if (col != null && col.attachedRigidbody != null)
            {
                if (col.attachedRigidbody.TryGetComponent<IGetObjectClass>(out _))
                {
                    if (enableDetailedLogging)
                    {
                        Debug.Log($"  找到实现IGetObjectClass接口的对象: {col.attachedRigidbody.name}");
                    }
                    return true; // 找到一个就返回，提高性能
                }
            }
        }

        return false; // 没有找到任何实现接口的对象
    }
}
