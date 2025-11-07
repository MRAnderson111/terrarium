using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ButtonGroupManager : MonoBehaviour
{
    // 按钮的普通状态颜色
    public Color normalColor = Color.white;
    // 按钮的选中状态颜色
    public Color selectedColor = new Color(0.8f, 0.8f, 0.8f); // 举个例子，浅灰色

    private Button _currentSelectedButton;
    private List<Button> _buttons;

    public Transform buttonContainer1; // 按钮的父物体
    public Transform buttonContainer2; // 按钮的父物体
    public Transform buttonContainer3; // 按钮的父物体

    void Start()
    {
        // 自动查找指定容器中的第一级子物体中的所有按钮
        FindButtonsInContainers();
        
        // 为每个按钮动态添加点击事件监听
        foreach (Button btn in _buttons)
        {
            // 使用了一个临时变量来避免闭包问题
            Button currentBtn = btn;
            btn.onClick.AddListener(() => OnButtonClicked(currentBtn));
        }

        // 默认不选中任何按钮，所有按钮保持普通状态
        // 如果需要默认选中第一个按钮，可以取消下面的注释
        // if (_buttons.Count > 0)
        // {
        //     OnButtonClicked(_buttons[0]);
        // }
    }

    // 自动查找指定容器中的第一级子物体中的所有按钮
    void FindButtonsInContainers()
    {
        _buttons = new List<Button>();
        
        // 创建一个包含所有容器的数组
        Transform[] containers = { buttonContainer1, buttonContainer2, buttonContainer3 };
        
        // 遍历每个容器
        foreach (Transform container in containers)
        {
            // 检查容器是否为空
            if (container == null)
            {
                continue;
            }
            
            // 遍历容器的第一级子物体
            foreach (Transform child in container)
            {
                // 尝试获取子物体上的Button组件
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    _buttons.Add(button);
                }
            }
        }
        
        Debug.Log($"在指定容器中找到了 {_buttons.Count} 个按钮");
    }

    void OnButtonClicked(Button clickedButton)
    {
        // 如果点击的已经是选中的按钮，可以什么都不做
        if (_currentSelectedButton == clickedButton)
        {
            return;
        }
        
        // 1. 恢复所有按钮到普通状态
        foreach (Button btn in _buttons)
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
