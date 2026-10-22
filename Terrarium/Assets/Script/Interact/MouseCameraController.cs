using UnityEngine;

public class MouseCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    public float smoothTime = 0.1f;
    public float moveSpeed = 10f;
    public float maxDistanceFromCenter = 20f;
    public Transform planeCenter;

    float pitch;
    float yaw;
    Vector2 mouseDelta;
    Vector2 mouseVelocity;
    Vector3 center;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        center = planeCenter != null ? planeCenter.position : transform.position;
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

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (movement.sqrMagnitude == 0f)
            return;
        Vector3 position = transform.position + movement.normalized * moveSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(position.x - center.x, 0f, position.z - center.z);
        offset = Vector3.ClampMagnitude(offset, maxDistanceFromCenter);
        transform.position = new Vector3(center.x + offset.x, position.y, center.z + offset.z);
    }
}
