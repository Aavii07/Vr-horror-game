using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    public TaskData taskToComplete;
    private bool triggered = false;
    
    public void CompleteThisTask() 
    {
        if (triggered) return;
        
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
}