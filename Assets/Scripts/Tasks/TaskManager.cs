using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("Task Settings")]
    public int numberOfTasks = 3;
    public string taskFolderPath = "Tasks";
    public string finalTaskFolderPath = "Tasks Ending";
    public float taskCompletionDeleteDelay = 1f;

    [Header("Current Tasks")]
    public TaskData[] currentTasks;

    [Header("Priority Task")]
    public TaskData priorityTask; // tutorial task

    [Header("Wall to Move")]
    public GameObject wall;
    public float slideDuration = 20f;

    private TaskData[] allAvailableTasks;
    private TaskData[] finalTasks;
    private bool usingFinalTasks = false;

    void Awake()
    {
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

    void Start()
    {
        LoadAndSelectRandomTasks();
    }

    void LoadAndSelectRandomTasks()
    {
        allAvailableTasks = Resources.LoadAll<TaskData>(taskFolderPath);
        finalTasks = Resources.LoadAll<TaskData>(finalTaskFolderPath);

        // Reset all tasks
        foreach (TaskData task in allAvailableTasks)
        {
            task.completed = false;
        }

        SelectRandomTasks(numberOfTasks);
    }

    void SelectRandomTasks(int count)
    {
        List<TaskData> available = new List<TaskData>(allAvailableTasks);
        List<TaskData> selected = new List<TaskData>();

        // Add tutorial task first
        bool hasPriority = false;
        if (priorityTask != null && available.Contains(priorityTask))
        {
            selected.Add(priorityTask);
            available.Remove(priorityTask);
            hasPriority = true;
        }

        int remainingSlots = count - selected.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            if (available.Count == 0) break;

            int randomIndex = Random.Range(0, available.Count);
            selected.Add(available[randomIndex]);
            available.RemoveAt(randomIndex);
        }

        currentTasks = selected.ToArray();

        if (UIManager.Instance != null)
            UIManager.Instance.OnTasksLoaded(currentTasks);
    }

    public void CompleteTask(TaskData completedTask)
    {
        if (!completedTask.completed)
        {
            completedTask.completed = true;
            UIManager.Instance?.MarkComplete(completedTask);

            StartCoroutine(ReplaceTaskAfterDelay(completedTask, taskCompletionDeleteDelay));
        }
    }

    System.Collections.IEnumerator ReplaceTaskAfterDelay(TaskData completedTask, float delay)
    {
        yield return new WaitForSeconds(delay);

        TaskData newTask = GetRandomIncompleteTask();
        List<TaskData> updatedTasks = new List<TaskData>();

        if (newTask != null)
        {
            // Replace the completed task with new one
            for (int i = 0; i < currentTasks.Length; i++)
            {
                if (currentTasks[i] == completedTask)
                {
                    newTask.completed = false;
                    updatedTasks.Add(newTask);

                    TaskTrigger trigger = FindTaskTriggerFor(newTask);
                    if (trigger != null && trigger.IsAlreadyComplete())
                        StartCoroutine(AutoCompleteNewTask(newTask));
                }
                else
                {
                    updatedTasks.Add(currentTasks[i]);
                }
            }
        }
        else
        {
            // No new task available - just remove the completed one
            for (int i = 0; i < currentTasks.Length; i++)
            {
                if (currentTasks[i] != completedTask)
                {
                    updatedTasks.Add(currentTasks[i]);
                }
            }
        }

        // Update currentTasks array
        currentTasks = updatedTasks.ToArray();

        // Update UI
        if (UIManager.Instance != null)
        {
            if (currentTasks.Length > 0)
            {
                UIManager.Instance.RefreshChecklist(currentTasks);
            }
            else
            {
                UIManager.Instance.ClearChecklist(); // All tasks done
            }
        }
    }

    TaskData GetRandomIncompleteTask()
    {
        bool allRegularTasksCompleted = true;
        if (!usingFinalTasks)
        {
            foreach (TaskData task in allAvailableTasks)
            {
                if (!task.completed)
                {
                    allRegularTasksCompleted = false;
                    break;
                }
            }
        }

        // Final Tasks - not randomized
        if (allRegularTasksCompleted && !usingFinalTasks && finalTasks.Length > 0)
        {
            usingFinalTasks = true;
            allAvailableTasks = finalTasks;

            StartCoroutine(SlideWallDown());

            foreach (TaskData task in allAvailableTasks)
            {
                task.completed = false;
            }

            List<TaskData> finalTasksList = new List<TaskData>();
            foreach (TaskData task in allAvailableTasks)
            {
                if (!task.completed)
                {
                    finalTasksList.Add(task);
                }
            }

            currentTasks = finalTasksList.ToArray();

            UIManager.Instance?.RefreshChecklist(currentTasks);

            return null;
        }

        // Regular tasks - random selection
        List<TaskData> availableTasks = new List<TaskData>();

        foreach (TaskData task in allAvailableTasks)
        {
            if (!task.completed && !currentTasks.Contains(task))
            {
                availableTasks.Add(task);
            }
        }

        if (availableTasks.Count == 0)
            return null;

        int randomIndex = Random.Range(0, availableTasks.Count);
        return availableTasks[randomIndex];
    }

    public bool IsTaskActive(TaskData task)
    {
        return System.Array.Exists(currentTasks, t => t == task);
    }

    TaskTrigger FindTaskTriggerFor(TaskData task)
    {
        foreach (TaskTrigger trigger in FindObjectsOfType<TaskTrigger>())
        {
            if (trigger.taskToComplete == task)
                return trigger;
        }
        return null;
    }

    System.Collections.IEnumerator AutoCompleteNewTask(TaskData task)
    {
        yield return null;
        CompleteTask(task);
    }

    public void ResetAllTasks()
    {
        foreach (TaskData task in currentTasks)
        {
            task.completed = false;
        }
    }
    
    // Opens up area to launchpad
    IEnumerator SlideWallDown()
    {
        Debug.Log("Wall is Down");
        if (wall == null) yield break;
        
        Vector3 startPosition = wall.transform.position;
        Vector3 targetPosition = startPosition;
        targetPosition.y = -10f;
        
        float elapsed = 0;
        
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            
            wall.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }
        
        Vector3 finalPos = wall.transform.position;
        finalPos.y = -10f;
        wall.transform.position = finalPos;
    }
}