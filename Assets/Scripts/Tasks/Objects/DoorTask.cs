using UnityEngine;

public class DoorTask : MonoBehaviour
{
    public TaskTrigger taskTrigger;
    public float completionAngle = 45f;
    public bool checkOppositeDirection = false;
    
    private HingeJoint hingeJoint;
    private bool taskCompleted = false;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
    }

    void Update()
    {
        if (taskCompleted || hingeJoint == null) return;
        
        float currentAngle = hingeJoint.angle;
        float checkAngle = checkOppositeDirection ? Mathf.Abs(currentAngle) : currentAngle;
        
        if (checkAngle >= completionAngle)
        {
            taskCompleted = true;
            
            if (taskTrigger != null)
            {
                taskTrigger.CompleteThisTask();
            }
        }
    }
}