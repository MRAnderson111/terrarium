using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_AnimalHabitat : MonoBehaviour
{
    // 栖息地属性
    private bool isInitialized = false;
    
    void Start()
    {
        InitializeHabitat();
    }
    
    void InitializeHabitat()
    {
        if (isInitialized) return;
        
        SetupComponents();
        SetupAppearance();
        SetupPhysics();
        
        isInitialized = true;
        Debug.Log("动物栖息地初始化完成");
    }
    
    void SetupComponents()
    {
        // 确保有必要的组件
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
        
        if (GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }
    
    void SetupAppearance()
    {
        // 设置为大型立方体网格
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateLargeBoxMesh();
        
        // 创建半透明灰色材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material habitatMaterial = new Material(Shader.Find("Standard"));
        
        // 设置为透明渲染模式
        habitatMaterial.SetFloat("_Mode", 3); // Transparent mode
        habitatMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        habitatMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        habitatMaterial.SetInt("_ZWrite", 0);
        habitatMaterial.DisableKeyword("_ALPHATEST_ON");
        habitatMaterial.EnableKeyword("_ALPHABLEND_ON");
        habitatMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        habitatMaterial.renderQueue = 3000;
        
        // 设置半透明灰色
        habitatMaterial.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // 30%透明度
        meshRenderer.material = habitatMaterial;
        
        // 设置大型尺寸
        transform.localScale = new Vector3(10f, 3f, 10f);
    }
    
    void SetupPhysics()
    {
        // 设置为静态物体，不需要刚体
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false; // 保持物理碰撞
        }
    }
    
    Mesh CreateLargeBoxMesh()
    {
        // 使用Unity内置的立方体网格
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh boxMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return boxMesh;
    }
    
    void Update()
    {
        // 栖息地通常是静态的，不需要更新逻辑
    }
    
    // 检查是否有动物在栖息地内
    void OnTriggerEnter(Collider other)
    {
        if (IsAnimalItem(other.gameObject))
        {
            Debug.Log($"动物 {other.name} 进入栖息地");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsAnimalItem(other.gameObject))
        {
            Debug.Log($"动物 {other.name} 离开栖息地");
        }
    }
    
    bool IsAnimalItem(GameObject obj)
    {
        try
        {
            var animalItemType = System.Type.GetType("AnimalItem");
            if (animalItemType != null)
            {
                return obj.GetComponent(animalItemType) != null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"检查动物组件失败: {e.Message}");
        }
        return false;
    }
}
