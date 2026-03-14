using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance;

    public GameObject togglePrefab;
    public Transform checklistParent;

    Dictionary<TaskData, Toggle> toggleMap = new Dictionary<TaskData, Toggle>();

    void Awake() 
    {
        Instance = this;
    }

    void Start() 
    {
        TaskManager tm = FindObjectOfType<TaskManager>();
        
        if (tm != null && tm.tasks != null)
        {
            // Force reset of all tasks before creating UI
            foreach (TaskData task in tm.tasks)
            {
                task.completed = false;
            }
            
            CreateChecklist(new List<TaskData>(tm.tasks));
        }
    }

    public void CreateChecklist(List<TaskData> tasks) 
    {
        foreach (Transform child in checklistParent) 
        {
            Destroy(child.gameObject);
        }
        
        toggleMap.Clear();

        foreach(TaskData task in tasks) 
        {
            GameObject toggleObj = Instantiate(togglePrefab, checklistParent);
            
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            
            // Find the TaskName label in the Toggle prefab
            Transform nameTransform = toggle.transform.Find("TaskName");
            if (nameTransform != null)
            {
                Text nameText = nameTransform.GetComponent<Text>();
                if (nameText != null)
                {
                    nameText.text = task.taskName;
                }
            }
            
            toggle.isOn = task.completed;
            toggleMap.Add(task, toggle);
        }
    }

    public void MarkComplete(TaskData task) 
    {
        if (toggleMap.ContainsKey(task)) 
        {
            toggleMap[task].isOn = true;
        } 
        else 
        {
            Debug.LogWarning($"Task {task.taskName} not found in UI map");
        }
    }
}