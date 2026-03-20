using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using XRDevice = UnityEngine.XR.InputDevice;
using XRCommon = UnityEngine.XR.CommonUsages;

public class SimpleMenuToggle : MonoBehaviour
{
    private Canvas canvas;
    private XRDevice leftController;
    private XRDevice rightController;
    
    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    
    void Start()
    {
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }
    
    void Update()
    {
        if (leftController.isValid && 
            leftController.TryGetFeatureValue(XRCommon.menuButton, out bool menuPressed) && 
            menuPressed)
        {
            ToggleMenu();
        }
        
        if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }
    
    void ToggleMenu()
    {
        if (canvas != null)
        {
            canvas.enabled = !canvas.enabled;
            Debug.Log($"Menu toggled: {canvas.enabled}");
        }
    }
}