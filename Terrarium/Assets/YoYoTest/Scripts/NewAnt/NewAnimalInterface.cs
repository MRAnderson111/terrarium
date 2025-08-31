using System;
using UnityEngine;

/// <summary>
/// 蚂蚁本身对外提供的接口
/// </summary>
public interface INewAnt
{
    // 设置是否为成虫状态
    void SetAdult(bool isAdult);
    
    // 获取移动速度
    float MoveSpeed { get; }
    
    // 设置是否在居住地内
    void SetInHome(bool inHome);
    
    // 设置是否在睡觉
    void SetSleeping(bool sleeping);
    
    // 获取蚂蚁的GameObject
    GameObject GetGameObject();
}

/// <summary>
/// 蚂蚁的需求管理器对外提供的接口
/// </summary>
public interface INewAntNeeds
{
    // 是否吃饱
    bool IsFull { get; }
    
    // 是否喝过水
    bool IsDrinkWater { get; }
    
    // 获取食物目标
    GameObject FoodTarget { get; }

    // 标记是否接触到水目标
    bool IsTouchWaterTarget { get; set; }
    
    // 标记是否接触到食物目标
    bool IsTouchFoodTarget { get; set; }
    
    // 执行喝水逻辑
    void DrinkWater();
    
    // 执行吃食物逻辑
    void EatFood();
    
    // 重置状态
    void ResetStates();
}

/// <summary>
/// 蚂蚁居住地管理器对外提供的接口
/// </summary>
public interface INewAntHome
{
    // 是否有居住地
    bool IsHaveHome { get; }
    
    // 强制创建居住地
    void ForceCreateHome();
    
    // 回家并停留
    void GoHomeAndStay(INewAnt ant, float moveSpeed);
    
    // 回家并睡觉
    void GoHomeAndSleep(INewAnt ant, float moveSpeed);
    
    // 离开居住地
    void LeaveHome(INewAnt ant, Action<bool> onAntLeft);
}