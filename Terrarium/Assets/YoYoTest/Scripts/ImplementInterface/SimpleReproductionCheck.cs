using System.Collections;
using UnityEngine;

public class SimpleReproductionCheck : MonoBehaviour, IReproductionCheck
{
    private readonly Collider[] colliderBuffer = new Collider[10]; // 预分配缓冲区
    public bool drawDebugSphere = false;
    
    public void ReproductionCheck()
    {
        float checkDistance = 1f; // 检测距离
        Vector3 currentPos = transform.position;
        
        // 定义四个方向：前后左右
        Vector3[] directions = {
            Vector3.forward,  // 前
            Vector3.back,     // 后
            Vector3.left,     // 左
            Vector3.right     // 右
        };
        
        foreach (Vector3 direction in directions)
        {
            Vector3 checkPosition = currentPos + direction * checkDistance;

            Debug.Log($"=== 开始检测 {direction} 方向 ===");
            Debug.Log($"检测位置: {checkPosition}, 检测半径: 0.5f");

            // 使用OverlapSphereNonAlloc避免内存分配
            int colliderCount = Physics.OverlapSphereNonAlloc(checkPosition, 0.5f, colliderBuffer);

            Debug.Log($"在 {direction} 方向检测到 {colliderCount} 个碰撞体");

            // 详细记录每个检测到的碰撞体信息
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

            // 根据布尔值决定是否生成可视化球体
            if (drawDebugSphere)
            {
                // 生成可视化球体
                GameObject visualSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                visualSphere.name = $"DebugSphere_{direction}";
                visualSphere.transform.position = checkPosition;
                visualSphere.transform.localScale = Vector3.one * 1f;

                Debug.Log($"生成可视化球体: {visualSphere.name} 在位置 {checkPosition}");

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
            else
            {
                Debug.Log($"drawDebugSphere为false，跳过 {direction} 方向的可视化球体生成");
            }

            // 检查是否有实现IGetObjectClass接口的对象
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

            // 如果没有找到实现IGetObjectClass接口的对象，停止检测
            if (!foundValidObject)
            {
                Debug.Log($"=== 在 {direction} 方向没有检测到实现IGetObjectClass接口的对象，停止检测 ===");
                Debug.Log("哈哈哈");
                return;
            }
            else
            {
                Debug.Log($"=== {direction} 方向检测通过，继续下一个方向 ===");
            }

        }
        
        Debug.Log("四个方向都有物体但没有找到包含fix和joint的脚本");
    }
}
