using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionAsset actionAsset;
    private InputAction rightTriggerAction;
    private InputAction leftTriggerAction;
    // Start is called before the first frame update
    void Start()
    {
        rightTriggerAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Select Value");
        leftTriggerAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Select Value");
        rightTriggerAction.Enable();
        leftTriggerAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (rightTriggerAction.triggered)
        {
            Debug.Log("右手柄扳机按下");
        }
        if (leftTriggerAction.triggered)
        {
            Debug.Log("左手柄扳机按下");
        }
    }
}
