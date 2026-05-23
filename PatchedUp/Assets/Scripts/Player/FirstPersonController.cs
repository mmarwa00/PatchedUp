using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 12.0f;
        public float SprintSpeed = 18.0f;
        public float RotationSpeed = 1.0f;
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

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Crouch")]
        public float CrouchHeight = 1.0f;
        public float StandHeight = 2.0f;
        public float StandCenter = 0.93f;
        public float CrouchSpeed = 2.0f;
        public float CrouchCameraY = 0.5f;
        public float StandCameraY = 1.375f;

        [Header("Jumps")]
        public int MaxJumps = 3; // total jumps (1 ground + extra mid-air)
        private int _jumpsRemaining;

        private float _cinemachineTargetPitch;
        private float _cinemachineTargetYaw;

        private Animator _animator;
        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool _isCrouching;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        
        //Global movement state monitoring for environmental interaction puzzle
        public enum PlayerMovementState {Idle, Walking, Sprinting, Crouching, CrouchWalking}
        public PlayerMovementState CurrentMovementState { get; private set; }

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _animator = GetComponentInChildren<Animator>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // auto-assign CinemachineCameraTarget if null (matches your PlayerCameraRoot)
            if (CinemachineCameraTarget == null)
            {
                var candidate = GameObject.Find("PlayerCameraRoot");
                if (candidate != null) CinemachineCameraTarget = candidate;
            }

            // init jumps
            _jumpsRemaining = MaxJumps;
        }

        private void Update()
        {
            if (_input == null) return;

            Grounded = _controller.isGrounded;

            JumpAndGravity();
            Move();
            Crouch();
            UpdateAnimatorParameters();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // Only mouse Y controls pitch (up/down look)
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                if (CinemachineCameraTarget != null)
                {
                    CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                }
            }

            // Rotate the PLAYER CAPSULE to match movement direction
            if (_input.move != Vector2.zero)
            {
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        private void Crouch()
        {
            if (_input == null) return;

            // treat crouch as hold (ensure Input Action is Button, not Toggle)
            bool wantCrouch = _input.crouch;

            // compute target height and center (center = height/2 keeps feet grounded)
            float targetHeight = wantCrouch ? CrouchHeight : StandHeight;
            Vector3 targetCenter = new Vector3(0f, targetHeight / 2f, 0f);

            // immediate set to avoid floating; if you want smoothing, replace with Lerp
            _controller.height = targetHeight;
            _controller.center = targetCenter;

            if (CinemachineCameraTarget != null)
            {
                float camY = wantCrouch ? CrouchCameraY : StandCameraY;
                CinemachineCameraTarget.transform.localPosition = new Vector3(0f, camY, 0f);
            }

            _isCrouching = wantCrouch;
        }

        private void Move()
        {
            if (_input == null) return;

            float targetSpeed = _isCrouching ? CrouchSpeed
                  : _input.sprint ? SprintSpeed
                  : MoveSpeed;

            if (_input.move == Vector2.zero)
            {
                if (!_isCrouching)
                {
                    CurrentMovementState = PlayerMovementState.Idle;
                }
                else
                {
                    CurrentMovementState = PlayerMovementState.Crouching;
                }
                
                targetSpeed = 0.0f;
            }
            else if(_isCrouching)
            {
                CurrentMovementState = PlayerMovementState.CrouchWalking;
            }
            else if(_input.sprint)
            {
                CurrentMovementState = PlayerMovementState.Sprinting;
            }
            else
            {
                CurrentMovementState = PlayerMovementState.Walking;
            }

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

            Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

            

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                _jumpsRemaining = MaxJumps; // reset jumps when grounded

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // allow immediate grounded jump
                if (_input.jump && _jumpsRemaining > 0)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_animator != null)
                    {
                        if (_input.sprint) _animator.SetTrigger("JumpUp");
                        else _animator.SetTrigger("NormalJump");
                    }

                    _jumpsRemaining--;
                    _input.jump = false;
                    _jumpTimeoutDelta = JumpTimeout;
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // mid-air jump (double/triple)
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
            _animator.SetBool("Crouching", _isCrouching);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}