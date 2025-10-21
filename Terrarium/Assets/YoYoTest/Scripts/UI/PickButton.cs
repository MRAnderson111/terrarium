using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickButton : MonoBehaviour
{
    public GameObject selectPrefab;
    public string smallClass;
    private PickPrefabUI PickPrefabUI;
    private Button button;
    
    // 存档键名常量
    private const string ITEMS_PREFIX = "Item_";
    
    // Start is called before the first frame update
    void Start()
    {
        PickPrefabUI = FindObjectOfType<PickPrefabUI>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Debug.Log("选择预制体：" + selectPrefab.name);
        
        // 获取预制体的分类信息
        IGetObjectClass objectClass = selectPrefab.GetComponent<IGetObjectClass>();
        if (objectClass == null)
        {
            Debug.LogWarning($"预制体 {selectPrefab.name} 没有实现IGetObjectClass接口");
            return;
        }
        
        string bigClass = objectClass.BigClass;
        string smallClass = objectClass.SmallClass;
        
        // 获取存档中购买的物品数量
        int purchasedCount = GetPurchasedItemCount(smallClass);
        
        // 获取当前已选择的预制体数量
        int currentCount = PickPrefabUI.Instance.GetSmallClassCount(bigClass, smallClass);
        
        // 检查是否超过购买数量
        if (currentCount >= purchasedCount)
        {
            Debug.Log($"无法添加 {smallClass}！当前已选择 {currentCount} 个，最多只能选择 {purchasedCount} 个");
            return;
        }
        
        // 如果没有超过限制，则添加预制体
        PickPrefabUI.Instance.AddPrefabToClass(selectPrefab);
    }
    
    /// <summary>
    /// 获取存档中购买的物品数量
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>购买的数量</returns>
    private int GetPurchasedItemCount(string itemName)
    {
        return PlayerPrefs.GetInt(ITEMS_PREFIX + itemName, 0);
    }
}
