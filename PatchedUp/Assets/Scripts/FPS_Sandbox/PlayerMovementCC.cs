using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementCC : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 12.0f;
        public float SprintSpeed = 18.0f;
        public float SpeedChangeRate = 10.0f;

        // ---> REQUIRED BY CAPTURESYSTEM.CS <---
        public float MoveSpeedValue { get => MoveSpeed; set => MoveSpeed = value; }
        public float SprintSpeedValue { get => SprintSpeed; set => SprintSpeed = value; }

        [Space(10)]
        public float JumpHeight = 6.0f;
        public float Gravity = -9.81f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;

        [Header("Crouch")]
        public float CrouchHeight = 3.0f; // Half your normal height of 6
        public float StandHeight = 6.0f; // Your normal height
        public float CrouchSpeed = 5.0f;
        private bool _isCrouching;

        [Header("Jumps")]
        public int MaxJumps = 3;
        private int _jumpsRemaining;

        private Animator _animator;
        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _animator = GetComponentInChildren<Animator>();

            if (_controller == null) Debug.LogError("PlayerMovementCC: Missing CharacterController!");
            if (_input == null) Debug.LogError("PlayerMovementCC: Missing StarterAssetsInputs!");
            if (_animator == null) Debug.LogWarning("PlayerMovementCC: Missing Animator! Animations won't play.");

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _jumpsRemaining = MaxJumps;
        }

        private void Update()
        {
            if (_input == null || _controller == null) return;

            Grounded = _controller.isGrounded;

            JumpAndGravity();
            Move();
            Crouch();
            UpdateAnimatorParameters();
        }

        private void Move()
        {
            // Simplified movement logic
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // Move relative to where the capsule is facing
            Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

            // Force movement even if inputDirection is tiny to test
            if (inputDirection != Vector3.zero) Debug.Log("Moving! Input Dir: " + inputDirection + ", Speed: " + _speed);

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        private void Crouch()
        {
            if (_input == null) return;

            bool wantCrouch = _input.crouch;

            float targetHeight = wantCrouch ? CrouchHeight : StandHeight;
            Vector3 targetCenter = new Vector3(0f, targetHeight / 2f, 0f);

            _controller.height = targetHeight;
            _controller.center = targetCenter;

            _isCrouching = wantCrouch;
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                _jumpsRemaining = MaxJumps;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpsRemaining > 0)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_animator != null) _animator.SetTrigger("NormalJump");
                    _jumpsRemaining--;
                    _input.jump = false;
                    _jumpTimeoutDelta = JumpTimeout;
                }
                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_input.jump && _jumpsRemaining > 0 && _jumpTimeoutDelta <= 0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_animator != null) _animator.SetTrigger("NormalJump");
                    _jumpsRemaining--;
                    _input.jump = false;
                    _jumpTimeoutDelta = JumpTimeout;
                }
                _jumpTimeoutDelta = Mathf.Min(_jumpTimeoutDelta, JumpTimeout);
                if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void UpdateAnimatorParameters()
        {
            if (_animator == null) return;
            _animator.SetFloat("Speed", _speed);
            _animator.SetBool("Grounded", Grounded);
        }
    }
}