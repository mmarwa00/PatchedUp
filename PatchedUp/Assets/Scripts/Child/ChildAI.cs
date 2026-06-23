using GLTFast.Schema;
using UnityEngine;
using UnityEngine.AI;

public class ChildAI : MonoBehaviour {

    [Header("Patrol Settings")]
    [SerializeField] private float baseWalkRange = 1.5f;
    [SerializeField] private float openAreaWalkRange = 4.0f;
    [SerializeField] private float patrolSpeed = 0.1f;
    [SerializeField] private float chaseSpeed = 0.2f;
    [SerializeField] private float searchWaitTime = 2.5f;

    private Vector3 _destPoint;
    private bool _walkPointSet;
    private float _stuckTimer = 0f;
    private float _waitTimer = 0f;
    private bool _isSearchingAtPoint = false;

    [Header("Sichtkegel (Visual Detection)")]
    [SerializeField] private float sightRange = 5.0f;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Capture")]
    [SerializeField] private float captureDistance = 0.3f;
    private NavMeshAgent _agent;
    private GameObject _player;
    private CaptureSystem _captureSystem;
    private Animator animator;
    private bool _playerDetected = false;

    private bool _isPaused = false;

    private float _smoothedAnimSpeed = 1f;

    [Header("Proximity Sound")]
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private float hearingRange = 8f;
    [SerializeField] private float maxVolume = 1f;
    private void Awake() {
        Debug.Log("Kind ist aufgewacht und lebt!");
    }

    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null) {
            _captureSystem = _player.GetComponent<CaptureSystem>();
        }
        animator = GetComponentInChildren<Animator>();

        if (_agent != null) {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas)) {
                _agent.Warp(hit.position);
                Debug.Log("Kind wurde erfolgreich auf das NavMesh gewarpt!");
            }
        }
    }

    private void Update() {
        if (_agent == null || !_agent.isOnNavMesh) return;
        if (_isPaused) return;

        _playerDetected = CanSeePlayer();

        if (_playerDetected) {
            _isSearchingAtPoint = false;
            Chase();
            TryCapture();
        }
        else {
            Patrol();
        }

        if (animator != null) {
            float speed = _agent.velocity.magnitude;
            bool isWalking = speed > 0.1f;
            animator.SetBool("IsWalking", isWalking);

            float targetAnimSpeed = isWalking ? Mathf.Clamp(speed / patrolSpeed, 0.2f, 2f) : 1f;

            if (!isWalking && animator.GetCurrentAnimatorStateInfo(0).IsName("Walking")) {
                targetAnimSpeed = 0.2f;
            }

            _smoothedAnimSpeed = Mathf.Lerp(_smoothedAnimSpeed, targetAnimSpeed, Time.deltaTime * 8f);
            animator.speed = _smoothedAnimSpeed;
        }

        if (footsteps != null && _player != null) {
            float dist = Vector3.Distance(transform.position, _player.transform.position);

            if (dist <= hearingRange) {
                float volume = 1f - (dist / hearingRange);
                footsteps.volume = Mathf.Clamp01(volume * maxVolume);

                if (!footsteps.isPlaying) {
                    footsteps.Play();
                }
            }
            else {
                if (footsteps.isPlaying) {
                    footsteps.Stop();
                }
            }
        }

    }

    public void SetPaused(bool paused) {
        _isPaused = paused;
        if (_agent != null && _agent.isOnNavMesh) {
            _agent.isStopped = paused;
        }
        if (animator != null) {
            animator.speed = paused ? 0f : 1f;
        }
    }

    private bool CanSeePlayer() {
        if (_player == null || !_agent.isOnNavMesh) return false;

        Vector3 dirToPlayer = _player.transform.position - transform.position;
        float distance = dirToPlayer.magnitude;

        if (distance > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle / 2f) return false;

        Vector3 rayStart = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(rayStart, dirToPlayer.normalized, distance, obstacleLayers)) {
            return false;
        }

        return true;
    }

    private void Chase() {
        if (!_agent.isOnNavMesh) return;
        _agent.speed = chaseSpeed;
        _agent.SetDestination(_player.transform.position);
    }

    private void TryCapture() {
        if (_captureSystem == null) return;

        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist <= captureDistance) {
            _captureSystem.OnCaught();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && _captureSystem != null) {
            _captureSystem.OnCaught();
        }
    }

    private void Patrol() {
        _agent.speed = patrolSpeed;

        if (_isSearchingAtPoint) {
            _waitTimer += Time.deltaTime;

            //Das Kind rotiert beim Warten langsam hin und her
            transform.Rotate(0, Mathf.Sin(Time.time * 3f) * 0.5f, 0);

            if (_waitTimer >= searchWaitTime) {
                _isSearchingAtPoint = false;
                _walkPointSet = false;
            }
            return;
        }

        if (!_walkPointSet) SearchForDest();

        if (_walkPointSet) {
            _agent.SetDestination(_destPoint);
            _stuckTimer += Time.deltaTime;

            if (Vector3.Distance(transform.position, _destPoint) < 1.5f || _stuckTimer > 4f) {
                _isSearchingAtPoint = true;
                _waitTimer = 0f;
                _stuckTimer = 0f;
            }
        }
    }

    private void SearchForDest() {
        float currentRange = baseWalkRange;

        if (!Physics.Raycast(transform.position, transform.forward, 8f, obstacleLayers)) {
            currentRange = openAreaWalkRange;
        }

        float randomZ = Random.Range(-currentRange, currentRange);
        float randomX = Random.Range(-currentRange, currentRange);

        Vector3 candidate = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4.0f, NavMesh.AllAreas)) {
            _destPoint = hit.position;
            _walkPointSet = true;
            _stuckTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 leftBound = Quaternion.Euler(0, -sightAngle / 2f, 0) * transform.forward;
        Vector3 rightBound = Quaternion.Euler(0, sightAngle / 2f, 0) * transform.forward;
        Gizmos.color = Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 1f;
        Gizmos.DrawRay(rayStart, leftBound * sightRange);
        Gizmos.DrawRay(rayStart, rightBound * sightRange);
    }
}