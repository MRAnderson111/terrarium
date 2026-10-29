using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionAsset actionAsset;
    private InputAction rightTriggerAction;
    private InputAction leftTriggerAction;
    private InputAction rightSelectAction;
    private InputAction leftSelectAction;
    private InputAction rightActivateAction;
    private InputAction leftActivateAction;
    private InputAction rightUIPressAction;
    private InputAction leftUIPressAction;
    private InputAction rightScaleToggleAction;
    private InputAction leftScaleToggleAction;
    private InputAction rightPrimary2DAxisAction;
    private InputAction leftPrimary2DAxisAction;
    // Start is called before the first frame update
    void Start()
    {
        rightTriggerAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Select Value");
        leftTriggerAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Select Value");
        rightSelectAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Select");
        leftSelectAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Select");
        rightActivateAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Activate");
        leftActivateAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Activate");
        rightUIPressAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("UI Press");
        leftUIPressAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("UI Press");
        rightScaleToggleAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Scale Toggle");
        leftScaleToggleAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Scale Toggle");
        rightPrimary2DAxisAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Rotate Anchor");
        leftPrimary2DAxisAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Rotate Anchor");
        
        rightTriggerAction.Enable();
        leftTriggerAction.Enable();
        rightSelectAction.Enable();
        leftSelectAction.Enable();
        rightActivateAction.Enable();
        leftActivateAction.Enable();
        rightUIPressAction.Enable();
        leftUIPressAction.Enable();
        rightScaleToggleAction.Enable();
        leftScaleToggleAction.Enable();
        rightPrimary2DAxisAction.Enable();
        leftPrimary2DAxisAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
        // 输出摇杆值
        Vector2 rightStickValue = rightPrimary2DAxisAction.ReadValue<Vector2>();
        Vector2 leftStickValue = leftPrimary2DAxisAction.ReadValue<Vector2>();
    }
}
