using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickButton : MonoBehaviour
{
    public GameObject selectPrefab;
    private PickPrefabUI PickPrefabUI;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        PickPrefabUI = FindObjectOfType<PickPrefabUI>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Debug.Log("选择预制体：" + selectPrefab.name);
        PickPrefabUI.Instance.AddPrefabToClass(selectPrefab);
    }
}
