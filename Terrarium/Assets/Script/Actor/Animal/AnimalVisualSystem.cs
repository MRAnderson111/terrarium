using UnityEngine;

/// <summary>
/// 动物视觉系统 - 处理外观、颜色变化、材质管理等
/// </summary>
public class AnimalVisualSystem : MonoBehaviour, IAnimalVisual
{
    [Header("视觉设置")]
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color adultColor = new Color(0.5f, 0f, 0f, 1f);
    [SerializeField] private Color hungryColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private Color thirstyColor = new Color(1f, 0.5f, 0f, 1f);
    
    // 组件引用
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Material animalMaterial;
    
    // 状态
    private bool isAdult = false;
    private bool isHungry = false;
    private bool isThirsty = false;
    
    // 事件
    public System.Action<Color> OnColorChanged;
    
    void Start()
    {
        InitializeVisualComponents();
    }
    
    private void InitializeVisualComponents()
    {
        // 确保有MeshRenderer和MeshFilter组件
        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }
        
        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
        }
        
        // 获取组件引用
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        
        // 设置为立方体网格
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建材质
        animalMaterial = new Material(Shader.Find("Standard"));
        animalMaterial.color = normalColor;
        meshRenderer.material = animalMaterial;
        
        // 添加碰撞器（如果主控制器还没有添加的话）
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // 注意：刚体组件现在由AnimalItem主控制器负责添加
        // 这里不再重复添加，避免组件初始化顺序问题

        Debug.Log("动物视觉组件初始化完成");
    }
    
    private Mesh CreateCubeMesh()
    {
        // 使用Unity内置的立方体网格
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }
    
    public void ChangeColor(Color color)
    {
        if (animalMaterial != null)
        {
            animalMaterial.color = color;
            OnColorChanged?.Invoke(color);
            Debug.Log($"动物颜色变更为: {color}");
        }
    }
    
    public void RestoreNormalColor()
    {
        Color targetColor = isAdult ? adultColor : normalColor;
        
        // 如果有特殊状态，优先显示状态颜色
        if (isHungry && isThirsty)
        {
            // 饥饿且口渴时显示混合颜色
            targetColor = Color.Lerp(hungryColor, thirstyColor, 0.5f);
        }
        else if (isHungry)
        {
            targetColor = hungryColor;
        }
        else if (isThirsty)
        {
            targetColor = thirstyColor;
        }
        
        ChangeColor(targetColor);
    }
    
    public void UpdateVisualState()
    {
        RestoreNormalColor();
    }
    
    public void SetAdultVisual(bool adult)
    {
        isAdult = adult;
        
        if (adult)
        {
            // 成年体：体型变大，颜色变暗
            transform.localScale = transform.localScale * 2f;
            Debug.Log("设置为成年体视觉效果：体型变大，颜色变暗");
        }
        
        UpdateVisualState();
    }
    
    public void SetHungryVisual(bool hungry)
    {
        isHungry = hungry;
        UpdateVisualState();
        
        if (hungry)
        {
            Debug.Log("设置饥饿视觉效果：变成鲜红色");
        }
    }
    
    public void SetThirstyVisual(bool thirsty)
    {
        isThirsty = thirsty;
        UpdateVisualState();
        
        if (thirsty)
        {
            Debug.Log("设置口渴视觉效果：变成橙色");
        }
    }
    
    public void SetTransparency(float alpha)
    {
        if (animalMaterial != null)
        {
            Color currentColor = animalMaterial.color;
            Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            animalMaterial.color = newColor;
        }
    }
    
    public void SpawnBlueSphere()
    {
        // 在动物正上方3米处生成蓝色小球
        Vector3 spherePosition = transform.position + Vector3.up * 3f;
        
        // 创建球体对象
        GameObject blueSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        blueSphere.name = "BlueSphere";
        blueSphere.transform.position = spherePosition;
        blueSphere.transform.localScale = Vector3.one * 0.3f;
        
        // 设置蓝色材质
        MeshRenderer sphereRenderer = blueSphere.GetComponent<MeshRenderer>();
        Material blueMaterial = new Material(Shader.Find("Standard"));
        blueMaterial.color = Color.blue;
        sphereRenderer.material = blueMaterial;
        
        // 移除刚体组件（保留碰撞器用于点击检测）
        Rigidbody sphereRb = blueSphere.GetComponent<Rigidbody>();
        if (sphereRb != null)
            Destroy(sphereRb);
        
        // 保留碰撞器但设置为触发器
        Collider sphereCollider = blueSphere.GetComponent<Collider>();
        if (sphereCollider != null)
            sphereCollider.isTrigger = true;
        
        // 添加点击脚本
        blueSphere.AddComponent<AnimalBlueSphereClickHandler>();
        
        Debug.Log($"在位置 {spherePosition} 生成了浮空蓝色小球");
    }
    
    // 获取当前颜色
    public Color GetCurrentColor()
    {
        return animalMaterial != null ? animalMaterial.color : Color.white;
    }
    
    // 设置自定义颜色配置
    public void SetColorConfiguration(Color normal, Color adult, Color hungry, Color thirsty)
    {
        normalColor = normal;
        adultColor = adult;
        hungryColor = hungry;
        thirstyColor = thirsty;
        
        UpdateVisualState();
        Debug.Log("更新动物颜色配置");
    }
    
    // 闪烁效果（用于特殊状态提示）
    public void StartBlinking(float duration, float interval)
    {
        StartCoroutine(BlinkingEffect(duration, interval));
    }
    
    private System.Collections.IEnumerator BlinkingEffect(float duration, float interval)
    {
        Color originalColor = GetCurrentColor();
        float elapsedTime = 0f;
        bool isVisible = true;
        
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(interval);
            
            isVisible = !isVisible;
            SetTransparency(isVisible ? originalColor.a : 0f);
            
            elapsedTime += interval;
        }
        
        // 恢复原始颜色
        ChangeColor(originalColor);
    }
    
    // 渐变颜色效果
    public void GradualColorChange(Color targetColor, float duration)
    {
        StartCoroutine(GradualColorChangeCoroutine(targetColor, duration));
    }
    
    private System.Collections.IEnumerator GradualColorChangeCoroutine(Color targetColor, float duration)
    {
        Color startColor = GetCurrentColor();
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            Color currentColor = Color.Lerp(startColor, targetColor, progress);
            ChangeColor(currentColor);
            
            yield return null;
        }
        
        ChangeColor(targetColor);
    }
    
    void OnDestroy()
    {
        // 清理材质资源
        if (animalMaterial != null)
        {
            Destroy(animalMaterial);
        }
    }
}

/// <summary>
/// 动物蓝色小球点击处理脚本
/// </summary>
public class AnimalBlueSphereClickHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        // 增加研究点数
        try
        {
            var researchPointType = System.Type.GetType("UI_ResearchPoint");
            if (researchPointType != null)
            {
                var addMethod = researchPointType.GetMethod("AddResearchPoints");
                var pointsProperty = researchPointType.GetProperty("ResearchPoints");
                
                if (addMethod != null && pointsProperty != null)
                {
                    addMethod.Invoke(null, new object[] { 1 });
                    int currentPoints = (int)pointsProperty.GetValue(null);
                    Debug.Log($"研究点数+1，当前研究点数: {currentPoints}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"添加研究点数失败: {e.Message}");
        }
        
        // 销毁小球
        Destroy(gameObject);
    }
}
