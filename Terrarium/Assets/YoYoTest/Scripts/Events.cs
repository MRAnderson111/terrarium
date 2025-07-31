using UnityEngine;
using UnityEngine.Events;

public static class Events
{
    // 游戏开始事件
    public static UnityEvent OnGameStart = new();

    // 游戏进入阶段事件 (参数: 阶段编号 1-第一阶段, 2-第二阶段, 3-第三阶段)
    public static UnityEvent<int> OnGameEnterStage = new();

    // 选择预制体事件 (参数: 预制体)
    public static UnityEvent<GameObject> OnSelectPrefab = new();

    // 物种生成,销毁事件 (参数: 物种信息接口)
    public static UnityEvent<IGetObjectClass> OnCreateObject = new();
    public static UnityEvent<IGetObjectClass> OnDestroyObject = new();



    // 游戏结束事件
    public static UnityEvent OnGameEnd = new();

    // 游戏暂停事件
    public static UnityEvent OnGamePause = new();

    // 游戏恢复事件
    public static UnityEvent OnGameResume = new();

    // 玩家死亡事件
    public static UnityEvent OnPlayerDeath = new();

    // 玩家重生事件
    public static UnityEvent OnPlayerRespawn = new();

    // 关卡完成事件
    public static UnityEvent OnLevelComplete = new();

    // 关卡失败事件
    public static UnityEvent OnLevelFailed = new();

    // 分数更新事件 (带参数)
    public static UnityEvent<int> OnScoreUpdate = new();

    // 生命值变化事件 (带参数)
    public static UnityEvent<float> OnHealthChanged = new();

    // 物品收集事件 (带参数)
    public static UnityEvent<string> OnItemCollected = new();

    // 清理所有事件监听器的方法
    public static void ClearAllListeners()
    {
        OnGameStart.RemoveAllListeners();
        OnGameEnterStage.RemoveAllListeners();
        OnGameEnd.RemoveAllListeners();
        OnGamePause.RemoveAllListeners();
        OnGameResume.RemoveAllListeners();
        OnPlayerDeath.RemoveAllListeners();
        OnPlayerRespawn.RemoveAllListeners();
        OnLevelComplete.RemoveAllListeners();
        OnLevelFailed.RemoveAllListeners();
        OnScoreUpdate.RemoveAllListeners();
        OnHealthChanged.RemoveAllListeners();
        OnItemCollected.RemoveAllListeners();
    }
}
