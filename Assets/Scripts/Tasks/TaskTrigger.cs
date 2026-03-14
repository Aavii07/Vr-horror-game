using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    public TaskData taskToComplete;
    public bool triggerOnce = true;
    private bool triggered = false;
    
    public void CompleteThisTask() 
    {
        if (triggerOnce && triggered) return;
        
        triggered = true;
        
        TaskManager tm = FindObjectOfType<TaskManager>();
        if (tm != null && taskToComplete != null) 
        {
            tm.CompleteTask(taskToComplete);
        }
    }
    
    public void OnGrab() 
    {
        CompleteThisTask();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) 
        {
            CompleteThisTask();
        }
    }
}