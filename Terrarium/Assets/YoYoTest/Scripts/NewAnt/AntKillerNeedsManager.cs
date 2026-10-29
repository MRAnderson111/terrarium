using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntKillerNeedsManager : MonoBehaviour, INewAntNeeds
{
    [Header("环境偏好（0-1）")]
    [SerializeField, Range(0f, 1f)]
    private float preferredTemperature = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float preferredHumidity = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float preferredSunlight = 0.5f;

    public float PreferredTemperature => preferredTemperature;
    public float PreferredHumidity => preferredHumidity;
    public float PreferredSunlight => preferredSunlight;

    [Header("食物偏好")]
    [Tooltip("最喜欢的猎物 SmallClass。场景中存在时优先捕食，留空则随机选择动物。")]
    [SerializeField]
    private string preferredAnimalSmallClass;

    public string PreferredAnimalSmallClass => preferredAnimalSmallClass;

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
    
    // 食物消耗速度（饱腹感增加速度）
    public float foodConsumptionRate = 30f;

    [SerializeField]
    private float interactionRange = 0.75f;

    private EnvironmentPreferenceDriver preferenceDriver;

    private void Start()
    {
        ant = GetComponent<INewAnt>();
        // 获取导航移动组件
        navMove = GetComponent<AnimalNavMove>();
        preferenceDriver = new EnvironmentPreferenceDriver(this, true, true, true);

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

    private void Update()
    {
        preferenceDriver?.Tick(preferredTemperature, preferredHumidity, preferredSunlight);
        RefreshDrinkState();
    }

    private void OnDestroy()
    {
        preferenceDriver?.Dispose();
    }

    private float GetRequiredWaterAmount()
    {
        EnvironmentalData data = EnvironmentPreferenceUtility.GetCurrentEnvironment();
        return EnvironmentPreferenceUtility.GetWaterNeed(data.sunshine, data.temperature);
    }

    private void RefreshDrinkState()
    {
        isDrinkWater = currentWaterSatisfaction >= GetRequiredWaterAmount();
    }

    private float GetGrowthRateMultiplier()
    {
        return preferenceDriver != null ? preferenceDriver.StatMultiplier : 1f;
    }

    /// <summary>
    /// 当蚂蚁碰到水目标时停止移动
    /// </summary>
    public void OnTouchWaterTarget()
    {
        if (navMove != null)
        {
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
            navMove.StopMoving();
        }
    }

    // 更新饮水满足度
    public void UpdateWaterSatisfaction()
    {
        if (isTouchWaterTarget && waterTarget != null)
        {
            float requiredWater = GetRequiredWaterAmount();
            currentWaterSatisfaction += 30 * Time.deltaTime;
            if (currentWaterSatisfaction >= requiredWater)
            {
                currentWaterSatisfaction = requiredWater;
                isDrinkWater = true;
                navMove?.StopEatingAnimation();
            }
        }
    }

    // 更新饱腹感
    public void UpdateFullness()
    {
        if (isTouchFoodTarget && foodTarget != null)
        {
            currentFullness += 30 * Time.deltaTime * GetGrowthRateMultiplier();
            if (currentFullness >= 100)
            {
                currentFullness = 100;
                isFull = true;
                navMove?.StopEatingAnimation();
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
            waterTarget = waterObject;
            isTouchWaterTarget = false;
        }
        else
        {
            waterTarget = null;
            isDrinkWater = false;
        }
    }

    // 寻找食物目标
    public void FindFoodTarget()
    {
        if (ObjectStatisticsManager.Instance == null)
        {
            Debug.LogError("ObjectStatisticsManager.Instance 为空");
            foodTarget = null;
            isHaveFoodTarget = false;
            return;
        }

        // 正在攻击当前猎物时不改目标；离开或目标消失后，再优先选择喜好动物。
        if (isTouchFoodTarget && foodTarget != null)
        {
            return;
        }

        if (foodTarget != null && (string.IsNullOrEmpty(preferredAnimalSmallClass) || IsCurrentPreferredFood(preferredAnimalSmallClass)))
        {
            return;
        }

        GameObject selectedAnimal = ObjectStatisticsManager.Instance.GetPriorityTarget(
            "Animal",
            preferredAnimalSmallClass,
            transform.position,
            gameObject);

        if (selectedAnimal == null)
        {
            foodTarget = null;
            isHaveFoodTarget = false;
            return;
        }

        if (foodTarget != selectedAnimal)
        {
            foodTarget = selectedAnimal;
            isTouchFoodTarget = false;
        }

        isHaveFoodTarget = true;
    }

    private bool IsCurrentPreferredFood(string preferredSmallClass)
    {
        if (foodTarget == null)
        {
            return false;
        }

        IGetObjectClass objectClass = foodTarget.GetComponent<IGetObjectClass>();
        return objectClass != null && objectClass.SmallClass == preferredSmallClass;
    }

    // 移动到水目标
    public void GoToWaterTarget()
    {
        if (waterTarget != null)
        {
            // 使用导航移动组件移动到水目标
            navMove.SetTarget(waterTarget.transform.position);
        }
    }

    // 移动到食物目标
    public void GoToFoodTarget()
    {
        if (foodTarget != null)
        {
            // 使用导航移动组件移动到食物目标
            navMove.SetTarget(foodTarget.transform.position);
        }
    }

    // 喝水逻辑
    public void DrinkWater()
    {
        if (waterTarget != null)
        {
            if (isTouchWaterTarget || IsWithinInteractionRange(waterTarget))
            {
                isTouchWaterTarget = true;
                OnTouchWaterTarget();
                if (navMove != null)
                    navMove.PlayEatingAnimation();
                UpdateWaterSatisfaction();
            }
            else
            {
                GoToWaterTarget();
            }
        }
        else
        {
            FindWaterTarget();
        }
    }

    // 吃食物逻辑
    public void EatFood()
    {
        FindFoodTarget();

        if (foodTarget != null)
        {
            isTouchFoodTarget = IsWithinInteractionRange(foodTarget);
            if (isTouchFoodTarget)
            {
                OnTouchFoodTarget(); // 触发停止移动
                
                // 播放进食动画
                if (navMove != null)
                {
                    navMove.PlayEatingAnimation();
                }
                
                // 检查植物是否有IBeHurt组件
                IBeHurt beHurtComponent = foodTarget.GetComponent<IBeHurt>();
                if (beHurtComponent != null)
                {
                    // 扣猎物的血，并让它朝远离捕食者的方向逃跑
                    beHurtComponent.BeHurt(hurtForce * Time.deltaTime);
                    NewAntTest prey = foodTarget.GetComponent<NewAntTest>();
                    if (prey != null)
                    {
                        prey.FleeFrom(transform.position);
                    }
                    // 增加饱腹感（相当于营养度）
                    currentFullness += foodConsumptionRate * Time.deltaTime * GetGrowthRateMultiplier();
                    if (currentFullness >= 100)
                    {
                        currentFullness = 100;
                        isFull = true;
                navMove?.StopEatingAnimation();
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
                GoToFoodTarget();
            }
        }
        else
        {
            FindFoodTarget();
        }
    }

    private bool IsWithinInteractionRange(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        Collider[] colliders = target.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider targetCollider = colliders[i];
            if (targetCollider == null || !targetCollider.enabled)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, targetCollider.ClosestPoint(transform.position));
            if (distance <= interactionRange)
            {
                return true;
            }
        }

        return Vector3.Distance(transform.position, target.transform.position) <= interactionRange;
    }

    // 重置状态
    public void ResetStates()
    {
        navMove?.StopEatingAnimation();
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