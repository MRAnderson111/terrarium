using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    public GameObject selectPrefab;
    public GameObject hitPrefab; // 用于显示鼠标射线指向位置的GameObject
    public GameObject selectPosition; // 用于显示鼠标射线指向位置的GameObject


    [Header("预制体")]
    public bool isHit;
    // Start is called before the first frame update
    void Start()
    {
        Events.OnSelectPrefab.AddListener(OnSelectPrefab);
        selectPosition = Instantiate(hitPrefab);
        selectPosition.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 实时检测鼠标射线并更新selectPosition位置
        UpdateSelectPosition();

        // 检测鼠标点击
        HandleMouseClick();
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
            // 当射线击中物体时，激活selectPosition
            selectPosition.SetActive(true);
        }
        else
        {
            // 如果没有击中任何物体，可以将位置设置到射线上的某个固定距离
            Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离摄像机10单位
            selectPosition.transform.position = targetPosition;
            isHit = false;
            // 当射线没有击中物体时，也激活selectPosition
            selectPosition.SetActive(false);
        }
    }

    /// <summary>
    /// 处理鼠标点击，在射线击中位置生成预制体
    /// </summary>
    private void HandleMouseClick()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 只有当射线击中物体且有选择的预制体时才生成
            if (isHit && selectPrefab != null)
            {
                // 在selectPosition的位置生成预制体
                GameObject newObject = Instantiate(selectPrefab, selectPosition.transform.position, Quaternion.identity);
                Debug.Log("在位置 " + selectPosition.transform.position + " 生成了预制体：" + selectPrefab.name);
            }
            else if (!isHit)
            {
                Debug.Log("射线未击中任何物体，无法生成预制体");
            }
            else if (selectPrefab == null)
            {
                Debug.Log("未选择预制体，无法生成");
            }
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
