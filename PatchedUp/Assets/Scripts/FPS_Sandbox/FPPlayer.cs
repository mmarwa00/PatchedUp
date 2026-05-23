using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class FPPlayer : MonoBehaviour
{
    private FPController _fpController;

    private void Start()
    {
        _fpController = GetComponent<FPController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Input System "Send Messages" setup
    private void OnMove(InputValue value)
    {
        _fpController.MoveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        _fpController.LookInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed) _fpController.TryJump();
    }
}
