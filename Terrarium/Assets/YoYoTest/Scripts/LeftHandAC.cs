using UnityEngine;
using UnityEngine.XR;

public class LeftHandAC : MonoBehaviour
{
    private Animator animator;
    private InputDevice leftController;
    private bool triggerPressed = false;
    private bool previousTriggerState = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

    }

    void Update()
    {
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        if (leftController.isValid)
        {
            // 尝试多种trigger检测方式
            bool triggerButton = false;
            float triggerValue = 0f;

            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton);
            leftController.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

            // 使用trigger按钮或trigger值大于0.5作为触发条件
            triggerPressed = triggerButton || triggerValue > 0.5f;

            // 每次按下扳机时触发动画
            if (triggerPressed && !previousTriggerState)
            {
                if (animator != null)
                {
                    // 触发动画播放
                    animator.SetTrigger("bIsTrigger");
                    Debug.Log("LeftHandAC: 扳机按下 - 触发动画");
                }
            }
            previousTriggerState = triggerPressed;
        }

        // 键盘测试（无论控制器是否有效都可以使用）
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (animator != null)
            {
                animator.SetTrigger("bIsTrigger");
                Debug.Log("LeftHandAC: 键盘测试 - 触发动画");
            }
        }
    }
}
