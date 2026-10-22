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
    public static int GetGroundFertility(Vector3 position, float rayDistance = 20f, Transform ignoreRoot = null)
    {
        // 从植物上方往下打，避免射线起点落在自身碰撞体内部而漏掉地面。
        Vector3 rayOrigin = position + Vector3.up * 2f;
        Vector3 rayDirection = Vector3.down;
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, rayDistance + 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (ignoreRoot != null && hit.collider.transform.IsChildOf(ignoreRoot))
                    continue;

                if (hit.collider.TryGetComponent<IGetGroundClass>(out IGetGroundClass groundClass))
                    return groundClass.Fertility;
            }
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
