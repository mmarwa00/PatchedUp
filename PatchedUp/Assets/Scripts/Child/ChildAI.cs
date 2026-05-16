using UnityEngine;
using UnityEngine.AI;

public class ChildAI : MonoBehaviour {

    [Header("Patrol")]
    [SerializeField] private float walkRange = 5f;
    private float _stuckTimer = 0f;

    private Vector3 _destPoint;
    private bool _walkPointSet;
    [Header("Sichtkegel")]
    [SerializeField] private float sightRange = 8f;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LayerMask playerLayer;

    [Header("Capture")]
    [SerializeField] private float captureDistance = 1.0f;

    private NavMeshAgent _agent;
    private GameObject _player;
    private CaptureSystem _captureSystem;
    private bool _playerDetected = false;
    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _captureSystem = _player.GetComponent<CaptureSystem>();
    }

    private void Update() {
        _playerDetected = CanSeePlayer();

        if (_playerDetected) {
            Chase();
            TryCapture();
        }
        else {
            Patrol();
        }
    }

    private bool CanSeePlayer() {
        Vector3 dirToPlayer = _player.transform.position - transform.position;
        float distance = dirToPlayer.magnitude;

        // Zu weit weg?
        if (distance > sightRange) return false;

        // Außerhalb des Kegels?
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle / 2f) return false;

        // Wand dazwischen?
        if (Physics.Raycast(transform.position, dirToPlayer.normalized,
            distance, obstacleLayers)) return false;

        return true;
    }

    private void Chase() {
        _agent.SetDestination(_player.transform.position);
    }

    private void TryCapture() {
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist <= captureDistance) {
            _captureSystem.OnCaught();
        }
    }

    private void Patrol() {
        if (!_walkPointSet) SearchForDest();

        if (_walkPointSet) {
            _agent.SetDestination(_destPoint);
            _stuckTimer += Time.deltaTime;


            if (_agent.pathStatus == NavMeshPathStatus.PathInvalid ||
                _agent.pathStatus == NavMeshPathStatus.PathPartial ||
                _stuckTimer > 3f ||
                Vector3.Distance(transform.position, _destPoint) < 1f) {
                _walkPointSet = false;
                _stuckTimer = 0f;
            }
        }
    }

    private void SearchForDest() {
        float randomZ = Random.Range(-walkRange, walkRange);
        float randomX = Random.Range(-walkRange, walkRange);

        Vector3 candidate = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2.0f, NavMesh.AllAreas)) {
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
        Gizmos.DrawRay(transform.position, leftBound * sightRange);
        Gizmos.DrawRay(transform.position, rightBound * sightRange);
    }
}