using UnityEngine;

// only for Keyboard testing, need to be changed when using vr controller
public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    private bool isOn = true;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}