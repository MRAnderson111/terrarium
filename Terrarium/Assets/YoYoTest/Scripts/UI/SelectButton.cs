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

    Text legacyNameText;
    Text legacyQuantityText;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);

        if (addButton != null)
            addButton.onClick.AddListener(OnAddButtonClick);

        Image image = GetComponent<Image>();
        if (image != null && image.sprite == null)
            image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");

        BindLegacyLabels();
        RefreshLabels();
    }

    void BindLegacyLabels()
    {
        if (nameText != null && quantityText != null)
            return;

        Text[] labels = GetComponentsInChildren<Text>(true);
        foreach (Text label in labels)
        {
            if (label.gameObject == gameObject)
                continue;
            if (legacyNameText == null && label.gameObject.name == "Text")
                legacyNameText = label;
            else if (legacyQuantityText == null)
                legacyQuantityText = label;
        }
    }

    void RefreshLabels()
    {
        string label = selectPrefab != null ? selectPrefab.name : gameObject.name;
        if (nameText != null)
            nameText.text = label;
        else if (legacyNameText != null)
            legacyNameText.text = label;

        if (quantityText != null)
            quantityText.text = quantity.ToString();
        else if (legacyQuantityText != null)
            legacyQuantityText.text = quantity.ToString();
    }

    void OnAddButtonClick()
    {
        if (CreateManager.Instance != null)
        {
            if (CreateManager.Instance.goldCount >= price)
            {
                CreateManager.Instance.AddGold(-price);
                quantity++;
                RefreshLabels();
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
        if (selectPrefab == null)
            return;

        Debug.Log("选择预制体：" + selectPrefab.name);
        Events.OnSelectPrefab.Invoke(selectPrefab, this);
        
        // 触发数量减一的action
        // onQuantityDecrease?.Invoke();
    }
    
    // 独立的让数量减一的方法
    public void DecrementQuantity()
    {
        quantity--;
        RefreshLabels();
        // Debug.Log("当前数量：" + quantity);
    }
}
