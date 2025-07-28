using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UCreate : MonoBehaviour
{
    [Header("UI管理器")]
    public Canvas mainCanvas;

    [Header("UI组件")]
    private GameObject startUI;
    private GameObject pauseUI;
    private GameObject gameOverUI;

    // 游戏状态枚举
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver
    }

    private GameState currentState = GameState.Start;

    void Start()
    {
        // 如果没有指定Canvas，自动创建一个
        if (mainCanvas == null)
        {
            CreateMainCanvas();
        }

        // 创建三组UI
        CreateStartUI();
        CreatePauseUI();
        CreateGameOverUI();

        // 初始显示开始UI
        ShowStartUI();
    }

    #region Canvas创建
    private void CreateMainCanvas()
    {
        GameObject canvasObj = new GameObject("MainCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;

        // 添加CanvasScaler组件
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // 添加GraphicRaycaster组件
        canvasObj.AddComponent<GraphicRaycaster>();

        DontDestroyOnLoad(canvasObj);
    }
    #endregion

    #region 开始UI
    private void CreateStartUI()
    {
        startUI = new GameObject("StartUI");
        startUI.transform.SetParent(mainCanvas.transform, false);

        // 创建背景
        CreateUIBackground(startUI, new Color(0.1f, 0.1f, 0.2f, 0.8f));

        // 创建标题
        CreateUIText(startUI, "游戏标题", new Vector2(0, 200), 48, Color.white, "欢迎来到Terrarium");

        // 创建开始按钮
        CreateUIButton(startUI, "开始游戏", new Vector2(0, 50), new Vector2(200, 60));

        // 创建设置按钮
        CreateUIButton(startUI, "游戏设置", new Vector2(0, -20), new Vector2(200, 60));

        // 创建退出按钮
        CreateUIButton(startUI, "退出游戏", new Vector2(0, -90), new Vector2(200, 60));
    }

    public void ShowStartUI()
    {
        currentState = GameState.Start;
        startUI.SetActive(true);
        if (pauseUI != null) pauseUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }
    #endregion

    #region 暂停UI
    private void CreatePauseUI()
    {
        pauseUI = new GameObject("PauseUI");
        pauseUI.transform.SetParent(mainCanvas.transform, false);

        // 创建半透明背景
        CreateUIBackground(pauseUI, new Color(0, 0, 0, 0.6f));

        // 创建暂停标题
        CreateUIText(pauseUI, "暂停标题", new Vector2(0, 150), 36, Color.white, "游戏暂停");

        // 创建继续按钮
        CreateUIButton(pauseUI, "继续游戏", new Vector2(0, 50), new Vector2(200, 60));

        // 创建重新开始按钮
        CreateUIButton(pauseUI, "重新开始", new Vector2(0, -20), new Vector2(200, 60));

        // 创建返回主菜单按钮
        CreateUIButton(pauseUI, "返回主菜单", new Vector2(0, -90), new Vector2(200, 60));

        pauseUI.SetActive(false);
    }

    public void ShowPauseUI()
    {
        currentState = GameState.Paused;
        if (startUI != null) startUI.SetActive(false);
        pauseUI.SetActive(true);
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }
    #endregion

    #region 结算UI
    private void CreateGameOverUI()
    {
        gameOverUI = new GameObject("GameOverUI");
        gameOverUI.transform.SetParent(mainCanvas.transform, false);

        // 创建背景
        CreateUIBackground(gameOverUI, new Color(0.2f, 0.1f, 0.1f, 0.9f));

        // 创建结算标题
        CreateUIText(gameOverUI, "结算标题", new Vector2(0, 200), 42, Color.white, "游戏结束");

        // 创建分数显示
        CreateUIText(gameOverUI, "分数显示", new Vector2(0, 120), 24, Color.yellow, "最终分数: 0");

        // 创建最高分显示
        CreateUIText(gameOverUI, "最高分显示", new Vector2(0, 80), 20, Color.cyan, "最高分: 0");

        // 创建重新开始按钮
        CreateUIButton(gameOverUI, "再来一局", new Vector2(-100, 0), new Vector2(180, 60));

        // 创建返回主菜单按钮
        CreateUIButton(gameOverUI, "返回主菜单", new Vector2(100, 0), new Vector2(180, 60));

        // 创建分享按钮
        CreateUIButton(gameOverUI, "分享成绩", new Vector2(0, -80), new Vector2(180, 60));

        gameOverUI.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        currentState = GameState.GameOver;
        if (startUI != null) startUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    // 更新结算UI的分数显示
    public void UpdateGameOverScore(int finalScore, int highScore)
    {
        Transform scoreText = gameOverUI.transform.Find("分数显示");
        if (scoreText != null)
        {
            scoreText.GetComponent<Text>().text = $"最终分数: {finalScore}";
        }

        Transform highScoreText = gameOverUI.transform.Find("最高分显示");
        if (highScoreText != null)
        {
            highScoreText.GetComponent<Text>().text = $"最高分: {highScore}";
        }
    }
    #endregion

    #region UI创建辅助方法
    private void CreateUIBackground(GameObject parent, Color color)
    {
        GameObject background = new GameObject("Background");
        background.transform.SetParent(parent.transform, false);

        Image bgImage = background.AddComponent<Image>();
        bgImage.color = color;

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
    }

    private GameObject CreateUIText(GameObject parent, string name, Vector2 position, int fontSize, Color color, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(400, fontSize + 10);

        return textObj;
    }

    private GameObject CreateUIButton(GameObject parent, string name, Vector2 position, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);

        // 添加按钮背景
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.8f, 0.8f);

        // 添加按钮组件
        Button button = buttonObj.AddComponent<Button>();

        // 设置按钮颜色变化
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.8f, 0.8f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.7f, 1f);
        button.colors = colors;

        // 设置位置和大小
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        // 创建按钮文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = name;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        return buttonObj;
    }
    #endregion
}
