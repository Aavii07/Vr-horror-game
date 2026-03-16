using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform target;

    [Header("Chase")]
    [SerializeField] private float lostSightDelay = 4f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Patrol")]
    [SerializeField] private PatrolPath patrolPath;
    [SerializeField] private float patrolWaitTime = 2f;


    

    private NavMeshAgent _agent;
    private Vector3 _lastKnownPosition;
    private float _lostSightTimer;

    private int _currentPatrolIndex;
    private float _patrolWaitTimer;

    private enum State { Patrol, Chasing, Lingering, Investigating }
    private State _state = State.Patrol;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;

        if (patrolPath != null && patrolPath.PointCount > 0)
        {
            _agent.SetDestination(patrolPath.GetPoint(0).position);
        }
    }

    void Update()
    {
        if (target == null) return;

        if (CanSeeTarget())
        {
            _lastKnownPosition = target.position;
            _lostSightTimer = lostSightDelay;

            _state = State.Chasing;
            _agent.SetDestination(target.position);

            RotateTowards(target.position);
        }
        else if (_state == State.Chasing || _state == State.Lingering)
        {
            _state = State.Lingering;
            _lostSightTimer -= Time.deltaTime;

            _agent.SetDestination(_lastKnownPosition);
            RotateTowards(_lastKnownPosition);

            if (_lostSightTimer <= 0f)
            {
                _state = State.Investigating;
                _agent.SetDestination(_lastKnownPosition);
            }
        }
        else if (_state == State.Investigating)
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                _state = State.Patrol;
                GoToNextPatrolPoint();
            }
        }
        else if (_state == State.Patrol)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPath == null || patrolPath.PointCount == 0)
            return;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _patrolWaitTimer += Time.deltaTime;

            if (_patrolWaitTimer >= patrolWaitTime)
            {
                GoToNextPatrolPoint();
                _patrolWaitTimer = 0f;
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPath == null || patrolPath.PointCount == 0)
            return;

        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPath.PointCount;

        Transform nextPoint = patrolPath.GetPoint(_currentPatrolIndex);
        _agent.SetDestination(nextPoint.position);
    }

    void RotateTowards(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        direction.y = 0;

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    bool CanSeeTarget()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 directionToTarget = target.position - origin;

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > sightRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToTarget.normalized);

        if (angle > fieldOfView / 2f)
            return false;

        if (Physics.Raycast(origin, directionToTarget.normalized, distanceToTarget, obstacleLayer))
            return false;

        return true;
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.DrawRay(origin, left * sightRange);
        Gizmos.DrawRay(origin, right * sightRange);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, target.position);
        }
    }
}