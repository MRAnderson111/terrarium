using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneEndRule : MonoBehaviour
{
    private bool hasLoggedPlantExtinction = false;

    // Start is called before the first frame update
    void Start()
    {
        // 监听统计更新事件
        Events.OnStatisticsUpdate.AddListener(OnStatisticsUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // 移除事件监听
        Events.OnStatisticsUpdate.RemoveListener(OnStatisticsUpdate);
    }

    /// <summary>
    /// 处理统计更新事件
    /// </summary>
    /// <param name="bigClass">大类名称</param>
    /// <param name="count">数量</param>
    private void OnStatisticsUpdate(string bigClass, int count)
    {
        // 检查是否是植物类
        if (bigClass == "Plant")
        {
            // 如果植物数量为0且还没有记录过
            if (count == 0 && !hasLoggedPlantExtinction)
            {
                Debug.Log("植物死光了 所以游戏结束");
                Events.OnGameEnd.Invoke();
                hasLoggedPlantExtinction = true;
            }
            // 如果植物数量大于0，重置记录标志
            else if (count > 0)
            {
                hasLoggedPlantExtinction = false;
            }
        }
    }
}
