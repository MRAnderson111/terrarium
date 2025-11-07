using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonGroupManager : MonoBehaviour
{
    // 在Inspector面板中把所有需要管理的按钮拖到这里
    public List<Button> buttons;

    // 按钮的普通状态颜色
    public Color normalColor = Color.white;
    // 按钮的选中状态颜色
    public Color selectedColor = new Color(0.8f, 0.8f, 0.8f); // 举个例子，浅灰色

    private Button _currentSelectedButton;

    void Start()
    {
        // 为每个按钮动态添加点击事件监听
        foreach (Button btn in buttons)
        {
            // 使用了一个临时变量来避免闭包问题
            Button currentBtn = btn;
            btn.onClick.AddListener(() => OnButtonClicked(currentBtn));
        }

        // 如果有按钮，默认选中第一个
        if (buttons.Count > 0)
        {
            OnButtonClicked(buttons[0]);
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        // 如果点击的已经是选中的按钮，可以什么都不做
        if (_currentSelectedButton == clickedButton)
        {
            return;
        }
        
        // 1. 恢复所有按钮到普通状态
        foreach (Button btn in buttons)
        {
            // 我们通过改变颜色来区分状态
            // 注意：这里需要按钮的Color Target模式是ColorTint
            btn.GetComponent<Image>().color = normalColor;
        }

        // 2. 设置被点击的按钮为选中状态
        if (clickedButton != null)
        {
            clickedButton.GetComponent<Image>().color = selectedColor;
            _currentSelectedButton = clickedButton;
        }

        // 在这里可以执行与按钮选择相关的其他逻辑
        Debug.Log(clickedButton.name + " was selected!");
    }
}
