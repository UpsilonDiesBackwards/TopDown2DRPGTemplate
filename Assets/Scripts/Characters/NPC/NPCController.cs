using Pathfinding;
using UnityEngine;

/*
 * The most jankiest A* Path Finding implementation out there.
 *
 * Has three states Roam, Pursue, and Patrol. Patrol does not currently work :'(
 */

public enum MovementBehavoiur {
    Roamer,
    Patroller,
    Pursuer
}

[RequireComponent(typeof(Seeker))]
public class NPCController : PhysicsObject {
    [Header("Behaviour")]
    public MovementBehavoiur behaviour;

    [SerializeField] private float minWaitTime = 5f;
    [SerializeField] private float maxWaitTime = 10f;
    [SerializeField] private float nextWaypointDistance = 0.5f;
    
    [Header("Roaming")]
    float minRoamRadius = 5f;
    public float maxRoamRadius = 5f;

    [Header("Patrolling")]
    public Transform[] patrolPoints;

    [Header("Persuing")]
    public Transform followTarget;
    [SerializeField] private float minimumDistanceToTarget = 3f;
    private bool _isDisengaging = false;

    // Private References
    [SerializeField] private Vector3 _targetPos;
    private Seeker _seeker;
    private CharacterController _controller;
    private Path _path;
    private int _currentWaypoint = 0;
    private bool _roaming = false;
    private bool _patrollingForward = true;
    private bool _isMovingToTarget = false;
    private bool _hasReachedTarget = false;

    void OnDrawGizmos() {
        // Draw NPC movement path
        if (_path == null) return;

        Gizmos.color = Color.red;

        for (int i = 0; i < _path.vectorPath.Count - 1; i++) {
            Gizmos.DrawLine(_path.vectorPath[i], _path.vectorPath[i + 1]);
        }

        // Draw Persue minimum distance radius
        if (behaviour == MovementBehavoiur.Pursuer && followTarget != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(followTarget.position, minimumDistanceToTarget);
        }
    }

    void Start() {
        _seeker = GetComponent<Seeker>();

        switch (behaviour) {
            case MovementBehavoiur.Roamer:
                InvokeRepeating("Roam", 0f, Random.Range(minWaitTime, maxWaitTime));
                return;
            case MovementBehavoiur.Patroller:
                InvokeRepeating("Patrol", 0f, Random.Range(minWaitTime, maxWaitTime));
                break;
            case MovementBehavoiur.Pursuer:
                if (followTarget == null) {
                    Debug.LogError("Target is not set for Pursuer behavior!");
                    enabled = false;
                    return;
                }
                InvokeRepeating("Pursue", 0f, 0.15f);
                break;
            default:
                Debug.LogError("Unknown NPC behavior!");
                enabled = false;
                return;
        }
    }

    void Update() {
        if (_path == null) { return; }

        if (_currentWaypoint >= _path.vectorPath.Count) {
            _path             = null;
            _hasReachedTarget = true;
            return;
        }

        if (!_isDisengaging && !GetComponent<NPC>().frozen) {
            Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - rb.position).normalized;
            Vector2 move = direction * moveSpeed;
            targetVel = move;
        } else {
            targetVel = Vector2.zero;
        }

        if (Vector2.Distance(rb.position, _path.vectorPath[_currentWaypoint]) < nextWaypointDistance) {
            _currentWaypoint++;
        }

        if (_currentWaypoint >= _path.vectorPath.Count) {
            targetVel         = Vector2.zero;
            _hasReachedTarget = true;
        }
    }

    void Roam() {
        if (!_isMovingToTarget) {
            _roaming          = true;
            _targetPos        = GetRandomPosition();
            _hasReachedTarget = false;
            _seeker.StartPath(transform.position, _targetPos, OnPathComplete);
        }
    }

    void Patrol() {
        if (!_isMovingToTarget) {
            int nextWaypointIndex;
            if (_patrollingForward) {
                nextWaypointIndex = _currentWaypoint + 1;
                if (nextWaypointIndex >= patrolPoints.Length) {
                    _patrollingForward = false;
                    nextWaypointIndex  = patrolPoints.Length - 2;
                }
            } else {
                nextWaypointIndex = _currentWaypoint - 1;
                if (nextWaypointIndex < 0) {
                    _patrollingForward = true;
                    nextWaypointIndex  = 1;
                }
            }

            Vector3 targetPosition = patrolPoints[nextWaypointIndex].position;
            _seeker.StartPath(transform.position, targetPosition, OnPathComplete);

            _currentWaypoint = nextWaypointIndex;
        }
    }


    void Pursue() {
        if (!_isMovingToTarget) {
            _roaming = true;
            float distanceToTarget = Vector3.Distance(transform.position, followTarget.position);

            if (distanceToTarget < minimumDistanceToTarget) { // if the NPC is persuing and too close to the player, back tf off.
                _isDisengaging = true;

                Vector3 directionFromTarget = (transform.position - followTarget.position).normalized;
                Vector3 backOffPosition = transform.position + directionFromTarget * (minimumDistanceToTarget * 1.1f); // Slightly outside the minimum distance
            
                _seeker.StartPath(transform.position, backOffPosition, OnPathComplete);
            } else {
                _isDisengaging = false;
                _seeker.StartPath(transform.position, followTarget.position, OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path p ) {
        if (!p.error) {
            _path             = p;
            _currentWaypoint  = 0;
            _hasReachedTarget = false;
        }
        _roaming = false;
    }

    Vector3 GetRandomPosition() {
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(minRoamRadius, maxRoamRadius);
        randomDirection += transform.position;
        randomDirection.z = 0;
        return randomDirection;
    }

    public void MoveToDestination(Transform destination) {
        _targetPos        = destination.position;
        _isMovingToTarget = true;
        _hasReachedTarget = false;
        _seeker.StartPath(transform.position, _targetPos, OnPathComplete);
    }

    public bool HasReachedDestination() {
        return _hasReachedTarget;
    }
}
