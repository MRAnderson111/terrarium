using UnityEngine;

public class Actor_ResearchPoint : MonoBehaviour
{
    [SerializeField] private float sphereSize = 0.5f; // 球体大小   

    void Start()
    {
        CreateBlueSphere();
    }

    private void CreateBlueSphere()
    {
        // 创建基础球体
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // 移除碰撞体
        Collider collider = sphere.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }

        // 设置蓝色材质
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material blueMaterial = new(Shader.Find("Standard"))
            {
                color = Color.blue
            };
            renderer.material = blueMaterial;
        }

        // 设置大小
        sphere.transform.localScale = Vector3.one * sphereSize;

        // 设置位置为当前对象位置
        sphere.transform.position = transform.position;

        // 设置为当前对象的子对象
        sphere.transform.SetParent(this.transform);
    }
}
