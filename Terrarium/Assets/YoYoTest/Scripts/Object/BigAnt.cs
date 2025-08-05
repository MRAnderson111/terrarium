using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BigAnt : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Animal";

    public string SmallClass => "BigAnt";

    private Rigidbody thisRb;

    private bool hasTarget = false;
    private GameObject target;


    private bool isTouchTarget = false;

    public float hurtForce = 10;

    public float moveSpeed = 10;

    //营养度
    public float nutrition = 0;


    public string prefabPath = "Prefabs/BigAnt";


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
        CheckNutrition();

        if(hasTarget)
        {
            // 检查目标是否仍然有效
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                hasTarget = false;
                isTouchTarget = false;
                target = null;
                return;
            }

            MoveToTarget();

            if(isTouchTarget)
            {
                // 再次确认目标仍然有效且有IBeHurt组件
                IBeHurt beHurtComponent = target.GetComponent<IBeHurt>();
                if (beHurtComponent != null)
                {
                    // 攻击目标
                    beHurtComponent.BeHurt(hurtForce*Time.deltaTime);
                    nutrition += 10*Time.deltaTime;
                    // Debug.Log("蚂蚁正在攻击植物：" + target.name + "，营养度：" + nutrition);
                }
                else
                {
                    // 如果目标没有IBeHurt组件，重置状态
                    isTouchTarget = false;
                    // Debug.LogWarning("目标植物没有IBeHurt组件：" + target.name);
                }
            }
        }
        else
        {

            // thisRb.velocity = Vector3.zero;
            FindATarget();
        }
    }

    private void CheckNutrition()
    {
        if(nutrition >= 100)
        {
            Reproduction();
            nutrition = 0;
            
            
        }
    }

    private void Reproduction()
    {
        Instantiate(Resources.Load<GameObject>(prefabPath), transform.position, Quaternion.identity);
    }

    private void CheckTargetState()
    {
        // 检查目标是否仍然有效
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            hasTarget = false;
            isTouchTarget = false;
            target = null;
        }
    }

    private void FindATarget()
    {
        // 从ObjectStatisticsManager获取随机植物对象
        if (ObjectStatisticsManager.Instance != null)
        {
            GameObject randomPlant = ObjectStatisticsManager.Instance.GetRandomPlantObject();
            if (randomPlant != null)
            {
                target = randomPlant;
                hasTarget = true;
                isTouchTarget = false; // 重置碰撞状态，确保需要重新碰撞才能攻击
                Debug.Log("找到植物目标：" + target.name);
            }
            else
            {
                Debug.LogWarning("没有找到任何植物目标");
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
        // 检查是否有目标且目标仍然有效
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            hasTarget = false;
            isTouchTarget = false;
            return;
        }

        // 检查碰撞对象是否有刚体和IGetObjectClass组件
        if (collision.collider.attachedRigidbody == null)
            return;

        IGetObjectClass collisionObject = collision.collider.attachedRigidbody.GetComponent<IGetObjectClass>();
        if (collisionObject == null)
            return;

        // 精确检查：碰撞对象是否就是当前目标对象
        if (collision.gameObject == target.gameObject)
        {
            isTouchTarget = true;
            Debug.Log("蚂蚁碰撞到目标植物：" + target.name);
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
        Destroy(gameObject);
    }



    // test func

}
