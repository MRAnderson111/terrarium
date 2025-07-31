using UnityEngine;

/// <summary>
/// 地面检测工具类
/// 提供静态方法用于检测地面并获取地面属性
/// </summary>
public static class GroundDetectionUtils
{
    /// <summary>
    /// 检测地面并获取肥沃度
    /// </summary>
    /// <param name="position">检测起始位置</param>
    /// <param name="rayDistance">射线检测距离，默认10f</param>
    /// <returns>返回地面的肥沃度，如果未检测到地面或地面没有实现接口则返回-1</returns>
    public static int GetGroundFertility(Vector3 position, float rayDistance = 10f)
    {
        // 向下发射射线检测地面
        Vector3 rayOrigin = position; // 从指定位置开始
        Vector3 rayDirection = Vector3.down; // 向下方向

        // 发射射线检测所有物体（使用所有层）
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, rayDistance, Physics.AllLayers);

        if (hits.Length > 0)
        {
            Debug.Log($"射线检测到 {hits.Length} 个物体");

            // 按距离排序，从近到远遍历
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            // 遍历所有击中的物体，找到第一个实现IGetGroundClass接口的
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Debug.Log($"检查物体 {i + 1}: {hit.collider.name}, 标签: {hit.collider.tag}, 距离: {hit.distance:F2}");

                // 尝试获取IGetGroundClass接口
                if (hit.collider.TryGetComponent<IGetGroundClass>(out IGetGroundClass groundClass))
                {
                    // 找到了实现接口的物体，获取Fertility属性
                    int fertility = groundClass.Fertility;
                    Debug.Log($"找到实现IGetGroundClass接口的物体: {hit.collider.name}，肥沃度: {fertility}");
                    return fertility;
                }
                else
                {
                    Debug.Log($"物体 {hit.collider.name} 没有实现IGetGroundClass接口，继续检查下一个");
                }
            }

            Debug.LogWarning("射线击中了物体，但没有找到实现IGetGroundClass接口的物体");
        }
        else
        {
            Debug.LogWarning("射线未击中任何物体");
        }


        return -3; // 未检测到有效地面
    }



    /// <summary>
    /// 检测地面并直接设置对象的生长速度（简化版本，无返回值）
    /// </summary>
    /// <param name="position">检测起始位置</param>
    /// <param name="growthSpeedSetter">设置生长速度的回调函数</param>
    /// <param name="rayDistance">射线检测距离，默认10f</param>
    public static void SetGrowthSpeedFromGroundSimple(Vector3 position, System.Action<float> growthSpeedSetter, float rayDistance = 10f)
    {
        int fertility = GetGroundFertility(position, rayDistance);
        if (fertility >= 0)
        {
            growthSpeedSetter?.Invoke(fertility);
            Debug.Log($"生长速度设置为: {fertility}");
        }
        else
        {


            Debug.LogWarning("未检测到有效地面，保持默认生长速度，当前fertility为：" + fertility);
        }
    }
}
