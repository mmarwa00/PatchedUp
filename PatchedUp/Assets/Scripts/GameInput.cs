using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnJumpAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {

        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;

    }

}
