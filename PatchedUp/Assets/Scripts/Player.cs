using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform orientation;

    private Rigidbody rb;
    private bool isWalking;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gameInput.OnJumpAction += GameInput_OnJumpAction;
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Update() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        moveDir.y = 0f;

        isWalking = moveDir != Vector3.zero;
    }

    private void FixedUpdate() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        moveDir.y = 0f;

        rb.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed) {
            Vector3 capped = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(capped.x, rb.linearVelocity.y, capped.z);
        }

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x * (1f - groundDrag * Time.fixedDeltaTime),
            rb.linearVelocity.y,
            rb.linearVelocity.z * (1f - groundDrag * Time.fixedDeltaTime)
        );
    }

    public bool IsWalking() {
        return isWalking;
    }
}