using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinLevel : MonoBehaviour
{
    public string levelName;

    // 当鼠标点击这个对象时调用
    void OnMouseDown()
    {
        // 检查levelName是否已设置
        if (!string.IsNullOrEmpty(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("Level name is not set in JoinLevel script!");
        }
    }
}
