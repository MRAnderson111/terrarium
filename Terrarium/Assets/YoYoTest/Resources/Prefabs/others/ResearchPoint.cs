using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    public GameObject fxPrefab;
    public CreateManager createManager;
    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        if (createManager == null)
            createManager = CreateManager.Instance;
    }

    void Update()
    {
        bool hovered = IsHitByRay();
        transform.localScale = Vector3.Lerp(transform.localScale,
            originalScale * (hovered ? 1.3f : 1f), Time.deltaTime * 5f);
        if (!hovered || !Input.GetMouseButtonDown(0))
            return;
        if (fxPrefab != null)
            Instantiate(fxPrefab, transform.position, transform.rotation);
        createManager.AddGold(100);
        Destroy(gameObject);
    }

    bool IsHitByRay()
    {
        if (createManager == null || createManager.placementCamera == null ||
            createManager.selectPrefab != null || createManager.IsPointerOverUI(Input.mousePosition))
            return false;
        Ray ray = createManager.placementCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) &&
            (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform));
    }
}