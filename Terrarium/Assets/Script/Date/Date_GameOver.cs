using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Date_GameOver : MonoBehaviour
{
    [Header("游戏结束设置")]
    [SerializeField] private float checkInterval = 1f; // 检查间隔（秒）
    
    private bool gameEnded = false;
    private bool hasFirstPlant = false; // 是否已经有第一株植物

    void Start()
    {
        // 开始检查是否有第一株植物
        StartCoroutine(WaitForFirstPlant());
    }
    
    IEnumerator WaitForFirstPlant()
    {
        // 等待第一株植物出现
        while (!hasFirstPlant)
        {
            yield return new WaitForSeconds(checkInterval);
            
            int currentPlantCount = GetTotalPlantCount();
            if (currentPlantCount > 0)
            {
                hasFirstPlant = true;
                Debug.Log("检测到第一株植物，开始监控植物数量");
                
                // 开始定期检查植物数量
                StartCoroutine(CheckPlantCount());
            }
        }
    }
    
    IEnumerator CheckPlantCount()
    {
        while (!gameEnded)
        {
            yield return new WaitForSeconds(checkInterval);
            
            // 获取当前植物数量（不包括PlantFirst）
            int currentPlantCount = GetTotalPlantCount();
            
            // 如果植物数量为0（即只剩PlantFirst），游戏结束
            if (currentPlantCount <= 0)
            {
                TriggerGameOver();
            }
        }
    }
    
    int GetTotalPlantCount()
    {
        // 通过反射调用PlantItem的静态方法
        try
        {
            var plantItemType = System.Type.GetType("PlantItem");
            if (plantItemType != null)
            {
                var method = plantItemType.GetMethod("GetTotalPlantCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    return (int)method.Invoke(null, null);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取植物数量失败: {e.Message}");
        }
        
        return 0;
    }
    
    void TriggerGameOver()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        Debug.Log("游戏结束：环境中没有植物了！");
        
        // 3秒后重新开始游戏
        StartCoroutine(RestartGameAfterDelay());
    }
    
    IEnumerator RestartGameAfterDelay()
    {
        // 等待3秒后重新开始游戏
        yield return new WaitForSecondsRealtime(3f);
        
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
