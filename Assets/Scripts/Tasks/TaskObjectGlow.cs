using UnityEngine;

public class TaskObjectGlow : MonoBehaviour
{
    [Header("Task Association")]
    public TaskData associatedTask;
    
    [Header("Light Settings")]
    public Color lightColor = Color.red;
    public float lightIntensity = 2f;
    public float lightRange = 3f;
    
    private Light objectLight;
    private bool isGlowing = false;

    void Start()
    {
        // Adding a light component
        objectLight = gameObject.AddComponent<Light>();
        objectLight.type = LightType.Point;
        objectLight.color = lightColor;
        objectLight.intensity = 0;
        objectLight.range = lightRange;
        objectLight.shadows = LightShadows.None; // Better performance
        
        CheckIfShouldGlow();
    }
    
        void CheckIfShouldGlow()
    {
        if (TaskManager.Instance != null && associatedTask != null)
        {
            bool isActive = TaskManager.Instance.IsTaskActive(associatedTask);
            SetGlow(isActive);
        }
    }
    
    public void SetGlow(bool glow)
    {
        if (objectLight == null) return;

        isGlowing = glow;
        objectLight.intensity = glow ? lightIntensity : 0;
    }
}