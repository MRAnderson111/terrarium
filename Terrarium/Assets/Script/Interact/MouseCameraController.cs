using UnityEngine;

public class MouseCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    public float smoothTime = 0.1f;

    float pitch;
    float yaw;
    Vector2 mouseDelta;
    Vector2 mouseVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yaw = transform.eulerAngles.y;
        pitch = Mathf.DeltaAngle(0f, transform.eulerAngles.x);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouseDelta = Vector2.SmoothDamp(mouseDelta, input, ref mouseVelocity, smoothTime);
            yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch - mouseDelta.y * mouseSensitivity * Time.deltaTime, minVerticalAngle, maxVerticalAngle);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
