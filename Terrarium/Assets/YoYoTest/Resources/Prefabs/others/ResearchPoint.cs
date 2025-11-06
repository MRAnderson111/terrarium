using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    public GameObject fxPrefab; // 特效预制体
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 当鼠标点击到该对象时调用
    private void OnMouseDown()
    {
        // 生成特效
        if (fxPrefab != null)
        {
            Instantiate(fxPrefab, transform.position, transform.rotation);
        }
        
        // 销毁自身
        Destroy(gameObject);
    }
}
