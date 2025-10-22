using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveSettings : MonoBehaviour
{
    private const string COINS_KEY = "PlayerCoins";
    public  int DEFAULT_COINS = 500;
    /// <summary>
    /// 重置所有游戏数据
    /// </summary>
    ///
    [ContextMenu("重置所有游戏数据")]
    public void ResetAllData()
    {
        // 删除所有PlayerPrefs数据
        PlayerPrefs.DeleteAll();
        
        //重新设置默认金币
        PlayerPrefs.SetInt(COINS_KEY, DEFAULT_COINS);

        // 保存更改
        PlayerPrefs.Save();

        Debug.Log("所有存档数据已重置！");
    }


 
}
