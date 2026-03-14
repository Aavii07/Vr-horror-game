using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public TaskData[] tasks;
    public static TaskManager Instance { get; private set; }

    void Awake()
    {
        // Safety to delete accidental duplicate task managers
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteTask(TaskData task)
    {
        if (!task.completed)
        {
            task.completed = true;
            Debug.Log(task.taskName + " completed");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.MarkComplete(task);
            }
        }
    }
    
    public void ResetAllTasks()
    {
        foreach (TaskData task in tasks)
        {
            task.completed = false;
        }
    }
}