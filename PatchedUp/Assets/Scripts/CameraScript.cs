using UnityEngine;
using UnityEngine.InputSystem;


public class CameraScript : MonoBehaviour
{

    [SerializeField] private float sensitivity;
    [SerializeField] private float verticalLimit;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform body;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentX;
    private float currentY;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentX = xRotation;
        currentY = yRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        currentX = Mathf.Lerp(currentX, xRotation, smoothSpeed * Time.deltaTime);
        currentY = Mathf.Lerp(currentY, yRotation, smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentX, currentY, 0);
        orientation.rotation = Quaternion.Euler(0, currentY, 0);
        body.rotation = Quaternion.Euler(0, currentY, 0);

    }
}
