using UnityEngine;

public class InputActionEnable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 启用所有 actions
        InputActionsManager.EnableAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        // 禁用所有 actions
        InputActionsManager.DisableAll();
    }
}
