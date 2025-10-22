using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public string itemName;
    public int itemPrice;
    public TextMeshProUGUI priceText;
    
    // Button组件引用
    private Button button;
    
    // 存档键名常量
    private const string COINS_KEY = "PlayerCoins";
    private const string ITEMS_PREFIX = "Item_";
    
    // 默认金币数量
    private const int DEFAULT_COINS = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取Button组件
        button = GetComponent<Button>();
        
        // 如果找到了Button组件，绑定OnClick方法到onClick事件
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogWarning("在GameObject上未找到Button组件！");
        }
        
        UpdatePriceText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePriceText();
    }
    
    /// <summary>
    /// 更新价格文本显示
    /// </summary>
    private void UpdatePriceText()
    {
        if (priceText != null)
        {
            // 获取当前物品数量
            string itemKey = ITEMS_PREFIX + itemName;
            int currentItemCount = PlayerPrefs.GetInt(itemKey, 0);
            
            // 格式化显示文本：物品名称 价格 数量
            priceText.text = $"{itemName}\nPrice: {itemPrice} Coin\nNumber: {currentItemCount}";
        }
    }

    public void OnClick()
    {
        Debug.Log("购买物品：" + itemName);
        
        // 获取当前金币数量
        int currentCoins = PlayerPrefs.GetInt(COINS_KEY, DEFAULT_COINS);
        
        // 检查金币是否足够
        if (currentCoins >= itemPrice)
        {
            // 扣除金币
            currentCoins -= itemPrice;
            PlayerPrefs.SetInt(COINS_KEY, currentCoins);
            
            // 获取当前物品数量
            string itemKey = ITEMS_PREFIX + itemName;
            int currentItemCount = PlayerPrefs.GetInt(itemKey, 0);
            
            // 增加物品数量
            currentItemCount += 1;
            PlayerPrefs.SetInt(itemKey, currentItemCount);
            
            // 保存存档
            PlayerPrefs.Save();
            
            // 更新显示文本
            UpdatePriceText();
            
            Debug.Log("购买成功！花费 " + itemPrice + " 金币，剩余金币: " + currentCoins);
            Debug.Log("物品 '" + itemName + "' 数量: " + currentItemCount);
        }
        else
        {
            Debug.Log("金币不足！需要 " + itemPrice + " 金币，当前只有 " + currentCoins + " 金币");
        }
    }
    
    // OnDestroy时移除事件监听，避免内存泄漏
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }
}
