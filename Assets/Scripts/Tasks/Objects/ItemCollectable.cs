using UnityEngine;

public class ItemCollectable : TaskTrigger
{
    private bool isCollected = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        
        if (other.name == "CollectionSphere" || other.name == "TestingCollectionSphere")
        {
            // Check if the task is currently active in the UI before allowing collection
            if (!IsTaskActive())
            {
                return;
            }
            
            Collect();
        }
    }
    
    bool IsTaskActive()
    {
        if (taskToComplete == null) return false;
        if (TaskManager.Instance == null) return false;
        
        return TaskManager.Instance.IsTaskActive(taskToComplete);
    }
    
    void Collect()
    {
        isCollected = true;
        
        CompleteThisTask();
        
        Destroy(gameObject);
    }
}