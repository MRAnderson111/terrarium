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

        // 右手摇杆输入（Vector2输出）
        Vector2 rightThumbstick = InputActionsManager.Actions.XRIRightLocomotion.Move.ReadValue<Vector2>();

        // 左手柄A键（PrimaryButton）
        if (InputActionsManager.Actions.XRILeftInteraction.PrimaryButton.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRILeftInteraction.PrimaryButton.WasReleasedThisFrame())
        {
        }

        // 左手柄B键（SecondaryButton）
        if (InputActionsManager.Actions.XRILeftInteraction.SecondaryButton.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRILeftInteraction.SecondaryButton.WasReleasedThisFrame())
        {
        }

        // 右手柄A键（PrimaryButton）
        if (InputActionsManager.Actions.XRIRightInteraction.PrimaryButton.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRIRightInteraction.PrimaryButton.WasReleasedThisFrame())
        {
        }

        // 右手柄B键（SecondaryButton）
        if (InputActionsManager.Actions.XRIRightInteraction.SecondaryButton.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRIRightInteraction.SecondaryButton.WasReleasedThisFrame())
        {
        }

        // 左手柄扳机键（Activate）
        float leftTrigger = InputActionsManager.Actions.XRILeftInteraction.ActivateValue.ReadValue<float>();

        // 右手柄扳机键（Activate）
        float rightTrigger = InputActionsManager.Actions.XRIRightInteraction.ActivateValue.ReadValue<float>();

        // 左手柄侧握键（Grip）
        float leftGrip = InputActionsManager.Actions.XRILeftInteraction.SelectValue.ReadValue<float>();

        // 右手柄侧握键（Grip）
        float rightGrip = InputActionsManager.Actions.XRIRightInteraction.SelectValue.ReadValue<float>();

        // 左手摇杆按下（ScaleToggle）
        if (InputActionsManager.Actions.XRILeftInteraction.ScaleToggle.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRILeftInteraction.ScaleToggle.WasReleasedThisFrame())
        {
        }

        // 右手摇杆按下（ScaleToggle）
        if (InputActionsManager.Actions.XRIRightInteraction.ScaleToggle.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRIRightInteraction.ScaleToggle.WasReleasedThisFrame())
        {
        }

        // 左手柄菜单键（Menu）
        if (InputActionsManager.Actions.XRILeftInteraction.Menu.WasPressedThisFrame())
        {
        }
        if (InputActionsManager.Actions.XRILeftInteraction.Menu.WasReleasedThisFrame())
        {
        }

    }

    // private void OnDestroy()
    // {
    //     // 禁用所有输入动作
    //     InputActionsManager.DisableAll();
    // }
}
