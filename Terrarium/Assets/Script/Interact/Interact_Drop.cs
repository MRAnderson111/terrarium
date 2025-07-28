using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Drop : MonoBehaviour
{
    public GameObject plantPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 按下左键时在地图中心生成植物预制体
        if (Input.GetMouseButtonDown(0))
        {
            SpawnPlantAtCenter();
        }
    }

    void SpawnPlantAtCenter()
    {
        if (plantPrefab != null)
        {
            // 在地图中心点(0,0,0)实例化植物预制体
            GameObject newPlant = Instantiate(plantPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("在地图中心点生成了植物预制体");
        }
    }
}
