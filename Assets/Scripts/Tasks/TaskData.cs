using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "VRTasks/Task")]
public class TaskData : ScriptableObject 
{
    public string taskName;
    public bool completed;  
}