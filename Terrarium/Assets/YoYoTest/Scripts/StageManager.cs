using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 单例实例
    private static StageManager _instance;

    // 公共访问点
    public static StageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中找到现有的StageManager
                _instance = FindObjectOfType<StageManager>();

                // 如果场景中没有，创建一个新的GameObject并添加StageManager组件
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("StageManager");
                    _instance = singletonObject.AddComponent<StageManager>();

                    // 确保在场景切换时不被销毁
                    // DontDestroyOnLoad(singletonObject); 
                }
            }
            return _instance;
        }
    }

    //总时间
    public float allTimeLimit = 100f;
    // 当前时间
    public float RemainingTime = 0f;

    //第二阶段时间
    public float secondStageTimeLimit = 10f;

    // 计时器是否正在运行
    private bool isTimerRunning = false;

    // 第二阶段独立计时器相关变量
    private float secondStageRemainingTime = 0f;
    private bool isSecondStageTimerRunning = false;

    //当前阶段
    public int stage = 0;

    // 私有字段用于跟踪上一次的阶段，避免重复触发事件
    private int previousStage = -1;

    //当前食物数量
    public int foodQuantity = 0;
    //切换阶段食物数量
    public int switchStageFoodQuantity = 40;



    // 确保只有一个实例存在
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // 如果已经存在实例，销毁这个重复的实例
            Destroy(gameObject);
        }

        Events.OnGameStart.AddListener(OnGameStart);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartTime();
        // }

        // 如果计时器正在运行，每帧减少时间
        if (isTimerRunning && RemainingTime > 0)
        {
            RemainingTime -= Time.deltaTime;

            // 如果时间用完，停止计时器
            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
                isTimerRunning = false;
                OnTimeUp();
            }
        }

        // 第二阶段独立计时器
        if (isSecondStageTimerRunning && secondStageRemainingTime > 0)
        {
            secondStageRemainingTime -= Time.deltaTime;

            // 如果第二阶段时间用完，进入阶段3
            if (secondStageRemainingTime <= 0)
            {
                secondStageRemainingTime = 0;
                isSecondStageTimerRunning = false;
                OnSecondStageTimeUp();
            }
        }

        if (stage == 1)
        {
            if (foodQuantity >= switchStageFoodQuantity)
            {
                ChangeStage(2);
                StartSecondStageTimer(); // 开始第二阶段计时
            }
        }
    }

    private void OnDestroy()
    {
        Events.OnGameStart.RemoveListener(OnGameStart);
    }


    public void OnGameStart()
    {
        Debug.Log("游戏开始");
        StartTime();
        ChangeStage(1);
    }

    /// <summary>
    /// 优雅地切换阶段并触发相应事件
    /// </summary>
    /// <param name="newStage">新的阶段编号</param>
    private void ChangeStage(int newStage)
    {
        // 如果阶段没有变化，不执行任何操作
        if (stage == newStage)
            return;

        // 更新阶段
        previousStage = stage;
        stage = newStage;

        // 触发阶段切换事件
        Events.OnGameEnterStage.Invoke(newStage);

        Debug.Log($"阶段切换：从阶段{previousStage}切换到阶段{newStage}");
    }


    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartTime()
    {
        // 设置剩余时间为总时间
        RemainingTime = allTimeLimit;
        // 开始计时器
        isTimerRunning = true;

        Debug.Log("计时开始！剩余时间：" + RemainingTime + "秒");
    }

    /// <summary>
    /// 时间用完时调用
    /// </summary>
    private void OnTimeUp()
    {
        Debug.Log("时间到！");
        // 在这里可以添加时间用完后的逻辑
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("计时器已停止");
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        isTimerRunning = false;
        RemainingTime = 0f;
        Debug.Log("计时器已重置");
    }

    /// <summary>
    /// 开始第二阶段独立计时器
    /// </summary>
    private void StartSecondStageTimer()
    {
        secondStageRemainingTime = secondStageTimeLimit;
        isSecondStageTimerRunning = true;
        Debug.Log("第二阶段计时开始！时间：" + secondStageTimeLimit + "秒");
    }

    /// <summary>
    /// 第二阶段时间用完时调用
    /// </summary>
    private void OnSecondStageTimeUp()
    {
        ChangeStage(3);
        Debug.Log("第二阶段时间到！进入阶段3");
        // 在这里可以添加进入阶段3后的逻辑
    }
}
