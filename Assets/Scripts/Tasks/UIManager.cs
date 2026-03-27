using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    public void OnTasksLoaded(TaskData[] loadedTasks)
    {
        if (loadedTasks == null || loadedTasks.Length == 0) return;     
        CreateChecklist(new List<TaskData>(loadedTasks));
    }
    
    public void RefreshChecklist(TaskData[] updatedTasks)
    {
        foreach (Transform child in checklistParent) 
            Destroy(child.gameObject);
        
        toggleMap.Clear();
        
        if (updatedTasks == null || updatedTasks.Length == 0)
        {
            Debug.Log("No tasks to display - checklist cleared");
            return;
        }
        
        foreach(TaskData task in updatedTasks) 
        {
            GameObject toggleObj = Instantiate(togglePrefab, checklistParent);
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            
            Transform nameTransform = toggle.transform.Find("TaskName");
            if (nameTransform != null)
            {
                TextMeshProUGUI tmpText = nameTransform.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = task.taskName;
                else
                {
                    Text regularText = nameTransform.GetComponent<Text>();
                    if (regularText != null)
                        regularText.text = task.taskName;
                }
            }
            
            toggle.isOn = task.completed;
            toggleMap.Add(task, toggle);
        }
    }
    
    public void ClearChecklist()
    {
        foreach (Transform child in checklistParent) 
            Destroy(child.gameObject);
        
        toggleMap.Clear();
        Debug.Log("Checklist cleared");
    }

    public void CreateChecklist(List<TaskData> tasks) 
    {
        foreach (Transform child in checklistParent) 
            Destroy(child.gameObject);
        
        toggleMap.Clear();

        foreach(TaskData task in tasks) 
        {
            GameObject toggleObj = Instantiate(togglePrefab, checklistParent);
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            
            Transform nameTransform = toggle.transform.Find("TaskName");
            if (nameTransform != null)
            {
                TextMeshProUGUI tmpText = nameTransform.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = task.taskName;
                else
                {
                    Text regularText = nameTransform.GetComponent<Text>();
                    if (regularText != null)
                        regularText.text = task.taskName;
                }
            }
            
            toggle.isOn = task.completed;
            toggleMap.Add(task, toggle);
        }
    }

    public void MarkComplete(TaskData task) 
    {
        if (toggleMap.ContainsKey(task)) 
            toggleMap[task].isOn = true;
    }
}