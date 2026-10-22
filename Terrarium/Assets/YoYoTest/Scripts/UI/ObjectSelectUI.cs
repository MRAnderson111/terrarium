using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectUI : MonoBehaviour
{
    public GameObject plantButtonGroup;
    public GameObject animalButtonGroup;
    public GameObject killerButtonGroup;
    public GameObject selectButtonPrefab;
    public TextAsset jsonFile;

    static readonly string[] PlantSpecies =
    {
        "Plantain", "Asplenium", "MedinillaMagnifica", "Schefflera", "VenusFlytrap",
        "Yarrow", "SweetWoodruff", "BigFern", "CalatheaCrocata", "MonsteraDeliciosa"
    };

    static readonly string[] AnimalSpecies =
    {
        "LeafCuttingAnt", "LadyBug", "Hercules", "Cricket", "PotatoBeetle", "Roach", "OryctesNasicornis"
    };

    static readonly string[] KillerSpecies =
    {
        "Gecko", "Spider", "Frog", "Mantis", "Scorpion", "Centipede", "LucanusCervus"
    };

    void Awake()
    {
        MakeMouseClickable();
        DisableVrPoke();
    }

    void Start()
    {
        LoadFromJson();
    }

    void MakeMouseClickable()
    {
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = null;
            if (canvas.transform.localScale == Vector3.zero)
                canvas.transform.localScale = Vector3.one;
            canvas.overrideSorting = true;
            canvas.sortingOrder = canvas == GetComponent<Canvas>() ? 10 : 20;
        }

        foreach (GraphicRaycaster raycaster in GetComponentsInChildren<GraphicRaycaster>(true))
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        foreach (Collider collider in GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        foreach (MonoBehaviour behaviour in GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour != null && behaviour.GetType().Name == "TrackedDeviceGraphicRaycaster")
                behaviour.enabled = false;
        }

        foreach (Selectable selectable in GetComponentsInChildren<Selectable>(true))
        {
            foreach (Graphic graphic in selectable.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = true;
        }
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
        Transform menu = transform.Find("LeftControllerUI");
        if (menu == null)
        {
            Debug.LogError("找不到 LeftControllerUI，无法绑定物种按钮");
            return;
        }

        int plantCount = BindPanel(menu.Find("Plant"), PlantSpecies, stock);
        int animalCount = BindPanel(menu.Find("Ant"), AnimalSpecies, stock);
        int killerCount = BindPanel(menu.Find("AntKiller"), KillerSpecies, stock);
        Debug.Log($"物种按钮已绑定：植物 {plantCount}，动物 {animalCount}，食肉动物 {killerCount}");
    }

    int BindPanel(Transform panel, string[] species, Dictionary<string, int> stock)
    {
        if (panel == null)
            return 0;

        List<SelectButton> buttons = new List<SelectButton>();
        foreach (Transform child in panel)
        {
            SelectButton button = child.GetComponent<SelectButton>();
            if (button == null)
                button = child.GetComponentInChildren<SelectButton>(true);
            if (button != null)
                buttons.Add(button);
        }

        HashSet<string> used = new HashSet<string>();
        foreach (SelectButton button in buttons)
        {
            string currentName = button.selectPrefab != null ? button.selectPrefab.name : null;
            if (currentName != null && Array.IndexOf(species, currentName) >= 0)
                used.Add(currentName);
        }

        int nextSpecies = 0;
        foreach (SelectButton button in buttons)
        {
            string currentName = button.selectPrefab != null ? button.selectPrefab.name : null;
            bool valid = currentName != null && Array.IndexOf(species, currentName) >= 0;
            if (!valid)
            {
                while (nextSpecies < species.Length && used.Contains(species[nextSpecies]))
                    nextSpecies++;
                if (nextSpecies < species.Length)
                {
                    GameObject prefab = ResourceLoadUtils.LoadPrefabByName(species[nextSpecies]);
                    if (prefab != null)
                    {
                        button.selectPrefab = prefab;
                        currentName = prefab.name;
                        used.Add(currentName);
                    }
                    nextSpecies++;
                }
            }

            int quantity = 1;
            if (currentName != null && stock.TryGetValue(currentName, out int jsonQuantity))
                quantity = Mathf.Max(1, jsonQuantity);
            button.ApplyStock(quantity);
            button.BindClick();
        }

        return buttons.Count;
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
}
