using UnityEngine;

public class StayInAreaTask : MonoBehaviour
{
    public TaskTrigger taskTrigger;
    public float requiredStayTime = 3f;
    private float stayTimer = 0f;
    private bool taskCompleted = false;

    void OnTriggerStay(Collider other)
    {
        if (taskCompleted || taskTrigger == null)
            return;

        // if has the player tag
        if (other.CompareTag("Player"))
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= requiredStayTime)
            {
                taskCompleted = true;
                taskTrigger.CompleteThisTask();
            }
        }
    }

    // when the player leaves the area
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stayTimer = 0f;
        }
    }
}