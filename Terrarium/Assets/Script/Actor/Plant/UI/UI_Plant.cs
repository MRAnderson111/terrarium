using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Plant : MonoBehaviour
{
    [Header("长方形UI面板")]
    public GameObject rectangularPanel;

    [Header("四个正方形图像")]
    public Image squareImage1;
    public Image squareImage2;
    public Image squareImage3;
    public Image squareImage4;

    [Header("图像精灵")]
    public Sprite[] imageSprites = new Sprite[4];

    [Header("UI控制")]
    public Button showUIButton;
    public Button hideUIButton;

    private Image[] allSquareImages;

    void Start()
    {
        InitializeUI();
        SetupImageArray();
        LoadDefaultImages();
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

    void SetupImageArray()
    {
        // 将四个图像组件放入数组中便于管理
        allSquareImages = new Image[4];
        allSquareImages[0] = squareImage1;
        allSquareImages[1] = squareImage2;
        allSquareImages[2] = squareImage3;
        allSquareImages[3] = squareImage4;
    }

    void LoadDefaultImages()
    {
        // 为四个正方形加载默认图像
        for (int i = 0; i < allSquareImages.Length; i++)
        {
            if (allSquareImages[i] != null && i < imageSprites.Length)
            {
                if (imageSprites[i] != null)
                {
                    allSquareImages[i].sprite = imageSprites[i];
                }
            }
        }
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

    public void SetSquareImage(int index, Sprite newSprite)
    {
        if (index >= 0 && index < allSquareImages.Length)
        {
            if (allSquareImages[index] != null)
            {
                allSquareImages[index].sprite = newSprite;
                Debug.Log($"设置第{index + 1}个正方形图像");
            }
        }
    }

    public void SetAllImages(Sprite[] newSprites)
    {
        if (newSprites != null)
        {
            for (int i = 0; i < Mathf.Min(newSprites.Length, allSquareImages.Length); i++)
            {
                if (allSquareImages[i] != null && newSprites[i] != null)
                {
                    allSquareImages[i].sprite = newSprites[i];
                }
            }
            Debug.Log("更新所有正方形图像");
        }
    }

    public void ClearAllImages()
    {
        for (int i = 0; i < allSquareImages.Length; i++)
        {
            if (allSquareImages[i] != null)
            {
                allSquareImages[i].sprite = null;
            }
        }
        Debug.Log("清空所有正方形图像");
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

    void SetupSquareClickEvents()
    {
        // 为第一个方块添加点击事件
        if (squareImage1 != null)
        {
            if (squareImage1.TryGetComponent<Button>(out var button1))
            {
                button1.onClick.AddListener(OnFirstSquareClicked);
            }
            else
            {
                Debug.LogWarning("第一个方块没有找到Button组件！");
            }
        }
    }

    void OnFirstSquareClicked()
    {
        // 调用ActorManager参数
        if (ActorManager.Instance != null)
        {
            Debug.Log("点击第一个方块，调用ActorManager");
            ActorManager.Instance.OnFirstSquareClicked();
            // 在这里添加具体的ActorManager调用逻辑
        }
        else
        {
            Debug.LogWarning("ActorManager实例未找到！");
        }
    }
}
