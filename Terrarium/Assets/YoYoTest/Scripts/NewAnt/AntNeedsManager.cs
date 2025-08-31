using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntNeedsManager : MonoBehaviour, INewAntNeeds
{
    // 水相关属性
    public GameObject waterTarget;
    public float currentWaterSatisfaction = 0f;  // 饮水满足度，0-100
    public bool isTouchWaterTarget = false;
    public bool IsTouchWaterTarget { get => isTouchWaterTarget; set => isTouchWaterTarget = value; }
    public bool isDrinkWater = false;
    public bool IsDrinkWater => isDrinkWater;

    // 食物相关属性
    public GameObject foodTarget;
    public GameObject FoodTarget => foodTarget;
    public bool isTouchFoodTarget = false;
    public bool IsTouchFoodTarget { get => isTouchFoodTarget; set => isTouchFoodTarget = value; }
    public bool isHaveFoodTarget = false;
    public float currentFullness = 0f;  // 饱腹感，0-100
    public bool isFull = false;
    public bool IsFull => isFull;

    // 移动速度（从蚂蚁类获取）
    private float moveSpeed;

    // 引用主蚂蚁对象
    private INewAnt ant;

    // 引用导航移动组件
    private AnimalNavMove navMove;

    // 伤害力（用于扣植物的血）
    public float hurtForce = 10;

    private void Start()
    {
        ant = GetComponent<INewAnt>();
        // 获取导航移动组件
        navMove = GetComponent<AnimalNavMove>();



        // 从蚂蚁类获取移动速度
        if (ant != null)
        {
            moveSpeed = ant.MoveSpeed;
        }
        else
        {
            Debug.LogError("无法获取蚂蚁组件，使用默认移动速度 2f");
            moveSpeed = 2f;
        }
    }

    /// <summary>
    /// 当蚂蚁碰到水目标时停止移动
    /// </summary>
    public void OnTouchWaterTarget()
    {
        if (navMove != null)
        {
            Debug.Log("碰到水目标，停止移动");
            navMove.StopMoving();
        }
    }

    /// <summary>
    /// 当蚂蚁碰到食物目标时停止移动
    /// </summary>
    public void OnTouchFoodTarget()
    {
        if (navMove != null)
        {
            Debug.Log("碰到食物目标，停止移动");
            navMove.StopMoving();
        }
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
                ant.SetAdult(true);
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
        // 从ObjectStatisticsManager获取随机植物对象
        if (ObjectStatisticsManager.Instance != null)
        {
            GameObject randomPlant = ObjectStatisticsManager.Instance.GetRandomPlantObject();
            if (randomPlant != null)
            {
                foodTarget = randomPlant;
                isHaveFoodTarget = true;
                isTouchFoodTarget = false;
                Debug.Log("找到植物目标: " + foodTarget.name);
            }
            else
            {
                Debug.LogWarning("未找到植物目标");
                foodTarget = null;
                isHaveFoodTarget = false;
            }
        }
        else
        {
            Debug.LogError("ObjectStatisticsManager.Instance 为空");
            foodTarget = null;
            isHaveFoodTarget = false;
        }
    }

    // 移动到水目标
    public void GoToWaterTarget()
    {
        if (waterTarget != null)
        {
            // 使用导航移动组件移动到水目标
            navMove.SetTarget(waterTarget.transform.position);
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
            // 使用导航移动组件移动到食物目标
            navMove.SetTarget(foodTarget.transform.position);
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
                OnTouchWaterTarget(); // 触发停止移动
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
            Debug.Log("有植物目标，去吃");
            if (isTouchFoodTarget)
            {
                Debug.Log("碰到植物目标，吃植物");
                OnTouchFoodTarget(); // 触发停止移动
                // 检查植物是否有IBeHurt组件
                IBeHurt beHurtComponent = foodTarget.GetComponent<IBeHurt>();
                if (beHurtComponent != null)
                {
                    // 扣植物的血
                    beHurtComponent.BeHurt(hurtForce * Time.deltaTime);
                    // 增加饱腹感（相当于营养度）
                    currentFullness += 10 * Time.deltaTime;
                    if (currentFullness >= 100)
                    {
                        currentFullness = 100;
                        isFull = true;
                        Debug.Log("蚂蚁吃饱了");
                        //变成成虫
                        ant.SetAdult(true);
                    }
                }
                else
                {
                    Debug.LogWarning("植物目标没有IBeHurt组件：" + foodTarget.name);
                }
            }
            else
            {
                Debug.Log("没碰到植物目标，去吃");
                GoToFoodTarget();
            }
        }
        else
        {
            Debug.Log("没有植物目标，去寻找");
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

        // 重置接触目标的标志位
        isTouchWaterTarget = false;
        isTouchFoodTarget = false;

        // 重置目标对象，强制蚂蚁在新的一天重新寻找
        waterTarget = null;
        foodTarget = null;
        isHaveFoodTarget = false;
    }
}