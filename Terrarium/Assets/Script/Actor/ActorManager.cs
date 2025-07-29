using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance { get; private set; }
    public static int PlantIndex;

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
    }

    public static void OnSecondSquareClicked()
    {
        PlantIndex = 2;
        Debug.Log("PlantIndex: " + PlantIndex);
    }

    public static void OnThirdSquareClicked()
    {
        PlantIndex = 3;
        Debug.Log("PlantIndex: " + PlantIndex);
    }

    public static void OnFourthSquareClicked()
    {
        PlantIndex = 4;
        Debug.Log("PlantIndex: " + PlantIndex);
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
