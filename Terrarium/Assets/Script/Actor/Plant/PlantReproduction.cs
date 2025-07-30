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

    public void StartDoubleReproduction()
    {
        
        // 繁衍第一个植物
        Vector3 reproductionPosition1 = transform.position + new Vector3(
            Random.Range(-3f, 5f), 7f, Random.Range(-5f, 3f)
        );
        GameObject newPlant1 = Instantiate(plantPrefab, reproductionPosition1, Quaternion.identity);
        if (newPlant1 != null)
        {
            newPlant1.name = "Actor_Plant_Offspring_1";
            Debug.Log($"成功生成第一个后代在位置: {reproductionPosition1}");
        }
        Rigidbody rb1 = newPlant1.GetComponent<Rigidbody>();
        if (rb1 == null)
        {
            rb1 = newPlant1.AddComponent<Rigidbody>();
        }
        rb1.mass = 1f;

        // 繁衍第二个植物
        Vector3 reproductionPosition2 = transform.position + new Vector3(
            Random.Range(-5f, 3f), 7f, Random.Range(-3f, 5f)
        );
        GameObject newPlant2 = Instantiate(plantPrefab, reproductionPosition2, Quaternion.identity);
        if (newPlant2 != null)
        {
            newPlant2.name = "Actor_Plant_Offspring_2";
            Debug.Log($"成功生成第二个后代在位置: {reproductionPosition2}");
        }
        Rigidbody rb2 = newPlant2.GetComponent<Rigidbody>();
        if (rb2 == null)
        {
            rb2 = newPlant2.AddComponent<Rigidbody>();
        }
        rb2.mass = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
