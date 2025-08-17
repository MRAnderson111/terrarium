using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntNeedsManager : MonoBehaviour
{
    // 水相关属性
    public GameObject waterTarget;
    public float currentWaterSatisfaction = 0f;  // 饮水满足度，0-100
    public bool isTouchWaterTarget = false;
    public bool isDrinkWater = false;
    
    // 食物相关属性
    public GameObject foodTarget;
    public bool isTouchFoodTarget = false;
    public bool isHaveFoodTarget = false;
    public float currentFullness = 0f;  // 饱腹感，0-100
    public bool isFull = false;
    
    // 移动速度
    private float moveSpeed = 2f;
    
    // 引用主蚂蚁对象
    private NewAntTest ant;
    
    private void Start()
    {
        // 获取主蚂蚁组件引用
        ant = GetComponent<NewAntTest>();
    }
    
    // 更新饮水满足度
    public void UpdateWaterSatisfaction()
    {
        if (isTouchWaterTarget && waterTarget != null)
        {
            currentWaterSatisfaction += 30 * Time.deltaTime;
            if (currentWaterSatisfaction >= 100)
            {
                currentWaterSatisfaction = 100;
                isDrinkWater = true;
                Debug.Log("蚂蚁喝饱了水");
            }
        }
    }
    
    // 更新饱腹感
    public void UpdateFullness()
    {
        if (isTouchFoodTarget && foodTarget != null)
        {
            currentFullness += 30 * Time.deltaTime;
            if (currentFullness >= 100)
            {
                currentFullness = 100;
                isFull = true;
                Debug.Log("蚂蚁吃饱了");
                //变成成虫
                ant.isAdult = true;
            }
        }
    }
    
    // 寻找水目标
    public void FindWaterTarget()
    {
        // 在场景中寻找名为 "water" 的对象
        GameObject waterObject = GameObject.Find("water");

        if (waterObject != null)
        {
            Debug.Log("找到水目标: " + waterObject.name);
            waterTarget = waterObject;
            isTouchWaterTarget = false;
        }
        else
        {
            Debug.Log("未找到水目标");
            waterTarget = null;
            isDrinkWater = false;
        }
    }
    
    // 寻找食物目标
    public void FindFoodTarget()
    {
        // 在场景中寻找名为 "food" 的对象
        GameObject foodObject = GameObject.Find("food");

        if (foodObject != null)
        {
            Debug.Log("找到食物目标: " + foodObject.name);
            foodTarget = foodObject;
            isHaveFoodTarget = true;
            isTouchFoodTarget = false;
        }
        else
        {
            Debug.Log("未找到食物目标");
            foodTarget = null;
            isHaveFoodTarget = false;
        }
    }
    
    // 移动到水目标
    public void GoToWaterTarget()
    {
        if (waterTarget != null)
        {
            // 计算从当前位置到水目标的方向
            Vector3 direction = waterTarget.transform.position - transform.position;
            
            // 归一化方向向量，确保移动速度一致
            direction.Normalize();
            
            // 向水目标方向移动
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            
            Debug.Log("正在向水目标移动");
        }
        else
        {
            Debug.Log("水目标不存在");
        }
    }
    
    // 移动到食物目标
    public void GoToFoodTarget()
    {
        if (foodTarget != null)
        {
            // 计算从当前位置到食物目标的方向
            Vector3 direction = foodTarget.transform.position - transform.position;
            
            // 归一化方向向量，确保移动速度一致
            direction.Normalize();
            
            // 向食物目标方向移动
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            
            Debug.Log("正在向食物目标移动");
        }
        else
        {
            Debug.Log("食物目标不存在");
        }
    }
    
    // 喝水逻辑
    public void DrinkWater()
    {
        if (waterTarget != null)
        {
            Debug.Log("有水目标，去喝水");
            if (isTouchWaterTarget)
            {
                Debug.Log("碰到水目标，喝水");
                UpdateWaterSatisfaction();
            }
            else
            {
                Debug.Log("没碰到水目标，去喝水");
                GoToWaterTarget();
            }
        }
        else
        {
            Debug.Log("没有水目标，去寻找");
            FindWaterTarget();
        }
    }
    
    // 吃食物逻辑
    public void EatFood()
    {
        if (foodTarget != null)
        {
            Debug.Log("有食物目标，去吃");
            if (isTouchFoodTarget)
            {
                Debug.Log("碰到食物目标，吃食物");
                UpdateFullness();
            }
            else
            {
                Debug.Log("没碰到食物目标，去吃");
                GoToFoodTarget();
            }
        }
        else
        {
            Debug.Log("没有食物目标，去寻找");
            FindFoodTarget();
        }
    }
    
    // 重置状态
    public void ResetStates()
    {
        currentFullness = 0f;
        isFull = false;
        currentWaterSatisfaction = 0f;
        isDrinkWater = false;
    }
}