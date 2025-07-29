using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_PlantIndex : MonoBehaviour
{
    public GameObject plant1;
    public GameObject plant2;
    public GameObject plant3;
    public GameObject plant4;
    public GameObject Ground;

    
    private GameObject currentFollowingPlant; // 当前跟随鼠标的植物


    void Start()
    {
        // 订阅事件
        ActorManager.OnPlantIndexChanged += OnPlantIndexChanged;
    }

    // 事件回调方法
    void OnPlantIndexChanged(int newIndex)
    {
        Debug.Log($"Interact_PlantIndex 收到通知：PlantIndex 变为 {newIndex}");
        
        // 在这里处理PlantIndex变化的逻辑
        HandlePlantIndexChange(newIndex);
    }

    void HandlePlantIndexChange(int index)
    {
        // 先销毁之前跟随的植物
        if (currentFollowingPlant != null)
        {
            Destroy(currentFollowingPlant);
            currentFollowingPlant = null;
        }
        
        switch (index)
        {
            case 0:
                Debug.Log("植物索引重置");
                break;
            case 1:
                Debug.Log("选择了第一种植物");
                CreateFollowingPlant(plant1);
                break;
            case 2:
                Debug.Log("选择了第二种植物");
                CreateFollowingPlant(plant2);
                break;
            case 3:
                Debug.Log("选择了第三种植物");
                CreateFollowingPlant(plant3);
                break;
            case 4:
                Debug.Log("选择了第四种植物");
                CreateFollowingPlant(plant4);
                break;
        }
    }

    void CreateFollowingPlant(GameObject plantPrefab)
    {
        if (plantPrefab != null)
        {
            // 在鼠标位置生成植物
            currentFollowingPlant = Instantiate(plantPrefab);

            Debug.Log("生成了跟随鼠标的植物");
        }
    }

    void Update()
    {
        // 如果有跟随的植物，让它跟随鼠标
        if (currentFollowingPlant != null)
        {
            UpdatePlantPosition();
            
            // 检测鼠标左键点击，释放植物让其自由下落
            if (Input.GetMouseButtonDown(0))
            {
                DropPlant();
            }
        }
    }

    void UpdatePlantPosition()
    {
        // 从摄像机发射射线到鼠标位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // 检测射线与地面的碰撞
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 让植物跟随鼠标，但在地面上方30ft处浮空
            Vector3 floatingPosition = hit.point + Vector3.up * 30f;
            currentFollowingPlant.transform.position = floatingPosition;
        }
    }

    void DropPlant()
    {
        if (currentFollowingPlant != null)
        {
            // 添加刚体组件让植物自由下落
            Rigidbody rb = currentFollowingPlant.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = currentFollowingPlant.AddComponent<Rigidbody>();
            }
            
            // 确保有碰撞器
            if (currentFollowingPlant.GetComponent<Collider>() == null)
            {
                currentFollowingPlant.AddComponent<BoxCollider>();
                Debug.Log("为下落植物添加了碰撞器");
            }
            
            // 启用重力让植物下落
            rb.useGravity = true;
            rb.isKinematic = false;
            
            Debug.Log("植物开始自由下落至地面");
            
            // 清空当前跟随的植物引用
            currentFollowingPlant = null;
        }
    }
}
