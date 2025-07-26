using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Predator : MonoBehaviour
{
    private static bool keyProcessedThisFrame = false; // 静态变量控制每帧只处理一次
    private bool hasGrounded = false; // 是否已经接触地面
    private bool isMovingToAnimal = false; // 是否正在向动物移动
    private Transform targetAnimal = null; // 目标动物
    private readonly float moveSpeed = 12f; // 移动速度（比动物稍快）
    
    // Start is called before the first frame update
    void Start()
    {
        // 设置为立方体网格
        if (!TryGetComponent<MeshFilter>(out var meshFilter))
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateCubeMesh();
        
        // 创建深紫色材质
        if (!TryGetComponent<MeshRenderer>(out var meshRenderer))
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Material predatorMaterial = new(Shader.Find("Standard"))
        {
            color = new Color(0.3f, 0f, 0.5f, 1f) // 深紫色
        };
        meshRenderer.material = predatorMaterial;
        
        // 添加碰撞器和刚体
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyInput();
        
        // 向动物移动
        if (isMovingToAnimal && targetAnimal != null)
        {
            MoveTowardsAnimal();
        }
        // 如果目标动物被销毁，寻找新目标
        else if (isMovingToAnimal && targetAnimal == null)
        {
            FindNearestAnimal();
            if (targetAnimal != null)
            {
                Debug.Log($"Predator目标动物已消失，找到新目标: {targetAnimal.name}");
            }
            else
            {
                isMovingToAnimal = false;
                Debug.Log("Predator未找到新的动物目标，停止移动");
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 改进的地面检测：排除与动物的碰撞
        if (!hasGrounded)
        {
            // 首先检查碰撞对象是否为动物，如果是则忽略
            if (IsAnimalObject(collision.gameObject))
            {
                Debug.Log($"Predator碰撞到动物 {collision.gameObject.name}，忽略碰撞");
                return;
            }

            // 检查是否为地面对象
            if (IsGroundObject(collision.gameObject))
            {
                hasGrounded = true;

                // 完全固定捕食者位置，移除刚体组件
                if (TryGetComponent<Rigidbody>(out var rb))
                {
                    DestroyImmediate(rb);
                }

                Debug.Log($"Predator落地在 {collision.gameObject.name}，完全固定位置");

                // 2秒后开始寻找动物
                StartCoroutine(StartMovingToAnimalAfterDelay());
            }
            else
            {
                Debug.Log($"Predator碰撞到 {collision.gameObject.name}，但不是地面，继续下落");
            }
        }
    }

    System.Collections.IEnumerator StartMovingToAnimalAfterDelay()
    {
        // 等待2秒
        yield return new WaitForSeconds(2f);

        // 寻找最近的动物
        FindNearestAnimal();

        if (targetAnimal != null)
        {
            isMovingToAnimal = true;
            Debug.Log($"Predator开始向最近的动物移动: {targetAnimal.name}");
        }
        else
        {
            Debug.Log("Predator未找到附近的动物");
        }
    }
    
    void FindNearestAnimal()
    {
        // 使用反射查找所有带有AnimalItem组件的对象
        Component[] allAnimals = null;
        try
        {
            var animalItemType = System.Type.GetType("AnimalItem");
            if (animalItemType != null)
            {
                allAnimals = FindObjectsOfType(animalItemType) as Component[];
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"查找动物对象失败: {e.Message}");
            targetAnimal = null;
            return;
        }
        
        if (allAnimals == null || allAnimals.Length == 0)
        {
            targetAnimal = null;
            return;
        }
        
        float nearestDistance = float.MaxValue;
        Transform nearestAnimal = null;
        
        foreach (Component animal in allAnimals)
        {
            if (animal != null && animal.transform != null)
            {
                float distance = Vector3.Distance(transform.position, animal.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestAnimal = animal.transform;
                }
            }
        }
        
        targetAnimal = nearestAnimal;
        if (targetAnimal != null)
        {
            Debug.Log($"找到最近的动物，距离: {nearestDistance:F2}");
        }
    }
    
    void MoveTowardsAnimal()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 targetPosition = new(targetAnimal.position.x, transform.position.y, targetAnimal.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 移动
        transform.position += moveSpeed * Time.deltaTime * direction;
        
        // 检查是否到达动物附近（距离小于2米）
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 2f)
        {
            isMovingToAnimal = false;
            EatAnimal();
            Debug.Log("Predator到达动物附近，停止移动");
        }
    }
    
    void EatAnimal()
    {
        if (targetAnimal != null)
        {
            // 使用反射获取AnimalItem组件
            Component animalComponent = null;
            try
            {
                var animalItemType = System.Type.GetType("AnimalItem");
                if (animalItemType != null)
                {
                    animalComponent = targetAnimal.GetComponent(animalItemType);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"获取动物组件失败: {e.Message}");
            }
            
            if (animalComponent != null)
            {
                Debug.Log($"Predator吃掉了动物: {targetAnimal.name}");
                Destroy(targetAnimal.gameObject);
                
                // 吃掉动物后继续寻找下一个目标
                StartCoroutine(FindNextAnimalAfterDelay());
            }
        }
    }
    
    System.Collections.IEnumerator FindNextAnimalAfterDelay()
    {
        // 等待1秒后寻找下一个动物
        yield return new WaitForSeconds(1f);
        
        FindNearestAnimal();
        if (targetAnimal != null)
        {
            isMovingToAnimal = true;
            Debug.Log($"Predator找到下一个目标: {targetAnimal.name}");
        }
        else
        {
            Debug.Log("Predator没有找到更多动物");
        }
    }
    
    void HandleKeyInput()
    {
        // 重置帧标记
        if (!Input.GetKeyDown(KeyCode.Alpha3))
            keyProcessedThisFrame = false;
        
        // 按下3键在指定位置生成黄色捕食者正方形
        if (Input.GetKeyDown(KeyCode.Alpha3) && !keyProcessedThisFrame)
        {
            keyProcessedThisFrame = true;
            SpawnPredatorAtPosition(new Vector3(-50, 15, -50));
        }
    }
    
    void SpawnPredatorAtPosition(Vector3 position)
    {
        // 创建新的Actor_Predator对象
        GameObject newPredator = new("PredatorItem");
        newPredator.transform.position = position;
        
        // 添加Actor_Predator组件
        newPredator.AddComponent<Actor_Predator>();
        
        Debug.Log($"在位置 {position} 生成了黄色捕食者正方形");
    }
    
    Mesh CreateCubeMesh()
    {
        // 使用Unity内置的立方体网格
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }

    bool IsGroundObject(GameObject obj)
    {
        return obj.CompareTag("Ground") ||
               obj.name.Contains("Ground") ||
               obj.name.Contains("Plane") ||
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround") ||
               obj.name.Contains("Terrain");
    }

    bool IsAnimalObject(GameObject obj)
    {
        // 检查对象是否有AnimalItem组件（使用字符串避免类型引用问题）
        if (obj.GetComponent("AnimalItem") != null)
        {
            return true;
        }

        // 检查对象名称是否包含Animal相关字符串
        if (obj.name.Contains("Animal") || obj.name.Contains("AnimalItem"))
        {
            return true;
        }

        // 检查标签
        if (obj.CompareTag("Animal"))
        {
            return true;
        }

        return false;
    }
}
