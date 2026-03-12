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
    [SerializeField] private float lostSightDelay = 2f; // seconds to keep chasing after losing sight
    [SerializeField] private float speed = 5f;

    private NavMeshAgent _agent;
    private Vector3 _lastKnownPosition;
    private float _lostSightTimer;

    private enum State { Idle, Chasing, Lingering, Investigating }
    private State _state = State.Idle;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
    }

    void Update()
    {
        if (target == null) return;

        if (CanSeeTarget())
        {
            // Visible — update last known position and chase
            _lastKnownPosition = target.position;
            _lostSightTimer = lostSightDelay;
            _state = State.Chasing;
            _agent.SetDestination(target.position);
        }
        else if (_state == State.Chasing || _state == State.Lingering)
        {
            // Just lost sight or already lingering — count down
            _state = State.Lingering;
            _lostSightTimer -= Time.deltaTime;
            _agent.SetDestination(target.position); // keep following exact position

            if (_lostSightTimer <= 0f)
            {
                // Timer expired — move to last known position
                _state = State.Investigating;
                _agent.SetDestination(_lastKnownPosition);
            }
        }
        else if (_state == State.Investigating)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _state = State.Idle;
            }
        }
    }

    bool CanSeeTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > fieldOfView / 2f) return false;

        if (Physics.Raycast(transform.position, directionToTarget.normalized, distanceToTarget, obstacleLayer))
            return false;

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _state switch
        {
            State.Chasing      => Color.red,
            State.Lingering    => Color.magenta,
            State.Investigating => Color.yellow,
            _                  => Color.green
        };

        Gizmos.DrawWireSphere(transform.position, sightRange);
        Vector3 left  = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0,  fieldOfView / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, left  * sightRange);
        Gizmos.DrawRay(transform.position, right * sightRange);

        if (_state == State.Investigating)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_lastKnownPosition, 0.3f);
        }
    }
}