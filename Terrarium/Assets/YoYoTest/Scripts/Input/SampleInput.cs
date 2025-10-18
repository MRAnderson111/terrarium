using UnityEngine;

public class SampleInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // // 启用所有输入动作
        // InputActionsManager.EnableAll();
    }

    // Update is called once per frame
    void Update()
    {
        // 左手摇杆输入（Vector2输出）
        Vector2 leftThumbstick = InputActionsManager.Actions.XRILeftLocomotion.Move.ReadValue<Vector2>();
        if (leftThumbstick.magnitude > 0.1f)
        {
            Debug.Log("左摇杆输入: " + leftThumbstick);
        }

        // 右手摇杆输入（Vector2输出）
        Vector2 rightThumbstick = InputActionsManager.Actions.XRIRightLocomotion.Move.ReadValue<Vector2>();
        if (rightThumbstick.magnitude > 0.1f)
        {
            Debug.Log("右摇杆输入: " + rightThumbstick);
        }

        // 左手柄A键（PrimaryButton）
        if (InputActionsManager.Actions.XRILeftInteraction.PrimaryButton.WasPressedThisFrame())
        {
            Debug.Log("左手柄A键按下");
        }
        if (InputActionsManager.Actions.XRILeftInteraction.PrimaryButton.WasReleasedThisFrame())
        {
            Debug.Log("左手柄A键释放");
        }

        // 左手柄B键（SecondaryButton）
        if (InputActionsManager.Actions.XRILeftInteraction.SecondaryButton.WasPressedThisFrame())
        {
            Debug.Log("左手柄B键按下");
        }
        if (InputActionsManager.Actions.XRILeftInteraction.SecondaryButton.WasReleasedThisFrame())
        {
            Debug.Log("左手柄B键释放");
        }

        // 右手柄A键（PrimaryButton）
        if (InputActionsManager.Actions.XRIRightInteraction.PrimaryButton.WasPressedThisFrame())
        {
            Debug.Log("右手柄A键按下");
        }
        if (InputActionsManager.Actions.XRIRightInteraction.PrimaryButton.WasReleasedThisFrame())
        {
            Debug.Log("右手柄A键释放");
        }

        // 右手柄B键（SecondaryButton）
        if (InputActionsManager.Actions.XRIRightInteraction.SecondaryButton.WasPressedThisFrame())
        {
            Debug.Log("右手柄B键按下");
        }
        if (InputActionsManager.Actions.XRIRightInteraction.SecondaryButton.WasReleasedThisFrame())
        {
            Debug.Log("右手柄B键释放");
        }

        // 左手柄扳机键（Activate）
        float leftTrigger = InputActionsManager.Actions.XRILeftInteraction.ActivateValue.ReadValue<float>();
        if (leftTrigger > 0.1f)
        {
            Debug.Log("左手柄扳机键: " + leftTrigger);
        }

        // 右手柄扳机键（Activate）
        float rightTrigger = InputActionsManager.Actions.XRIRightInteraction.ActivateValue.ReadValue<float>();
        if (rightTrigger > 0.1f)
        {
            Debug.Log("右手柄扳机键: " + rightTrigger);
        }

        // 左手柄侧握键（Grip）
        float leftGrip = InputActionsManager.Actions.XRILeftInteraction.SelectValue.ReadValue<float>();
        if (leftGrip > 0.1f)
        {
            Debug.Log("左手柄侧握键: " + leftGrip);
        }

        // 右手柄侧握键（Grip）
        float rightGrip = InputActionsManager.Actions.XRIRightInteraction.SelectValue.ReadValue<float>();
        if (rightGrip > 0.1f)
        {
            Debug.Log("右手柄侧握键: " + rightGrip);
        }

        // 左手摇杆按下（ScaleToggle）
        if (InputActionsManager.Actions.XRILeftInteraction.ScaleToggle.WasPressedThisFrame())
        {
            Debug.Log("左摇杆按下");
        }
        if (InputActionsManager.Actions.XRILeftInteraction.ScaleToggle.WasReleasedThisFrame())
        {
            Debug.Log("左摇杆释放");
        }

        // 右手摇杆按下（ScaleToggle）
        if (InputActionsManager.Actions.XRIRightInteraction.ScaleToggle.WasPressedThisFrame())
        {
            Debug.Log("右摇杆按下");
        }
        if (InputActionsManager.Actions.XRIRightInteraction.ScaleToggle.WasReleasedThisFrame())
        {
            Debug.Log("右摇杆释放");
        }

        // 左手柄菜单键（Menu）
        if (InputActionsManager.Actions.XRILeftInteraction.Menu.WasPressedThisFrame())
        {
            Debug.Log("左手柄菜单键按下");
        }
        if (InputActionsManager.Actions.XRILeftInteraction.Menu.WasReleasedThisFrame())
        {
            Debug.Log("左手柄菜单键释放");
        }

    }

    // private void OnDestroy()
    // {
    //     // 禁用所有输入动作
    //     InputActionsManager.DisableAll();
    // }
}
