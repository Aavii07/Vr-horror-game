using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip taskCompleteSound;
    [Range(0f, 1f)]
    public float audioVolume = 0.7f;

    public TaskData taskToComplete;
    private bool triggered = false;
    
    // This is bool because we need to know if the task was in the UI (and should be completed) or should be ignored
    public bool CompleteThisTask() 
    {
        if (triggered) return false;
        
        TaskManager tm = TaskManager.Instance;
        if (tm != null && taskToComplete != null) 
        {
            if (tm.IsTaskActive(taskToComplete))
            {
                triggered = true;

                if (taskCompleteSound != null)
                {
                    // Create a temporary GameObject to play sound (more reliable)
                    GameObject soundObject = new GameObject("TempSound");
                    AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                    audioSource.clip = taskCompleteSound;
                    audioSource.volume = audioVolume;
                    audioSource.Play();
                    Destroy(soundObject, taskCompleteSound.length);
                }

                tm.CompleteTask(taskToComplete);
                return true;
            }
        }
        return false;
    }

    // Override in subclasses to auto-complete a task when it cycles in.
    // Only use if the condition is a persistent world state that can be re-checked (like door angle).
    public virtual bool IsAlreadyComplete()
    {
        return false;
    }
    
    // make sure to call this void func for anything interactable under
    // "xr grab interactable component -> interactable events -> select entered/exited/activate"
    // calling the bool func directly will cause an error
    public void OnInteract() 
    {
        CompleteThisTask();
    }
}