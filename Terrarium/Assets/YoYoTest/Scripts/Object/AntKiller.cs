using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class AntKiller : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Killer";

    public string SmallClass => "AntKiller";

    private Rigidbody thisRb;

    private bool hasTarget = false;
    private GameObject target;


    private bool isTouchTarget = false;

    public float hurtForce = 10;

    public float moveSpeed = 10;


    // Start is called before the first frame update
    void Start()
    {
        Events.OnCreateObject.Invoke(this);
        thisRb = GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {

        CheckTargetState();

        if(hasTarget)
        {
            //thisRb.velocity = (target.transform.position - transform.position).normalized * 10;
            MoveToTarget();

            if(isTouchTarget)
            {
                // 攻击目标
                target.GetComponent<IBeHurt>().BeHurt(hurtForce*Time.deltaTime);
            }

        }
        else
        {

            // thisRb.velocity = Vector3.zero;
            FindATarget();
        }
    }

    private void CheckTargetState()
    {
        if(target == null)
        {
            hasTarget = false;
        }
        else
        {
            
        }
    }

    private void FindATarget()
    {
        // 从ObjectStatisticsManager获取随机动物对象
        if (ObjectStatisticsManager.Instance != null)
        {
            GameObject randomAnimal = ObjectStatisticsManager.Instance.GetRandomAnimalObject();
            if (randomAnimal != null)
            {
                target = randomAnimal;
                hasTarget = true;
                isTouchTarget = false; // 重置碰撞状态，确保需要重新碰撞才能攻击
                Debug.Log("找到动物目标：" + target.name);
            }
            else
            {
                Debug.LogWarning("没有找到任何动物目标");
            }
        }
        else
        {
            Debug.LogError("ObjectStatisticsManager.Instance 为空");
        }
    }

    private void MoveToTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        thisRb.velocity = direction * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.attachedRigidbody.GetComponent<IGetObjectClass>().SmallClass == target.GetComponent<IGetObjectClass>().SmallClass)
        {
            isTouchTarget = true;
        }
    }

    private void OnEnable()
    {
        // Events.OnSelectPrefab.AddListener(OnSelectPrefab);
    }

    private void OnDestroy()
    {
        Events.OnDestroyObject.Invoke(this);
    }

    public void Death()
    {
        throw new System.NotImplementedException();
    }



    // test func

}
