using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUI : MonoBehaviour
{
    public Text timeText;
    public Text stageText;
    public Text foodQuantityText;
    





    private StageManager stageManager; // 缓存单例引用
    private float updateInterval = 0.1f; // 更新间隔（秒）
    private float lastUpdateTime;

    void Start()
    {
        stageManager = StageManager.Instance; // 缓存单例
        lastUpdateTime = Time.time;
        PlaceOnScreen();
    }

    void PlaceOnScreen()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
            return;

        Rect area = canvasRect.rect;
        transform.localPosition = new Vector3(area.xMax - 220f, area.yMax - 280f, 0f);
    }

    void Update()
    {
        if (stageManager == null || Time.time - lastUpdateTime < updateInterval)
            return;

        if (timeText != null)
            timeText.text = $"剩余时间: {stageManager.RemainingTime:F2}";
        if (stageText != null)
            stageText.text = $"阶段: {stageManager.stage}";
        if (foodQuantityText != null)
            foodQuantityText.text = $"食物数量: {stageManager.foodQuantity}";
        lastUpdateTime = Time.time;
    }
}
