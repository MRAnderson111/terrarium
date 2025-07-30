using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantReproduction : MonoBehaviour
{
    public GameObject plantPrefab;
    private bool hasReproduced = false; // 最简单的防重复标志

    void Start()
    {

    }

    public void StartReproduction()
    {
        // 最简单的防重复检查
        if (hasReproduced) return;
        hasReproduced = true;

        // 在植株周围随机位置生成新植物
        Vector3 reproductionPosition = transform.position + new Vector3(
            Random.Range(-5f, 5f), // X轴随机偏移
            10f,                   // Y轴高度，让植物从上方掉落
            Random.Range(-5f, 5f)  // Z轴随机偏移
        );
        
        // 使用plantPrefab实例化新植物
        GameObject newPlant = Instantiate(plantPrefab, reproductionPosition, Quaternion.identity);
        newPlant.name = "Actor_Plant_Offspring";
        
        // 添加刚体让植物自然下落
        Rigidbody rb = newPlant.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = newPlant.AddComponent<Rigidbody>();
        }
        rb.mass = 1f;
        
        Debug.Log($"植物在位置 {reproductionPosition} 繁殖了新的后代");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
