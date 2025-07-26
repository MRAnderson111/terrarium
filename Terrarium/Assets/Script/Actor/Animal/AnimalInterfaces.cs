using UnityEngine;

/// <summary>
/// 动物系统接口定义文件
/// </summary>

// 动物行为接口
public interface IAnimalBehavior
{
    void UpdateBehavior();
    void StopBehavior();
}

// 动物移动接口
public interface IAnimalMovement
{
    void MoveTo(Vector3 target);
    void StopMovement();
    bool IsMoving { get; }
    float MoveSpeed { get; set; }
}

// 动物生存需求接口
public interface IAnimalNeeds
{
    bool IsHungry { get; }
    bool IsThirsty { get; }
    void Eat(Transform food);
    void Drink(Transform water);
    void UpdateNeeds();
}

// 动物繁衍接口
public interface IAnimalReproduction
{
    bool CanReproduce { get; }
    void TryFindMate();
    void AttemptReproduction(Transform mate);
    float ReproductionCooldown { get; }
}

// 动物环境适应接口
public interface IAnimalEnvironment
{
    float EnvironmentalStress { get; }
    bool IsInOptimalEnvironment { get; }
    void CheckEnvironmentalConditions();
}

// 动物生命周期接口
public interface IAnimalLifecycle
{
    float LifeTimer { get; }
    float MaxLifeTime { get; }
    bool IsDying { get; }
    void Die();
}

// 动物视觉接口
public interface IAnimalVisual
{
    void ChangeColor(Color color);
    void RestoreNormalColor();
    void UpdateVisualState();
}
