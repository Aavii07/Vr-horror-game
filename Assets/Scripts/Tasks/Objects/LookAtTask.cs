using UnityEngine;

public class LookAtTask : MonoBehaviour
{
    // shows in the inspector
    public TaskTrigger taskTrigger;
    public Transform playerCamera;
    public float requiredLookTime = 4f;
    public float requiredDistance = 3f;
    // how accurate
    public float lookDotThreshold = 0.9f;
    private float lookTimer = 0f;
    private bool taskCompleted = false;

    void Update()
    {
        if (taskCompleted || playerCamera == null || taskTrigger == null)
            return;

        // calculate the distance
        float distanceToTarget = Vector3.Distance(playerCamera.position, transform.position);
        // calculate the angle
        Vector3 directionToTarget = (transform.position - playerCamera.position).normalized;
        float dot = Vector3.Dot(playerCamera.forward, directionToTarget);
        
        if (dot >= lookDotThreshold && distanceToTarget <= requiredDistance)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= requiredLookTime)
            {
                if (taskTrigger.CompleteThisTask())
                    taskCompleted = true;
                else
                    lookTimer = 0f; // Reset timer so player can try again once task cycles into UI
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }
}