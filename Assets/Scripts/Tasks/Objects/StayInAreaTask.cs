using UnityEngine;

public class StayInAreaTask : TaskTrigger
{
    public float requiredStayTime = 3f;
    private float stayTimer = 0f;
    private bool taskCompleted = false;

    void OnTriggerStay(Collider other)
    {
        if (taskCompleted)
            return;

        // if has the player tag
        if (other.CompareTag("Player"))
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= requiredStayTime)
            {
                if (CompleteThisTask())
                    taskCompleted = true;
                else
                    stayTimer = 0f;
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