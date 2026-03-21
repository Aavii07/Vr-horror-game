using UnityEngine;
using UnityEngine.XR;

public class HandFlipMenuToggle : MonoBehaviour
{
    public Canvas taskCanvas;
    public XRNode hand = XRNode.RightHand;
    public float flipThreshold = 90f;
    
    private InputDevice device;
    private bool wasFlipped = false;
    
    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(hand);
    }
    
    void Update()
    {
        if (!device.isValid) return;
        
        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            // Check angle
            Vector3 palmUpDirection = rotation * Vector3.down;
            float angle = Vector3.Angle(Vector3.down, palmUpDirection);
            bool isFlipped = angle > flipThreshold;
            
            // Toggle when flip state changes
            if (isFlipped != wasFlipped)
            {
                wasFlipped = isFlipped;
                
                if (taskCanvas != null)
                    taskCanvas.enabled = isFlipped;
            }
        }
    }
}