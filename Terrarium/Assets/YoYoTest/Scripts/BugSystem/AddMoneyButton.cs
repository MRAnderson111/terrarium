using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AddMoneyButton : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    
    // Button组件引用
    private Button button;
    
    // 存档键名常量
    private const string COINS_KEY = "PlayerCoins";
    
    // 默认金币数量
    private const int DEFAULT_COINS = 100;
    
    // 每次添加的金币数量
    private const int ADD_COINS_AMOUNT = 200;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取Button组件
        button = GetComponent<Button>();
        
        // 如果找到了Button组件，绑定AddMoney方法到onClick事件
        if (button != null)
        {
            button.onClick.AddListener(AddMoney);
        }
        else
        {
            Debug.LogWarning("在GameObject上未找到Button组件！");
        }
        
        UpdateMoneyText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoneyText();
    }
    
    /// <summary>
    /// 更新金币文本显示
    /// </summary>
    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            // 获取当前金币数量
            int currentCoins = PlayerPrefs.GetInt(COINS_KEY, DEFAULT_COINS);
            
            // 更新文本显示
            moneyText.text = "当前金币: " + currentCoins;
        }
    }
    
    /// <summary>
    /// 添加金币的方法
    /// </summary>
    public void AddMoney()
    {
        // 获取当前金币数量
        int currentCoins = PlayerPrefs.GetInt(COINS_KEY, DEFAULT_COINS);
        
        // 添加200金币
        currentCoins += ADD_COINS_AMOUNT;
        
        // 保存新的金币数量
        PlayerPrefs.SetInt(COINS_KEY, currentCoins);
        PlayerPrefs.Save();
        
        // 立即更新显示
        UpdateMoneyText();
        
        Debug.Log("成功添加 " + ADD_COINS_AMOUNT + " 金币，当前金币总数: " + currentCoins);
    }
    
    // OnDestroy时移除事件监听，避免内存泄漏
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(AddMoney);
        }
    }
}
