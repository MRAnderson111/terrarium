using UnityEngine;

/// <summary>
/// 球形检测工具类，提供静态方法进行球形碰撞检测和可视化
/// </summary>
public static class SphereDetectionUtility
{
    private static readonly Collider[] colliderBuffer = new Collider[10]; // 预分配缓冲区
    
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

            Debug.Log($"=== 开始检测 {direction} 方向 ===");
            Debug.Log($"检测位置: {checkPosition}, 检测半径: {sphereRadius}");

            // 使用OverlapSphereNonAlloc避免内存分配
            int colliderCount = Physics.OverlapSphereNonAlloc(checkPosition, sphereRadius, colliderBuffer);

            Debug.Log($"在 {direction} 方向检测到 {colliderCount} 个碰撞体");

            // 详细记录每个检测到的碰撞体信息
            LogColliderDetails(colliderCount, direction);

            // 根据布尔值决定是否生成可视化球体
            if (drawDebugSphere)
            {
                CreateDebugSphere(checkPosition, direction);
            }
            else
            {
                Debug.Log($"drawDebugSphere为false，跳过 {direction} 方向的可视化球体生成");
            }

            // 检查是否有实现IGetObjectClass接口的对象
            if (!CheckForValidObjects(colliderCount, direction))
            {
                Debug.Log($"=== 在 {direction} 方向没有检测到实现IGetObjectClass接口的对象，找到空位置 ===");
                Debug.Log("哈哈哈");
                emptyPosition = checkPosition;
                return true;
            }
            else
            {
                Debug.Log($"=== {direction} 方向检测通过，继续下一个方向 ===");
            }
        }

        Debug.Log("四个方向都有物体但没有找到空位置");
        return false;
    }
    
    /// <summary>
    /// 记录碰撞体详细信息
    /// </summary>
    private static void LogColliderDetails(int colliderCount, Vector3 direction)
    {
        for (int i = 0; i < colliderCount; i++)
        {
            Collider col = colliderBuffer[i];
            if (col != null)
            {
                string objectName = col.gameObject.name;
                string colliderType = col.GetType().Name;
                Vector3 colliderPos = col.transform.position;
                
                Debug.Log($"  碰撞体 {i + 1}: 名称='{objectName}', 类型={colliderType}, 位置={colliderPos}");
                
                // 检查是否有Rigidbody
                if (col.attachedRigidbody != null)
                {
                    Debug.Log($"    - 附加刚体: {col.attachedRigidbody.name}");
                    
                    // 检查所有组件
                    Component[] components = col.attachedRigidbody.GetComponents<Component>();
                    Debug.Log($"    - 刚体上的组件数量: {components.Length}");
                    foreach (Component comp in components)
                    {
                        if (comp != null)
                        {
                            Debug.Log($"      * {comp.GetType().Name}");
                        }
                    }
                }
                else
                {
                    Debug.Log($"    - 无附加刚体");
                    
                    // 如果没有刚体，检查GameObject上的组件
                    Component[] components = col.GetComponents<Component>();
                    Debug.Log($"    - GameObject上的组件数量: {components.Length}");
                    foreach (Component comp in components)
                    {
                        if (comp != null)
                        {
                            Debug.Log($"      * {comp.GetType().Name}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"  碰撞体 {i + 1}: null");
            }
        }
    }
    
    /// <summary>
    /// 创建调试球体
    /// </summary>
    private static void CreateDebugSphere(Vector3 position, Vector3 direction)
    {
        GameObject visualSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualSphere.name = $"DebugSphere_{direction}";
        visualSphere.transform.position = position;
        visualSphere.transform.localScale = Vector3.one * 1f;
        
        Debug.Log($"生成可视化球体: {visualSphere.name} 在位置 {position}");
        
        // 设置半透明红色材质
        Renderer renderer = visualSphere.GetComponent<Renderer>();
        Material debugMaterial = new Material(Shader.Find("Standard"));
        debugMaterial.color = new Color(1f, 0f, 0f, 0.5f); // 半透明红色
        debugMaterial.SetFloat("_Mode", 3); // 设置为透明模式
        debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        debugMaterial.SetInt("_ZWrite", 0);
        debugMaterial.DisableKeyword("_ALPHATEST_ON");
        debugMaterial.EnableKeyword("_ALPHABLEND_ON");
        debugMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        debugMaterial.renderQueue = 3000;
        renderer.material = debugMaterial;
    }
    
    /// <summary>
    /// 检查是否有实现IGetObjectClass接口的对象
    /// </summary>
    private static bool CheckForValidObjects(int colliderCount, Vector3 direction)
    {
        bool foundValidObject = false;
        Debug.Log($"开始检查 {direction} 方向的碰撞体是否实现IGetObjectClass接口...");
        
        for (int i = 0; i < colliderCount; i++)
        {
            Collider col = colliderBuffer[i];
            if (col != null && col.attachedRigidbody != null)
            {
                if (col.attachedRigidbody.TryGetComponent<IGetObjectClass>(out var objectClass))
                {
                    Debug.Log($"  找到实现IGetObjectClass接口的对象: {col.attachedRigidbody.name}");
                    foundValidObject = true;
                    break;
                }
                else
                {
                    Debug.Log($"  对象 {col.attachedRigidbody.name} 未实现IGetObjectClass接口");
                }
            }
            else if (col != null)
            {
                Debug.Log($"  对象 {col.name} 没有附加刚体，跳过接口检查");
            }
        }
        
        return foundValidObject;
    }
}
