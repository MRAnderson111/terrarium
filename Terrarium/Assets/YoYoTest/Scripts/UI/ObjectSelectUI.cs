using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ObjectSelectUI : MonoBehaviour
{
    public TextAsset jsonFile;

    void Start() => LoadFromJson();

    // The scene owns the buttons and prefab references; JSON only supplies stock.
    public void LoadFromJson(TextAsset externalJsonFile = null)
    {
        TextAsset data = externalJsonFile != null ? externalJsonFile : jsonFile;
        if (data == null)
            return;

        var stock = new Dictionary<string, int>();
        foreach (Match match in Regex.Matches(data.text, "\"([A-Za-z0-9_]+)\"\\s*:\\s*(\\d+)"))
            if (int.TryParse(match.Groups[2].Value, out int quantity))
                stock[match.Groups[1].Value] = quantity;

        foreach (SelectButton button in GetComponentsInChildren<SelectButton>(true))
            if (button.selectPrefab != null && stock.TryGetValue(button.selectPrefab.name, out int quantity))
                button.ApplyStock(quantity);
    }
}
