using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Button restartButton;
    public Button toMainMenuButton;
    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        toMainMenuButton.onClick.AddListener(OnToMainMenuButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRestartButtonClick()
    {
        Debug.Log("重新开始");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnToMainMenuButtonClick()
    {
        Debug.Log("返回主菜单");
        SceneManager.LoadScene("Start");
    }
}
