using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_StartingMenu : MonoBehaviour
{
    [Header("菜单按钮")]
    public Button startGameButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("菜单面板")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("设置选项")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        // 初始化菜单
        InitializeMenu();

        // 绑定按钮事件
        BindButtonEvents();

        // 设置按钮文字
        SetButtonTexts();

        // 加载设置
        LoadSettings();
    }

    void SetButtonTexts()
    {
        // 设置开始游戏按钮文字
        if (startGameButton != null)
        {
            SetButtonText(startGameButton, "开始游戏");
        }

        // 设置设置按钮文字
        if (settingsButton != null)
        {
            SetButtonText(settingsButton, "设置");
        }

        // 设置退出按钮文字
        if (exitButton != null)
        {
            SetButtonText(exitButton, "退出游戏");
        }
    }

    void SetButtonText(Button button, string text)
    {
        // 尝试获取TextMeshProUGUI组件（Text Mesh Pro）
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = text;
            return;
        }

        // 如果没有找到TextMeshProUGUI，尝试获取普通Text组件
        Text normalText = button.GetComponentInChildren<Text>();
        if (normalText != null)
        {
            normalText.text = text;
        }
    }

    void InitializeMenu()
    {
        // 显示主菜单面板，隐藏设置面板
        ShowMainMenu();

        // 设置时间缩放为正常
        Time.timeScale = 1f;
    }

    void BindButtonEvents()
    {
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        // 这里可以添加场景切换逻辑
        // SceneManager.LoadScene("GameScene");

        // 或者隐藏菜单开始游戏
        gameObject.SetActive(false);

        Debug.Log("开始游戏");
    }

    public void ShowSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

        Debug.Log("退出游戏");
    }

    public void OnVolumeChanged()
    {
        if (volumeSlider != null)
        {
            AudioListener.volume = volumeSlider.value;
            PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        }
    }

    public void OnFullscreenToggle()
    {
        if (fullscreenToggle != null)
        {
            Screen.fullScreen = fullscreenToggle.isOn;
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        }
    }

    public void OnQualityChanged()
    {
        if (qualityDropdown != null)
        {
            QualitySettings.SetQualityLevel(qualityDropdown.value);
            PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        }
    }

    void LoadSettings()
    {
        // 加载音量设置
        if (volumeSlider != null)
        {
            float volume = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.value = volume;
            AudioListener.volume = volume;
        }

        // 加载全屏设置
        if (fullscreenToggle != null)
        {
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.isOn = fullscreen;
            Screen.fullScreen = fullscreen;
        }

        // 加载画质设置
        if (qualityDropdown != null)
        {
            int quality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
            qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }
    }

    void Update()
    {
        // 按ESC键返回主菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeInHierarchy)
            {
                ShowMainMenu();
            }
        }
    }
}
