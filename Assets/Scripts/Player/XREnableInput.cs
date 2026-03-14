using UnityEngine;
using UnityEngine.InputSystem;

public class XREnableInput : MonoBehaviour
{
    public InputActionAsset xrInputActions;

    void OnEnable()
    {
        xrInputActions.Enable();
    }

    void OnDisable()
    {
        xrInputActions.Disable();
    }
}