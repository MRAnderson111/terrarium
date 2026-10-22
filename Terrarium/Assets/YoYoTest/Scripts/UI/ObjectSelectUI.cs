using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ObjectSelectUI : MonoBehaviour
{
    public GameObject plantButtonGroup;
    public GameObject animalButtonGroup;
    public GameObject killerButtonGroup;
    public GameObject selectButtonPrefab;
    public TextAsset jsonFile;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>(true);
        if (canvas == null)
            return;

        if (canvas.transform.localScale == Vector3.zero)
            canvas.transform.localScale = Vector3.one;
        canvas.sortingOrder = Mathf.Max(canvas.sortingOrder, 10);
    }

    void Start()
    {
        DisableVrPoke();
        LoadFromJson();
    }

    public void LoadFromJson(TextAsset externalJsonFile = null)
    {
        TextAsset jsonAsset = externalJsonFile != null ? externalJsonFile : jsonFile;
        if (jsonAsset == null)
        {
            Debug.LogError("JSON文件未分配！");
            return;
        }

        Dictionary<string, int> stock = ParseStock(jsonAsset.text);
        SelectButton[] buttons = GetComponentsInChildren<SelectButton>(true);
        int matched = 0;
        foreach (SelectButton button in buttons)
        {
            if (button == null || button.selectPrefab == null)
                continue;
            if (!TryMatchQuantity(stock, button.selectPrefab.name, out int quantity))
                continue;

            button.ApplyStock(quantity);
            matched++;
        }

        Debug.Log("已把物种数量同步到现有按钮：" + matched);
    }

    void DisableVrPoke()
    {
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour != null && behaviour.GetType().Name == "XRPokeFollowAffordance")
                behaviour.enabled = false;
        }
    }

    static Dictionary<string, int> ParseStock(string json)
    {
        var stock = new Dictionary<string, int>();
        if (string.IsNullOrEmpty(json))
            return stock;

        MatchCollection matches = Regex.Matches(json, "\"([A-Za-z0-9_]+)\"\\s*:\\s*(\\d+)");
        foreach (Match match in matches)
            stock[match.Groups[1].Value] = int.Parse(match.Groups[2].Value);

        return stock;
    }

    static bool TryMatchQuantity(Dictionary<string, int> stock, string prefabName, out int quantity)
    {
        quantity = 0;
        if (string.IsNullOrEmpty(prefabName) || stock.Count == 0)
            return false;

        if (stock.TryGetValue(prefabName, out quantity))
            return true;

        string stripped = prefabName;
        if (stripped.StartsWith("PF_1_"))
            stripped = stripped.Substring(5);
        else if (stripped.StartsWith("PF_"))
            stripped = stripped.Substring(3);

        if (stock.TryGetValue(stripped, out quantity))
            return true;

        foreach (KeyValuePair<string, int> pair in stock)
        {
            if (prefabName.IndexOf(pair.Key, StringComparison.OrdinalIgnoreCase) >= 0
                || stripped.IndexOf(pair.Key, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                quantity = pair.Value;
                return true;
            }
        }

        return false;
    }
}
