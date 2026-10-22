using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateManager : MonoBehaviour
{
    public static CreateManager Instance { get; private set; }

    [Header("Scene references")]
    public Camera placementCamera;
    public EventSystem eventSystem;
    public GameObject selectPosition;
    public TMP_Text placementStatus;

    [Header("Placement")]
    public float maxRayDistance = 200f;
    public float placeHeightOffset = 0.05f;
    public LayerMask placementLayers = Physics.DefaultRaycastLayers;
    public int goldCount = 1000;

    [Header("Debug (Scene view Gizmos)")]
    public bool useAdvancedRayVisualizer;
    public AdvancedRayVisualizer rayVisualizer;

    [Header("Runtime state")]
    public GameObject selectPrefab;
    public SelectButton selectButton;
    public bool isHit;
    public bool pointerOverUI;

    readonly List<RaycastResult> uiHits = new();
    readonly PlacementPreview preview = new();
    PointerEventData pointer;
    Vector3 placePoint;
    int selectionFrame;

    void Awake()
    {
        Instance = this;
        if (placementCamera == null)
            placementCamera = Camera.main;
        if (eventSystem == null)
            eventSystem = EventSystem.current;
        if (eventSystem != null)
            pointer = new PointerEventData(eventSystem);
        if (selectPosition != null)
            selectPosition.SetActive(false);
    }

    void OnEnable() => Events.OnSelectPrefab.AddListener(OnSelectPrefab);

    void OnDisable()
    {
        Events.OnSelectPrefab.RemoveListener(OnSelectPrefab);
        ClearSelection();
        if (selectPosition != null)
            selectPosition.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ClearSelection();

        Vector2 mousePosition = Input.mousePosition;
        pointerOverUI = IsPointerOverUI(mousePosition);
        isHit = false;
        if (placementCamera != null && placementCamera.pixelRect.Contains(mousePosition) && !pointerOverUI)
        {
            Ray ray = placementCamera.ScreenPointToRay(mousePosition);
            isHit = TryGetGroundHit(ray, out RaycastHit hit);
            if (isHit)
                placePoint = hit.point;

            if (useAdvancedRayVisualizer)
            {
                Vector3 end = isHit ? placePoint : ray.GetPoint(maxRayDistance);
                Debug.DrawLine(ray.origin, end, isHit ? Color.green : Color.red);
                if (rayVisualizer != null)
                    rayVisualizer.DrawRay(ray.origin, end, isHit);
            }
        }

        if (selectPosition != null)
        {
            selectPosition.SetActive(isHit);
            if (isHit)
                selectPosition.transform.position = placePoint + Vector3.up * 0.02f;
        }

        if (selectPrefab == null)
        {
            ShowStatus("Select a species from the menu");
            return;
        }

        bool hasStock = selectButton == null || selectButton.quantity > 0;
        ShowStatus(!hasStock ? selectPrefab.name + " - out of stock (use +)"
            : selectPrefab.name + (isHit ? " - click to place | Esc: cancel" : " - move cursor onto soil | Esc: cancel"));
        if (!isHit || !hasStock)
            return;

        Vector3 position = placePoint + Vector3.up * (placeHeightOffset - preview.BottomOffset);
        preview.Draw(position, Quaternion.identity, placementCamera);
        if (Input.GetMouseButtonDown(0) && Time.frameCount != selectionFrame)
        {
            if (CreatePrefab(selectPrefab, position) == null)
                return;
            if (selectButton != null)
                selectButton.DecrementQuantity();
            if (StaticCreateLimitManager.HasCreateLimit(selectPrefab.name))
                StaticCreateLimitManager.AddToCreateLimit(selectPrefab.name);
            ClearSelection();
        }
    }

    // Raycast this frame's UI position instead of mixing legacy pointer IDs with Input System device IDs.
    public bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (eventSystem == null || pointer == null)
            return false;
        pointer.Reset();
        pointer.position = screenPosition;
        uiHits.Clear();
        eventSystem.RaycastAll(pointer, uiHits);
        foreach (RaycastResult hit in uiHits)
            if (hit.module is GraphicRaycaster)
                return true;
        return false;
    }

    public bool TryGetGroundHit(Ray ray, out RaycastHit groundHit)
    {
        groundHit = default;
        float nearest = float.PositiveInfinity;
        foreach (RaycastHit hit in Physics.RaycastAll(ray, maxRayDistance, placementLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance >= nearest)
                continue;
            for (Transform current = hit.collider.transform; current != null; current = current.parent)
            {
                if (!current.CompareTag("Ground") && current.GetComponent<IGetGroundClass>() == null)
                    continue;
                groundHit = hit;
                nearest = hit.distance;
                break;
            }
        }
        return nearest < float.PositiveInfinity;
    }

    public void OnSelectPrefab(GameObject prefab, SelectButton button)
    {
        ClearSelection();
        selectPrefab = prefab;
        selectButton = button;
        selectionFrame = Time.frameCount;
        if (selectButton != null)
            selectButton.SetSelected(true);
        if (prefab != null)
            preview.SetPrefab(prefab);
    }

    public void ClearSelection()
    {
        if (selectButton != null)
            selectButton.SetSelected(false);
        selectPrefab = null;
        selectButton = null;
        preview.Clear();
    }

    void ShowStatus(string message)
    {
        if (placementStatus != null && placementStatus.text != message)
            placementStatus.text = message;
    }

    public void AddGold(int amount) => goldCount += amount;

    // Also used by reproduction; only menu placement consumes the button's stock.
    public GameObject CreatePrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return null;
        ObjectStatisticsManager stats = ObjectStatisticsManager.Instance;
        int limit = StaticCreateLimitManager.GetCreateLimit(prefab.name, 10);
        if (stats != null && stats.smallClassCount.TryGetValue(prefab.name, out int count) && count >= limit)
        {
            Debug.LogWarning($"Cannot create {prefab.name}: population limit {limit} reached.", this);
            return null;
        }
        return Instantiate(prefab, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
    }

    public int GetCurrentQuantityLimit(string smallClass)
    {
        foreach (MonoBehaviour component in FindObjectsOfType<MonoBehaviour>())
            if (component is IGetQuantityLimits limits &&
                component.TryGetComponent(out IGetObjectClass objectClass) && objectClass.SmallClass == smallClass)
                return limits.QuantityLimits;
        return 10;
    }
}
