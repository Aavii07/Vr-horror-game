using UnityEngine;
using UnityEngine.AI;
 
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{

    [SerializeField] private float speed = 7f;
    private NavMeshAgent _agent;
    private Camera _cam;
 
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _cam = Camera.main;
        _agent.speed = speed;
    }
 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
 
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.SetDestination(hit.point);
            }
        }
    }
}
 