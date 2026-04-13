using UnityEngine;
public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float dampening = 5f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform orientation;
    private CharacterController controller;
    private float jumpVelocity = 0f;
    private bool isJumping = false;
    private float jumpCooldownTimer = 0f;
    private bool isWalking;
    private bool jumpRequested = false;
    private void Start() {
        controller = GetComponent<CharacterController>();
        gameInput.OnJumpAction += GameInput_OnJumpAction;
    }
    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        jumpRequested = true;
    }
    private void Update() {
        HandleJumping();
        Vector2 inputMovement = gameInput.GetMovementVectorNormalized();
        Vector3 inputRightDirection = orientation.right;
        Vector3 inputForwardDirection = orientation.forward;
        inputRightDirection.y = 0f;
        inputForwardDirection.y = 0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        // Gravity
        if (controller.isGrounded && jumpVelocity < 0f) {
            jumpVelocity = -2f;
        }
        else if (!controller.isGrounded) {
            jumpVelocity -= gravity * Time.deltaTime;
        }

        // Horizontal movement
        Vector3 move = inputRightDirection * inputMovement.x * moveSpeed
                     + inputForwardDirection * inputMovement.y * moveSpeed;
        move.y = jumpVelocity;
        isWalking = new Vector3(inputMovement.x, 0f, inputMovement.y) != Vector3.zero;
        controller.Move(move * Time.deltaTime);
    }
    public bool IsWalking() {
        return isWalking;
    }
    void HandleJumping() {
        if (controller.isGrounded && isJumping && jumpCooldownTimer <= 0f) {
            isJumping = false;
            jumpVelocity = 0f;
        }
        if (controller.isGrounded && !isJumping && jumpRequested) {
            jumpVelocity = jumpSpeed;
            jumpCooldownTimer = jumpCooldown;
            isJumping = true;
        }
        jumpRequested = false;
        if (jumpVelocity > 0f) {
            jumpVelocity -= Time.deltaTime * 10f;
        }
        jumpCooldownTimer -= Time.deltaTime;
    }
}