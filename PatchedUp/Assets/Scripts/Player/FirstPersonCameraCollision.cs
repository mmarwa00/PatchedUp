using UnityEngine;
using Unity.Cinemachine;

namespace StarterAssets
{
    /// <summary>
    /// First-person eye collision for a Cinemachine 3rd Person Follow rig.
    ///
    /// The eye is pushed forward of the head by <see cref="ForwardOffset"/> (the
    /// "negative CameraDistance" look that keeps the player model visible). Each frame
    /// a spherecast probes ahead and shortens that offset so the eye can never pass
    /// through walls. This replaces the Cinemachine Deoccluder for first-person use,
    /// which cannot guard a camera that sits on its own follow target.
    ///
    /// Setup:
    ///  - Put this on the CinemachineCamera object (the one with the 3rd Person Follow).
    ///  - It drives 3rd Person Follow > Camera Distance every frame, so don't set that by hand.
    ///  - Set <see cref="CollisionLayers"/> to your walls/furniture (e.g. Default, Ground, Door)
    ///    and leave OUT the Player layer and the pickable-items layer.
    ///  - Remove/disable the Deoccluder extension once this is in.
    /// </summary>
    [DefaultExecutionOrder(200)] // run after the controller pitches the camera target in LateUpdate
    [RequireComponent(typeof(CinemachineThirdPersonFollow))]
    public class FirstPersonCameraCollision : MonoBehaviour
    {
        [Tooltip("How far in front of the head the eye sits when nothing is in the way.")]
        [SerializeField] private float ForwardOffset = 0.5f;
        [Tooltip("Thickness used when probing for walls. Larger = the eye stops further from surfaces.")]
        [SerializeField] private float CameraRadius = 0.2f;
        [Tooltip("Layers the eye must not pass through. Include walls/furniture; exclude Player and pickables.")]
        [SerializeField] private LayerMask CollisionLayers = ~0;
        [Tooltip("Extra gap kept between the eye and a surface it hits.")]
        [SerializeField] private float SkinPadding = 0.05f;

        private CinemachineThirdPersonFollow _body;

        private void Awake()
        {
            _body = GetComponent<CinemachineThirdPersonFollow>();
        }

        private void LateUpdate()
        {
            if (_body == null) return;

            Transform target = _body.FollowTarget;
            if (target == null) return;

            // Negative CameraDistance places the eye at: target.position + target.forward * |distance|,
            // so probe along target.forward from the head and clamp the offset to the first hit.
            Vector3 origin = target.position;
            Vector3 dir = target.forward;

            float allowed = ForwardOffset;
            if (Physics.SphereCast(origin, CameraRadius, dir, out RaycastHit hit,
                    ForwardOffset + CameraRadius, CollisionLayers, QueryTriggerInteraction.Ignore))
            {
                allowed = Mathf.Clamp(hit.distance - CameraRadius - SkinPadding, 0f, ForwardOffset);
            }

            _body.CameraDistance = -allowed; // negative pushes the eye forward of the head
        }
    }
}
