using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectButton : MonoBehaviour
{
    public GameObject selectPrefab;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public int quantity;
    public int price;
    public Button addButton;

    Button button;
    Color normalColor;

    void Awake()
    {
        button = GetComponent<Button>();
        normalColor = button.targetGraphic != null ? button.targetGraphic.color : Color.white;
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        if (addButton != null)
            addButton.onClick.AddListener(OnAddButtonClick);
        RefreshLabels();
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        if (addButton != null)
            addButton.onClick.RemoveListener(OnAddButtonClick);
    }

    public void OnClick()
    {
        if (selectPrefab != null)
            Events.OnSelectPrefab.Invoke(selectPrefab, this);
    }

    public void SetSelected(bool selected)
    {
        if (button.targetGraphic != null)
            button.targetGraphic.color = selected ? new Color(0.65f, 1f, 0.65f) : normalColor;
    }

    public void ApplyStock(int stock)
    {
        quantity = Mathf.Max(0, stock);
        RefreshLabels();
    }

    public void DecrementQuantity() => ApplyStock(quantity - 1);

    void OnAddButtonClick()
    {
        CreateManager manager = CreateManager.Instance;
        if (manager == null || manager.goldCount < price)
            return;
        manager.AddGold(-price);
        ApplyStock(quantity + 1);
    }

    void RefreshLabels()
    {
        if (nameText != null && selectPrefab != null)
            nameText.text = selectPrefab.name;
        if (quantityText != null)
            quantityText.text = quantity.ToString();
    }
}
