using System.Collections;
using UnityEngine;

public class PlantGrowthSystem : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    private bool isGrowing = false;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            plantMaterial = meshRenderer.material;
        }
    }

    public void StartGrowth()
    {
        if (!isGrowing && plantMaterial != null)
        {
            isGrowing = true;
            StartCoroutine(DelayedGrowth());
        }
    }

    IEnumerator DelayedGrowth()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);
        
        // 开始生长
        yield return StartCoroutine(GrowPlant());
    }

    IEnumerator GrowPlant()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f; // 体型变大2倍
        Color originalColor = plantMaterial.color;
        Color targetColor = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f); // 颜色变深
        
        float growthTime = 3f; // 生长时间3秒
        float elapsedTime = 0f;
        
        Debug.Log("植物开始从幼年体成长为壮年体");
        
        while (elapsedTime < growthTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthTime;
            
            // 平滑插值缩放和颜色
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            plantMaterial.color = Color.Lerp(originalColor, targetColor, progress);
            
            yield return null;
        }
        
        // 确保最终值准确
        transform.localScale = targetScale;
        plantMaterial.color = targetColor;
        
        Debug.Log("植物成长完成：体型变大，颜色变深");
        isGrowing = false;
    }

    public bool IsGrowing => isGrowing;
}
