using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Plant : MonoBehaviour
{
    [Header("长方形UI面板")]
    public GameObject rectangularPanel;

    [Header("四个正方形图像")]
    public Button Image1Button;
    public Button Image2Button;
    public Button Image3Button;
    public Button Image4Button;

    [Header("图像精灵")]
    public Sprite[] imageSprites = new Sprite[4];
    [Header("UI控制")]
    public Button showUIButton;
    public Button hideUIButton;

    private Image[] allSquareImages;

    void Start()
    {
        InitializeUI();
        SetupSquareClickEvents();
    }

    void InitializeUI()
    {
        // 默认隐藏UI面板
        if (rectangularPanel != null)
            rectangularPanel.SetActive(false);

        // 绑定按钮事件
        if (showUIButton != null)
            showUIButton.onClick.AddListener(ShowUI);

        if (hideUIButton != null)
            hideUIButton.onClick.AddListener(HideUI);
    }

    public void ShowUI()
    {
        if (rectangularPanel != null)
        {
            rectangularPanel.SetActive(true);
            Debug.Log("显示植物UI");
        }
    }

    public void HideUI()
    {
        if (rectangularPanel != null)
        {
            rectangularPanel.SetActive(false);
            Debug.Log("隐藏植物UI");
        }
    }

    void Update()
    {
        // 按空格键切换UI显示状态
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rectangularPanel != null)
            {
                if (rectangularPanel.activeInHierarchy)
                    HideUI();
                else
                    ShowUI();
            }
        }

        // 按ESC键隐藏UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (rectangularPanel != null && rectangularPanel.activeInHierarchy)
            {
                HideUI();
            }
        }
    }

    public void SetupSquareClickEvents()
    {
        // 为第一个方块添加点击事件
        if (Image1Button != null)
        {
            Image1Button.onClick.AddListener(OnFirstSquareClicked);
        }
        if (Image2Button != null)
        {
            Image2Button.onClick.AddListener(OnSecondSquareClicked);
        }

        if (Image3Button != null)
        {
            Image3Button.onClick.AddListener(OnThirdSquareClicked);
        }
        if (Image4Button != null)
        {
            Image4Button.onClick.AddListener(OnFourthSquareClicked);
        }
    }

    public void OnFirstSquareClicked()
    {
        // 调用ActorManager参数
        if (ActorManager.Instance != null)
        {
            Debug.Log("点击第一个方块，调用ActorManager");
            ActorManager.OnFirstSquareClicked();
            // 在这里添加具体的ActorManager调用逻辑
        }
        else
        {
            Debug.LogWarning("ActorManager实例未找到！");
        }
    }

    void OnSecondSquareClicked()
    {
        if (ActorManager.Instance != null)
        {
            Debug.Log("点击第二个方块，调用ActorManager");
            ActorManager.OnSecondSquareClicked();
        }
    }

    void OnThirdSquareClicked()
    {
        if (ActorManager.Instance != null)
        {
            Debug.Log("点击第二个方块，调用ActorManager");
            ActorManager.OnThirdSquareClicked();
        }
    }

    void OnFourthSquareClicked()
    {
        if (ActorManager.Instance != null)
        {
            Debug.Log("点击第四个方块，调用ActorManager");
            ActorManager.OnFourthSquareClicked();
        }
    }

    public void OnRightMouseDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ResetValue();
        }
    }

    public void ResetValue()
    {
        if (ActorManager.Instance != null)
        {
            ActorManager.ResetValue();
        }
    }
}
