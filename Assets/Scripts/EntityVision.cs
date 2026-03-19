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



    // Patrol: default state. Follows the path that has been designated by the Patrol points 
    // Chasing: seen the player and now is following them
    // Investigating: Lost sight of the player so now is going to the last known location
    private enum State { Patrol, Chasing, Investigating }
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

        // Start the chase and go towards the target
        if (CanSeeTarget())
        {
            _lastKnownPosition = target.position;

            _state = State.Chasing;
            _agent.SetDestination(target.position);

            RotateTowards(target.position);
        }

        // Just lost sight or is still following the location after losing sight
        // Goes towards the player's last known position and then once it gets there,
        // if it hasn't see the player yet, go back to paatroling
        else if (_state == State.Chasing || _state == State.Investigating)
        {
            _state = State.Investigating;


            _agent.SetDestination(_lastKnownPosition);
            RotateTowards(_lastKnownPosition);


            // enemy has gotten to the player's location and hasn't seen the player
            // now start patrolling
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                // set (next patrol point to be the nearest point and then go to it)
                _currentPatrolIndex = NearestPatrolPointIndex(transform.position);
                _agent.SetDestination(patrolPath.GetPoint(_currentPatrolIndex).position);


                _state = State.Patrol;
                Patrol();
            }
        }

        // It is not chasing or investigating the last spot it saw the player, go back to following the patrol points
        else
        {
            _state = State.Patrol;

            Patrol();
        }
    }

    // Follow the patrol path that has been designated
    // Goes to each point, waits the designated _patrolWaitTimer, then goes to next
    void Patrol()
    {
        if (patrolPath == null || patrolPath.PointCount == 0)
            return;

        // Does not set the next location untill it has actually reached the patrol point
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _patrolWaitTimer += Time.deltaTime;

            //Has waited out the timer
            if (_patrolWaitTimer >= patrolWaitTime)
            {
                GoToNextPatrolPoint();
                _patrolWaitTimer = 0f;
            }
        }
    }

    // Searches a specified area
    // Checks around nearby corners
    // Checks nearby hiding spots
    void SearchArea()
    {
        

    
    }

    int NearestPatrolPointIndex(Vector3 position)
    {
        if (patrolPath == null || patrolPath.PointCount == 0)
        return -1;

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
    
    // Sets the next patrol point based off the given PatrolPath
    void GoToNextPatrolPoint()
    {
        if (patrolPath == null || patrolPath.PointCount == 0)
            return;

        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPath.PointCount;

        Transform nextPoint = patrolPath.GetPoint(_currentPatrolIndex);
        _agent.SetDestination(nextPoint.position);
    }

    // Rotates towards a given direction
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

    //Checks if it can see the target
    //Returns true if yes, returns false if not
    bool CanSeeTarget()
    {
        
        Vector3 origin = transform.position + Vector3.up * 1.5f; //might not need this
        Vector3 directionToTarget = target.position - origin;

        float distanceToTarget = directionToTarget.magnitude;

        // target is too far
        if (distanceToTarget > sightRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToTarget.normalized);

        // target is in distance but outside of field of view
        if (angle > fieldOfView / 2f)
            return false;

        // target is behind an obstacle
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


        // make a switch function that sets the color based on what state the enemy is in


        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, target.position);
        }
    }
}