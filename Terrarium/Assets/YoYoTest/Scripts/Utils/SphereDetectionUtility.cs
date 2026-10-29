using UnityEngine;

/// <summary>
/// 球形检测工具类，提供静态方法进行球形碰撞检测和可视化
/// </summary>
public static class SphereDetectionUtility
{
    private static readonly Collider[] colliderBuffer = new Collider[10]; // 预分配缓冲区
    private static Material debugMaterial; // 缓存调试材质，避免重复创建
    public static bool enableDebugSpheres = false; // 默认关闭调试球体

    // 地面检测设置
    public static float groundCheckDistance = 10f; // 向下检测地面的距离
    public static string groundTag = "Ground"; // 地面标签
    
    // 精确地面检测设置
    public static float preciseGroundCheckDistance = 2f; // 精确地面检测距离
    public static float preciseGroundCheckOffset = 0.5f; // 精确地面检测的向上偏移量
    
    /// <summary>
    /// 执行四个方向的球形检测，找到第一个空位置
    /// </summary>
    /// <param name="centerPosition">检测中心位置</param>
    /// <param name="checkDistance">检测距离</param>
    /// <param name="sphereRadius">检测球体半径</param>
    /// <param name="drawDebugSphere">是否绘制调试球体</param>
    /// <param name="emptyPosition">输出参数：找到的空位置</param>
    /// <returns>是否找到空位置</returns>
    public static bool PerformDirectionalSphereDetection(Vector3 centerPosition, out Vector3 emptyPosition, float checkDistance = 1f, float sphereRadius = 0.5f, bool drawDebugSphere = false, bool returnGroundHitPoint = false)
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

        // 随机化方向数组的顺序，使每次检测的方向都是随机的
        ShuffleArray(directions);

        foreach (Vector3 direction in directions)
        {
            Vector3 checkPosition = centerPosition + direction * checkDistance;

            // 使用OverlapSphereNonAlloc避免内存分配
            int colliderCount = Physics.OverlapSphereNonAlloc(checkPosition, sphereRadius, colliderBuffer);

            // 根据布尔值决定是否生成可视化球体
            if (drawDebugSphere && enableDebugSpheres)
            {
                CreateDebugSphere(checkPosition, direction);
            }

            // 检查是否有实现IGetObjectClass接口的对象
            if (!CheckForValidObjects(colliderCount))
            {

                // 第一步：使用原有的地面检测（粗略检测）
                if (CheckGroundBelow(checkPosition))
                {
                    
                    // 第二步：进行精确的射线检测
                    if (CheckGroundWithPreciseRaycast(checkPosition, out Vector3 groundHitPoint))
                    {
                        
                        // 根据参数决定返回哪个位置
                        emptyPosition = returnGroundHitPoint ? groundHitPoint : checkPosition;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 执行六个方向的六边形球形检测，找到第一个空位置
    /// </summary>
    /// <param name="centerPosition">检测中心位置</param>
    /// <param name="checkDistance">检测距离</param>
    /// <param name="sphereRadius">检测球体半径</param>
    /// <param name="drawDebugSphere">是否绘制调试球体</param>
    /// <param name="emptyPosition">输出参数：找到的空位置</param>
    /// <returns>是否找到空位置</returns>
    public static bool PerformHexagonSphereDetection(Vector3 centerPosition, out Vector3 emptyPosition, float checkDistance = 1f, float sphereRadius = 0.5f, bool drawDebugSphere = false, bool returnGroundHitPoint = false)
    {
        // 初始化输出参数
        emptyPosition = Vector3.zero;

        // 定义六个方向的六边形检测：前、后、左前、右前、左后、右后
        Vector3[] directions = {
            Vector3.forward,                    // 前
            Vector3.back,                       // 后
            (Vector3.forward + Vector3.left).normalized,   // 左前
            (Vector3.forward + Vector3.right).normalized,  // 右前
            (Vector3.back + Vector3.left).normalized,      // 左后
            (Vector3.back + Vector3.right).normalized      // 右后
        };

        // 随机化方向数组的顺序，使每次检测的方向都是随机的
        ShuffleArray(directions);

        foreach (Vector3 direction in directions)
        {
            Vector3 checkPosition = centerPosition + direction * checkDistance;

            // 使用OverlapSphereNonAlloc避免内存分配
            int colliderCount = Physics.OverlapSphereNonAlloc(checkPosition, sphereRadius, colliderBuffer);

            // 根据布尔值决定是否生成可视化球体
            if (drawDebugSphere && enableDebugSpheres)
            {
                CreateDebugSphere(checkPosition, direction);
            }

            // 检查是否有实现IGetObjectClass接口的对象
            if (!CheckForValidObjects(colliderCount))
            {

                // 第一步：使用原有的地面检测（粗略检测）
                if (CheckGroundBelow(checkPosition))
                {
                    
                    // 第二步：进行精确的射线检测
                    if (CheckGroundWithPreciseRaycast(checkPosition, out Vector3 groundHitPoint))
                    {
                        
                        // 根据参数决定返回哪个位置
                        emptyPosition = returnGroundHitPoint ? groundHitPoint : checkPosition;
                        return true;
                    }
                }
            }
        }
        return false;
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
                    return true; // 找到一个就返回，提高性能
                }
            }
        }

        return false; // 没有找到任何实现接口的对象
    }

    /// <summary>
    /// 检查指定位置下方是否有地面（通过标签检测）
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否检测到地面</returns>
    private static bool CheckGroundBelow(Vector3 position)
    {
        // 从指定位置向下发射射线
        Vector3 rayOrigin = position;
        Vector3 rayDirection = Vector3.down;

        // 发射射线检测地面
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, groundCheckDistance))
        {

            // 检查击中的物体是否有Ground标签
            if (hit.collider.CompareTag(groundTag))
            {
                return true;
            }
        }

        return false; // 没有找到合适的地面
    }
    
    /// <summary>
    /// 使用精确射线检测检查指定位置下方是否有地面
    /// 从位置上方偏移一定距离后向下发射射线，提供更精确的地面检测
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <param name="groundHitPoint">输出参数：射线击中地面的位置</param>
    /// <returns>是否检测到地面</returns>
    private static bool CheckGroundWithPreciseRaycast(Vector3 position, out Vector3 groundHitPoint)
    {
        // 初始化输出参数
        groundHitPoint = Vector3.zero;
        
        // 从位置上方偏移一定距离开始发射射线，避免从地面内部开始检测
        Vector3 rayOrigin = position + Vector3.up * preciseGroundCheckOffset;
        Vector3 rayDirection = Vector3.down;
        
        // 发射射线检测地面
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, preciseGroundCheckDistance))
        {
            
            // 检查击中的物体是否有Ground标签
            if (hit.collider.CompareTag(groundTag))
            {
                groundHitPoint = hit.point;
                return true;
            }
        }
        
        return false; // 没有找到合适的地面
    }

    /// <summary>
    /// 使用Fisher-Yates洗牌算法随机化数组顺序
    /// </summary>
    /// <param name="array">要随机化的数组</param>
    private static void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
