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
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float MoveSpeed = 12.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        [SerializeField] private float SprintSpeed = 20.0f;
        [Tooltip("Rotation speed of the character")]
        [SerializeField] private float RotationSpeed = 5.0f;
        [Tooltip("Acceleration and deceleration")]
        [SerializeField] private float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        [SerializeField] private float JumpHeight = 5f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [SerializeField] private float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] private float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        [SerializeField] private bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        [SerializeField] private float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] private float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        [SerializeField] private LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField] private GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField] private float TopClamp = 89.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField] private float BottomClamp = -89.0f;

        [Header("Crouch")]
        [SerializeField] private float CrouchHeight = 1.0f;
        [SerializeField] private float StandHeight = 4.3f;
        [SerializeField] private float StandCenter = 2.15f;
        [SerializeField] private float CrouchSpeed = 2.0f;
        [SerializeField] private float CrouchCameraY = 1.8f;
        [SerializeField] private float StandCameraY = 4.0f;

        // Stun System
        [Header("Stun Settings")]
        [SerializeField] private float StunFallSpeedThreshold = -12.0f;
        [SerializeField] private float StunDuration = 1.5f;

        [Header("Visuals")]
        [Tooltip("Drag the TeddyBear object here so it visually turns")]
        [SerializeField] private GameObject BearModel;

        private Animator _animator;

        // Movement State
        public enum PlayerMovementState { Idle, Walking, Sprinting, Crouching, CrouchWalking }
        public PlayerMovementState CurrentMovementState { get; private set; }

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool _canMove = true;
        private bool _isStunned = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // crouching
        private float _targetHeight;
        private bool _isCrouching;

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
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            if (BearModel != null)
            {
                _animator = BearModel.GetComponent<Animator>();
            }
            else
            {
                _animator = GetComponentInChildren<Animator>();
            }

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _targetHeight = StandHeight;
        }

        public float MoveSpeedValue
        {
            get => MoveSpeed;
            set => MoveSpeed = value;
        }
        public float SprintSpeedValue
        {
            get => SprintSpeed;
            set => SprintSpeed = value;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            Crouch();
            UpdateAnimatorParameters();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // HYBRID CONTROL:
            // Mouse Y = tilt head UP/DOWN (to look at high obstacles)
            // Mouse X = IGNORED (body rotation handled by WASD in Move())

            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // ONLY process vertical look (Y axis)
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Apply ONLY vertical rotation (pitch)
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // Mouse X is completely ignored - no horizontal camera rotation
            }
        }

        private void Crouch()
        {
            if (!_canMove) return;

            if (_input.crouch && !_isCrouching)
            {
                _isCrouching = true;
                _controller.height = CrouchHeight;
                _controller.center = new Vector3(0, CrouchHeight / 2f, 0);
                CinemachineCameraTarget.transform.localPosition = new Vector3(0, CrouchCameraY, 2.5f);
            }
            else if (!_input.crouch && _isCrouching)
            {
                _isCrouching = false;
                _controller.height = StandHeight;
                _controller.center = new Vector3(0, StandCenter, 0);
                CinemachineCameraTarget.transform.localPosition = new Vector3(0, StandCameraY, 0.15f);
                _input.crouch = false;
            }
        }

        private void Move()
        {
            if (!_canMove) return;

            float targetSpeed = _isCrouching ? CrouchSpeed : _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero)
            {
                CurrentMovementState = _isCrouching ? PlayerMovementState.Crouching : PlayerMovementState.Idle;
                targetSpeed = 0.0f;
            }
            else if (_input.sprint)
            {
                CurrentMovementState = PlayerMovementState.Sprinting;
            }
            else if (_isCrouching)
            {
                CurrentMovementState = PlayerMovementState.CrouchWalking;
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

            Vector3 inputDirection = Vector3.zero;

            if (_input.move != Vector2.zero)
            {
                // 90-DEGREE INCREMENTAL TURNS RELATIVE TO CAMERA
                // W = forward, A = 90° left, S = 180° back, D = 90° right

                // Get camera's current Y rotation (where it's facing)
                float cameraYRotation = _mainCamera.transform.eulerAngles.y;

                // Calculate input angle relative to camera
                float inputAngle = Mathf.Atan2(_input.move.x, _input.move.y) * Mathf.Rad2Deg;

                // Snap to nearest 90-degree increment (0, 90, 180, 270)
                float snappedAngle = Mathf.Round(inputAngle / 90f) * 90f;

                // Add camera rotation to make it relative
                float targetAngle = cameraYRotation + snappedAngle;

                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

                // SMOOTH rotation to the snapped angle
                // Change this 8f number to adjust smoothness:
                // 5f = slower, more visible turning
                // 12f = faster, snappier turning
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

                // Movement follows body direction
                inputDirection = transform.forward;
                inputDirection.y = 0f;

                // Bear model faces forward in local space
                if (BearModel != null)
                {
                    BearModel.transform.localRotation = Quaternion.identity;
                }
            }
            else if (BearModel != null)
            {
                BearModel.transform.localRotation = Quaternion.identity;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            bool isHoldingJump = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                isHoldingJump = true;
            }
#endif

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < StunFallSpeedThreshold && !_isStunned)
                {
                    StartCoroutine(StunCoroutine());
                }

                // STRONGER GROUND FORCE - Changed from -2f to -8f
                // This PUSHES the bear into the ground instead of gently tapping him
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -8f;
                }

                // INSTANT JUMP - Removed the _jumpTimeoutDelta check
                // Now he can jump IMMEDIATELY when grounded
                if (_input.jump && _canMove)
                {
                    // ANIMATION FIRES INSTANTLY - Trigger happens BEFORE velocity change
                    if (_animator != null)
                    {
                        _animator.SetTrigger("NormalJump");
                    }

                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // FORCE UNGROUNDED - This stops the "float before jump" bug
                    // We FORCE the bear to NOT be grounded immediately so the jump starts NOW
                    Grounded = false;

                    _jumpTimeoutDelta = JumpTimeout; // Reset timeout AFTER jump
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                _input.jump = false;

                // -------- VARIABLE JUMP --------
                if (_verticalVelocity > 0.0f && !isHoldingJump)
                {
                    _verticalVelocity += Gravity * 2f * Time.deltaTime;
                }
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        // -------- CLAMPANGLE FUNCTION --------
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private System.Collections.IEnumerator StunCoroutine()
        {
            _isStunned = true;
            _canMove = false;
            yield return new WaitForSeconds(StunDuration);
            _canMove = true;
            _isStunned = false;
        }

        private void UpdateAnimatorParameters()
        {
            if (_animator == null) return;
            _animator.SetFloat("Blend", _speed);
            _animator.SetFloat("Speed", _speed);
            _animator.SetBool("Grounded", Grounded);
            _animator.SetBool("Crouching", _isCrouching);
            _animator.SetBool("Stunned", _isStunned);
        }
    }
}