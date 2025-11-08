using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    public GameObject selectPrefab;
    private Button button;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public int quantity;
    public int price;
    public Button addButton;

    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        

        addButton.onClick.AddListener(OnAddButtonClick);
        quantityText.text = quantity.ToString();

        nameText.text = selectPrefab.name;
    }

    void OnAddButtonClick()
    {
        if (CreateManager.Instance != null)
        {
            if (CreateManager.Instance.goldCount >= price)
            {
                CreateManager.Instance.AddGold(-price);
                quantity++;
                quantityText.text = quantity.ToString();
            }
            else
            {
                Debug.Log("金币不足，无法购买！");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Debug.Log("选择预制体：" + selectPrefab.name);
        Events.OnSelectPrefab.Invoke(selectPrefab, this);
        
        // 触发数量减一的action
        // onQuantityDecrease?.Invoke();
    }
    
    // 独立的让数量减一的方法
    public void DecrementQuantity()
    {
        quantity--;
        quantityText.text = quantity.ToString();
        // Debug.Log("当前数量：" + quantity);
    }
}
