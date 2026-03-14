using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRTaskMenuToggle : MonoBehaviour
{
    private Canvas canvas;
    
    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    
    void Update()
    {
        // secondary button
        if (CheckSecondaryButton())
        {
            ToggleMenu();
        }
        
        // Simulator support
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleMenu();
        }
    }
    
    bool CheckSecondaryButton()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        
        foreach (InputDevice device in devices)
        {
            if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool sec) && sec)
                return true;
        }
        return false;
    }
    
    void ToggleMenu()
    {
        canvas.enabled = !canvas.enabled;
    }
}