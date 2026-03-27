using UnityEngine;

public class DoorTask : TaskTrigger
{
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
            CompleteThisTask();
        }
    }

    public override bool IsAlreadyComplete()
    {
        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>();

        float checkAngle = checkOppositeDirection ? Mathf.Abs(hingeJoint.angle) : hingeJoint.angle;
        return checkAngle >= completionAngle;
    }
}