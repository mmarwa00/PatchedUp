using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Components")]
    public Transform FPCamera;
    [Tooltip("Drag the Teddy Bear 3D Model here so it can spin!")]
    public Transform TeddyBearModel;

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;

    [Header("Movement")]
    public float MaxSpeed = 3.5f;
    public float Acceleration = 20f;
    public float JumpHeight = 2.0f;
    public float GravityScale = 3.0f;

    [Header("Looking")]
    public Vector2 LookSensitivity = new Vector2(1f, 1f);
    public float PitchLimit = 85f;

    private float _currentPitch;
    private CharacterController _characterController;
    private Vector3 _currentVelocity;
    private float _verticalVelocity;

    public bool IsGrounded => _characterController.isGrounded;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        LookUpdate();
        MoveUpdate();
    }

    private void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);

        // Look Up/Down (Rotates the Camera)
        _currentPitch -= input.y;
        _currentPitch = Mathf.Clamp(_currentPitch, -PitchLimit, PitchLimit);
        if (FPCamera != null)
        {
            FPCamera.localRotation = Quaternion.Euler(_currentPitch, 0, 0);
        }

        // Look Left/Right (Rotates the invisible Capsule)
        transform.Rotate(Vector3.up * input.x);
    }

    private void MoveUpdate()
    {
        // motion calculation
        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        motion.y = 0;
        motion.Normalize();

        // Acceleration logic
        if (motion.sqrMagnitude > 0.01f)
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, motion * MaxSpeed, Acceleration * Time.deltaTime);
        else
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, Acceleration * Time.deltaTime);

        // --- THE ANTI-CRAB WALK ADDITION ---
        // Rotates the Bear mesh to face the movement direction, independent of the camera
        if (motion.sqrMagnitude > 0.01f && TeddyBearModel != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(motion);
            TeddyBearModel.rotation = Quaternion.Slerp(TeddyBearModel.rotation, targetRotation, Time.deltaTime * 15f);
        }

        // Gravity logic 
        if (IsGrounded && _verticalVelocity <= 0)
        {
            _verticalVelocity = -3f;
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }

        Vector3 fullVelocity = new Vector3(_currentVelocity.x, _verticalVelocity, _currentVelocity.z);
        _characterController.Move(fullVelocity * Time.deltaTime);
    }

    public void TryJump()
    {
        if (!IsGrounded) return;
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * (Physics.gravity.y * GravityScale));
    }
}
