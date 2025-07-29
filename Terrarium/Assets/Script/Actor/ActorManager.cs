using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance { get; private set; }
    public static int PlantIndex;
    
    // 定义事件
    public static System.Action<int> OnPlantIndexChanged;

    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void OnFirstSquareClicked()
    {
        PlantIndex = 1;
        Debug.Log("PlantIndex: " + PlantIndex);
        // 触发事件
        OnPlantIndexChanged?.Invoke(PlantIndex);
    }

    public static void OnSecondSquareClicked()
    {
        PlantIndex = 2;
        Debug.Log("PlantIndex: " + PlantIndex);
        OnPlantIndexChanged?.Invoke(PlantIndex);
    }

    public static void OnThirdSquareClicked()
    {
        PlantIndex = 3;
        Debug.Log("PlantIndex: " + PlantIndex);
        OnPlantIndexChanged?.Invoke(PlantIndex);
    }

    public static void OnFourthSquareClicked()
    {
        PlantIndex = 4;
        Debug.Log("PlantIndex: " + PlantIndex);
        OnPlantIndexChanged?.Invoke(PlantIndex);
    }

    public static void ResetValue()
    {
        if (PlantIndex != 0)
        {
            PlantIndex = 0;
            OnPlantIndexChanged?.Invoke(PlantIndex);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
