using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeLayerOnGrab : MonoBehaviour
{
    private int originalLayer;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnPickup);
        grabInteractable.selectExited.AddListener(OnDrop);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnPickup);
        grabInteractable.selectExited.RemoveListener(OnDrop);
    }

    void OnPickup(SelectEnterEventArgs args)
    {
        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("HeldItem");
    }

    void OnDrop(SelectExitEventArgs args)
    {
        gameObject.layer = originalLayer;
    }
}