using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAntTest : MonoBehaviour
{
    //是否是“成虫”
    public bool isAdult;
    //是否是“白天”
    public bool isDay;
    //是否“吃饱”
    public bool isFull;
    //是否“有居住地”
    public bool isHaveHome;
    //是否“完成繁殖”
    public bool isFinishReproduction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAdult)
        {
            if (isDay)
            {
                if (isFull)
                {
                    Debug.Log("成虫白天吃饱了");
                    if (isHaveHome)
                    {
                        if(isFinishReproduction)
                        {
                            Debug.Log("成虫白天吃饱了，有居住地，完成繁殖");
                            TakeAWalk();
                        }
                        else
                        {
                            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖");
                            GoHomeAndStay();
                        }
                        // Debug.Log("成虫白天吃饱了，有居住地");
                        // GoHomeAndStay();
                    }
                    else
                    {
                        Debug.Log("成虫白天吃饱了，没有居住地");
                        CreateHome();
                    }

                }
                else
                {
                    FindAndEat();
                }
            }
            else
            {
                Debug.Log("成虫夜晚");
            }
        }
        else
        {
            FindAndEat();
        }
    }

    private void TakeAWalk()
    {
        Debug.Log("成虫白天吃饱了，有居住地，完成繁殖，去散步");
    }

    private void CreateHome()
    {
        Debug.Log("成虫白天吃饱了，有居住地，完成繁殖，去散步");
    }

    private void GoHomeAndStay()
    {
        Debug.Log("成虫白天吃饱了，有居住地，完成繁殖，回家");
    }

    private void FindAndEat()
    {
        if (isFull)
        {
            Debug.Log("吃饱了");
        }
        else
        {
            Debug.Log("没吃饱");
        }
    }
}
