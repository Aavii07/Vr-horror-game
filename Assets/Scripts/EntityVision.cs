using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EntityVision : MonoBehaviour
{

    public static EntityVision Instance { get; private set; }

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
    [SerializeField] private float patrolWaitTime = 25;

    [Header("Search")]
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private int locationsToCheckUpper = 5;
    [SerializeField] private int locationsToCheckLower = 2;
    [SerializeField] private float searchDuration = 4f;

    [Header("Audio")]
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioSource ScreamAudioSource;

    [SerializeField] private AudioClip walkingSound;
    [SerializeField] private AudioClip ScreamSound;





    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _lastKnownPosition;

    private int _currentPatrolIndex;
    private float _patrolWaitTimer;
    private float _searchTimer;

    private static readonly int SpeedParam    = Animator.StringToHash("Speed");
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
    private static readonly int AttackParam   = Animator.StringToHash("Attack");

    private enum State { Patrol, Chasing, Investigating, Searching }
    private State _state = State.Patrol;

    private bool _isScreaming = false;
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

        if (_isScreaming)
        {
            UpdateAnimations();
            HandleWalkingAudio();
            return;
        }


        if (CanSeeTarget())
        {
            _lastKnownPosition = target.position;

            if (_state != State.Chasing && !_isScreaming 
                && ScreamAudioSource != null && ScreamSound != null)
            {
                Debug.Log("Triggering scream coroutine");
                StartCoroutine(ScreamThenChase());
            }
            else if (!_isScreaming)
            {
                Debug.Log($"Chasing — speed: {_agent.speed}, pathStatus: {_agent.pathStatus}, remainingDist: {_agent.remainingDistance}");
                _state = State.Chasing;
                _agent.speed = chaseSpeed;
                _agent.SetDestination(_lastKnownPosition);
                RotateTowards(_lastKnownPosition);
            }
        }

        // Just lost sight of target go to last known position
        else if (_state == State.Chasing && !CanSeeTarget())
        {
            _state = State.Investigating;
            _agent.speed = patrolSpeed;
            _agent.SetDestination(_lastKnownPosition);
        }

        
        else if (_state == State.Investigating)
        {
            RotateTowards(_lastKnownPosition);

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                // Reached last known position — begin searching the area
                _state = State.Patrol;
                _currentPatrolIndex = NearestPatrolPointIndex(transform.position);
                _agent.SetDestination(patrolPath.GetPoint(_currentPatrolIndex).position);
                _searchTimer = searchDuration;
                _agent.ResetPath();
            }
        }

        else if (_state == State.Searching)
        {
            _searchTimer -= Time.deltaTime;
           // SearchArea();

            if (_searchTimer <= 0f)
            {
 
                _patrolWaitTimer = 0f;
                _currentPatrolIndex = NearestPatrolPointIndex(transform.position);
                _agent.SetDestination(patrolPath.GetPoint(_currentPatrolIndex).position);
                _state = State.Patrol;
            }
        }

        else
        {
            _state = State.Patrol;
            _agent.speed = patrolSpeed;
            Patrol();
        }

        UpdateAnimations();
        HandleWalkingAudio();
    }

    private IEnumerator ScreamThenChase()
    {
        Debug.Log("ScreamThenChase START");
        _isScreaming = true;
        _state = State.Chasing;
        _agent.ResetPath();

        ScreamAudioSource.clip = ScreamSound;
        ScreamAudioSource.Play();

        yield return new WaitForSeconds(ScreamSound.length);

        Debug.Log($"ScreamThenChase END — agent enabled: {_agent.enabled}, isOnNavMesh: {_agent.isOnNavMesh}, pathStatus: {_agent.pathStatus}");
        
        _isScreaming = false;
        _agent.speed = chaseSpeed;
        bool result = _agent.SetDestination(_lastKnownPosition);
        Debug.Log($"SetDestination result: {result}, lastKnownPos: {_lastKnownPosition}");
    }

    void UpdateAnimations()
    {
        switch (_state)
        {
            case State.Patrol:

                if(_patrolWaitTimer > 0)
                {
                   _animator.SetBool(IsMovingParam, false); 
                }
                else
                {
                    _animator.SetBool(IsMovingParam, true);
                }
                break;

            case State.Chasing:
                if (_isScreaming)
                {
                    _animator.SetBool(IsMovingParam, false);
                }
                else
                {
                    _animator.SetBool(IsMovingParam, true);
                }
                
                break;

            case State.Investigating:
                _animator.SetBool(IsMovingParam, true);
                break;

            case State.Searching:
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


    private int _searchLocationsChecked;
    private bool _searchMoving;

    void SearchArea()
    {
        // Determine how many locations to visit this search session
        int targetLocations = Random.Range(locationsToCheckLower, locationsToCheckUpper + 1);

        if (_searchLocationsChecked >= targetLocations) return;

        // If we're not currently moving to a search point, pick a new one
        if (!_searchMoving)
        {
            Vector2 randomCircle = Random.insideUnitCircle * searchRadius;
            Vector3 candidate    = _lastKnownPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            {
                _agent.speed = patrolSpeed;
                _agent.SetDestination(hit.position);
                _searchMoving = true;
            }
        }
        else if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            // Reached the search point — count it and look for the next
            _searchLocationsChecked++;
            _searchMoving = false;
        }
    }

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

    void HandleWalkingAudio()
    {
        if (walkingAudioSource == null || walkingSound == null) return;

        bool isMoving = !_agent.pathPending && _agent.remainingDistance > 0.2f;

        if (isMoving)
        {
            if (!walkingAudioSource.isPlaying)
            {
                walkingAudioSource.clip = walkingSound;
                walkingAudioSource.loop = true;
                walkingAudioSource.Play();
            }
        }
        else
        {
            if (walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }
        }
    }

    public void PlayerCollectsItem()
    {
        if (_state != State.Chasing)
        {
            _state = State.Investigating;
            _agent.speed = patrolSpeed;
            _agent.SetDestination(target.position);
        }
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

        if (_state == State.Searching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_lastKnownPosition, searchRadius);
        }

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, target.position);
        }


        // UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, _state.ToString());
    }
}