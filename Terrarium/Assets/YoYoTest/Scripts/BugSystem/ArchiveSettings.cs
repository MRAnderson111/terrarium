using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveSettings : MonoBehaviour
{
    /// <summary>
    /// 重置所有游戏数据
    /// </summary>
    /// 
    [ContextMenu("重置所有游戏数据")]
    public void ResetAllData()
    {
        // 删除所有PlayerPrefs数据
        PlayerPrefs.DeleteAll();

        // 保存更改
        PlayerPrefs.Save();

        Debug.Log("所有存档数据已重置！");
    }


    [ContextMenu("log所有游戏数据")]
    public void LogAllData()
    {
        Debug.Log("所有存档数据：" + PlayerPrefs.GetString("AllData"));
    }
}
