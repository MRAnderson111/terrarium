using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    public GameObject selectPrefab;
    public GameObject selectPosition; // 用于显示鼠标射线指向位置的GameObject


    [Header("预制体")]
    public bool isHit;
    // Start is called before the first frame update
    void Start()
    {
        Events.OnSelectPrefab.AddListener(OnSelectPrefab);
        
    }

    // Update is called once per frame
    void Update()
    {
        // 实时检测鼠标射线并更新selectPosition位置
        UpdateSelectPosition();
    }

    /// <summary>
    /// 更新选择位置，根据鼠标射线检测结果
    /// </summary>
    private void UpdateSelectPosition()
    {
        if (selectPosition == null) return;

        // 从摄像机发射射线到鼠标位置
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 检测射线是否击中物体
        if (Physics.Raycast(ray, out hit))
        {
            // 将selectPosition移动到射线击中的位置
            selectPosition.transform.position = hit.point;
            isHit = true;
        }
        else
        {
            // 如果没有击中任何物体，可以将位置设置到射线上的某个固定距离
            Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离摄像机10单位
            selectPosition.transform.position = targetPosition;
            isHit = false;
        }
    }

    public void OnSelectPrefab(GameObject prefab)
    {
        selectPrefab = prefab;
        Debug.Log("选择预制体：" + selectPrefab.name);
    }

    private void OnDestroy()
    {
        Events.OnSelectPrefab.RemoveListener(OnSelectPrefab);
    }
}
