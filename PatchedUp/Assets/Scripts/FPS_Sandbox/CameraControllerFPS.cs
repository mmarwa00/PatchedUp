using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class CameraControllerFPS : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The main player object that should rotate left/right")]
        public Transform PlayerBody;

        [Tooltip("The object that should rotate up/down (usually this object itself)")]
        public Transform CameraTarget;

        [Header("Settings")]
        public float RotationSpeed = 1.0f;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        private float _cinemachineTargetPitch;
        private StarterAssetsInputs _input;
        private const float _threshold = 0.01f;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Start()
        {
            // Lock and hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Get inputs from the player body
            if (PlayerBody != null)
            {
                _input = PlayerBody.GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
                _playerInput = PlayerBody.GetComponent<PlayerInput>();
#endif
            }

            if (CameraTarget == null) CameraTarget = transform;
        }

        private void LateUpdate()
        {
            if (_input == null) return;

            CameraRotation();
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // 1. Mouse Y controls Pitch (up/down look) applied to the Camera
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
                CameraTarget.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // 2. Mouse X controls Yaw (left/right look) applied to the whole Player body
                float yawVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;
                PlayerBody.Rotate(Vector3.up * yawVelocity);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
