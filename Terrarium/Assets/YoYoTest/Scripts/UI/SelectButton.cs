using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour, IPointerClickHandler
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
    bool clickBound;

    void Awake()
    {
        BindClick();
    }

    public void BindClick()
    {
        if (clickBound)
            return;

        button = GetComponent<Button>();
        if (button == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button candidate in buttons)
            {
                if (addButton != null && candidate == addButton)
                    continue;
                button = candidate;
                break;
            }
        }

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }

        if (addButton != null)
        {
            addButton.onClick.RemoveListener(OnAddButtonClick);
            addButton.onClick.AddListener(OnAddButtonClick);
        }

        clickBound = true;
    }

    void Start()
    {
        BindClick();

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

    public void ApplyStock(int stock)
    {
        quantity = Mathf.Max(0, stock);
        RefreshLabels();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (addButton != null && eventData.pointerPress != null
            && (eventData.pointerPress == addButton.gameObject || eventData.pointerPress.transform.IsChildOf(addButton.transform)))
            return;

        OnClick();
    }

    public void OnClick()
    {
        if (selectPrefab == null)
        {
            Debug.LogWarning(name + " 没有对应的预制体");
            return;
        }

        Debug.Log("选择预制体：" + selectPrefab.name + "，剩余数量：" + quantity);
        Events.OnSelectPrefab.Invoke(selectPrefab, this);
    }
    
    // 独立的让数量减一的方法
    public void DecrementQuantity()
    {
        quantity--;
        RefreshLabels();
        // Debug.Log("当前数量：" + quantity);
    }
}
