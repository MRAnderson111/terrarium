using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    public GameObject selectPrefab;
    private Button button;
    private GameObject selectPosition;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        selectPosition =  Resources.Load<GameObject>("Prefabs/SelectPosition");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Debug.Log("选择预制体：" + selectPrefab.name);
        Events.OnSelectPrefab.Invoke(selectPrefab);
    }
}
