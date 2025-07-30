using System.Collections;
using UnityEngine;

public class PlantGrowthSystem : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material plantMaterial;
    private bool isGrowing = false;
    public bool FirstGrowthDone = false;
    public bool SecondGrowthDone = false;
    public System.Action OnFirstGrowthCompleted;
    public System.Action OnSecondGrowthCompleted;

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
        
        // 从Actor_Plant获取生长速度
        Actor_Plant actorPlant = GetComponent<Actor_Plant>();
        float GrowSpeed = actorPlant.GrowSpeed;

        float growthTime = 3f;
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
        FirstGrowthDone = true;
        
        OnFirstGrowthCompleted?.Invoke();
    }

    public bool IsGrowing => isGrowing;

    public void SecondGrowth()
    {
        if (!isGrowing && plantMaterial != null && FirstGrowthDone && !SecondGrowthDone) 
        {
            isGrowing = true;
            StartCoroutine(DelayedSecondGrowth());
        }
    }

    IEnumerator DelayedSecondGrowth()
    {
        // 等待3秒
        yield return new WaitForSeconds(3f);
        
        // 开始第二阶段生长
        yield return StartCoroutine(SecondGrowthProcess());
    }

    IEnumerator SecondGrowthProcess()
    {
        Vector3 originalScale = transform.localScale;
        
        // 从Actor_Plant获取生长速度
        Actor_Plant actorPlant = GetComponent<Actor_Plant>();
        float speedMultiplier = actorPlant.GrowSpeed;
        float heightMultiplier = 2.5f * speedMultiplier; // 根据生长速度调整长高倍数
        
        // 设置生长上限，最高不超过3倍
        heightMultiplier = Mathf.Min(heightMultiplier, 5f);
        
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y * heightMultiplier, originalScale.z);
        
        Color originalColor = plantMaterial.color;
        Color targetColor = new Color(0f, 0.5f, 0f, 1f); // 深绿色
        
        float growthTime = 5f / speedMultiplier; // 根据速度调整生长时间：速度越快，时间越短
        float elapsedTime = 0f;
        
        Debug.Log($"植物开始第二阶段生长：长高{heightMultiplier}倍，生长时间{growthTime:F1}秒");
        
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
        
        Debug.Log("植物第二阶段生长完成：长高且颜色更深");
        isGrowing = false;
        SecondGrowthDone = true;
        OnSecondGrowthCompleted?.Invoke();
    }
}











