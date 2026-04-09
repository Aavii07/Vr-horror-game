using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EntityVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Patrol")]
    [SerializeField] private PatrolPath patrolPath;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Search")]
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private int locationsToCheckUpper = 5;
    [SerializeField] private int locationsToCheckLower = 2;

    [Header("Animation")]
    [SerializeField] private float speedDampTime = 0.1f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _lastKnownPosition;

    private int _currentPatrolIndex;
    private float _patrolWaitTimer;

    private static readonly int SpeedParam    = Animator.StringToHash("Speed");
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
    private static readonly int AttackParam   = Animator.StringToHash("Attack");

    private enum State { Patrol, Chasing, Investigating, Searching }
    private State _state = State.Patrol;

    void Start()
    {
        _agent    = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _agent.speed = patrolSpeed;

        if (patrolPath != null && patrolPath.PointCount > 0)
            _agent.SetDestination(patrolPath.GetPoint(0).position);
    }

    void Update()
    {
        if (target == null) return;

        if (CanSeeTarget())
        {
            _lastKnownPosition = target.position;
            _state = State.Chasing;
            _agent.speed = chaseSpeed;
            _agent.SetDestination(target.position);
            RotateTowards(target.position);
        }
        else if (_state == State.Chasing || _state == State.Investigating)
        {
            _state = State.Investigating;
            _agent.speed = patrolSpeed;
            _agent.SetDestination(_lastKnownPosition);
            RotateTowards(_lastKnownPosition);

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                _currentPatrolIndex = NearestPatrolPointIndex(transform.position);
                _agent.SetDestination(patrolPath.GetPoint(_currentPatrolIndex).position);
                _state = State.Patrol;
                Patrol();
            }
        }
        else
        {
            _state = State.Patrol;
            _agent.speed = patrolSpeed;
            Patrol();
        }

        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        switch (_state)
        {
            case State.Patrol:
                _animator.SetFloat(SpeedParam, patrolSpeed, speedDampTime, Time.deltaTime);
                _animator.SetBool(IsMovingParam, true);
                break;

            case State.Chasing:
                _animator.SetFloat(SpeedParam, chaseSpeed, speedDampTime, Time.deltaTime);
                _animator.SetBool(IsMovingParam, true);
                break;

            case State.Investigating:
                _animator.SetFloat(SpeedParam, patrolSpeed, speedDampTime, Time.deltaTime);
                _animator.SetBool(IsMovingParam, true);
                break;

            case State.Searching:
                _animator.SetFloat(SpeedParam, 0f, speedDampTime, Time.deltaTime);
                _animator.SetBool(IsMovingParam, false);
                break;
        }
    }

    public void TriggerAttack()
    {
        _animator.SetTrigger(AttackParam);
    }

    void Patrol()
    {
        if (patrolPath == null || patrolPath.PointCount == 0) return;

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

    void SearchArea() { }

    int NearestPatrolPointIndex(Vector3 position)
    {
        if (patrolPath == null || patrolPath.PointCount == 0) return -1;

        int nearestIndex = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < patrolPath.PointCount; i++)
        {
            float distance = Vector3.Distance(position, patrolPath.GetPoint(i).position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPath == null || patrolPath.PointCount == 0) return;
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPath.PointCount;
        _agent.SetDestination(patrolPath.GetPoint(_currentPatrolIndex).position);
    }

    void RotateTowards(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    bool CanSeeTarget()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 directionToTarget = target.position - origin;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, directionToTarget.normalized);
        if (angle > fieldOfView / 2f) return false;

        if (Physics.Raycast(origin, directionToTarget.normalized, distanceToTarget, obstacleLayer)) return false;

        return true;
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Gizmos.color = _state switch
        {
            State.Chasing       => Color.red,
            State.Investigating => Color.yellow,
            State.Searching     => Color.cyan,
            _                   => Color.green
        };

        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 left  = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0,  fieldOfView / 2f, 0) * transform.forward;
        Gizmos.DrawRay(origin, left  * sightRange);
        Gizmos.DrawRay(origin, right * sightRange);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, target.position);
        }
    }
}