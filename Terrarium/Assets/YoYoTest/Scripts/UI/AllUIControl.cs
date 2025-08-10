using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllUIControl : MonoBehaviour
{
    public GameObject gameEndUI;
    
    private void Start()
    {
        // 订阅游戏结束事件
        Events.OnGameEnd.AddListener(OnGameEndHandler);
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        Events.OnGameEnd.RemoveListener(OnGameEndHandler);
    }
    
    // 游戏结束事件处理方法
    private void OnGameEndHandler()
    {
        gameEndUI.SetActive(true);
        Debug.Log("哈哈哈");
    }
}
