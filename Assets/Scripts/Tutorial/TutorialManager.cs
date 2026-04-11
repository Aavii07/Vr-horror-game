using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public GameObject targetCanvas;

    [Header("Tutorial Content")]
    public string[] messages = new string[]
    {
        "Welcome. To begin, please bend your left wrist toward your face to view the task list. Press OK once you figure it out.",
        "These are your tasks to complete. Try to get them all done.",
        "Be careful about the monster roaming around. Don't let it catch you.",
        "Good luck."
    };

    [Header("Walls to Move")]
    public GameObject[] walls;
    public float slideDuration = 20f;

    private int currentIndex = 0;
    
    void Start()
    {
        tutorialText.text = messages[0];
        tutorialPanel.SetActive(true);
        nextButton.onClick.AddListener(OnNextPressed);
        
        TextMeshProUGUI btnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = "OK";
    }
    
    public void OnNextPressed()
    {
        currentIndex++;
        
        if (currentIndex < messages.Length)
        {
            tutorialText.text = messages[currentIndex];
        }
        else
        {
            StartCoroutine(SlideWallsDown());
            StartCoroutine(FadeOutTutorial());
        }
    }
    
    IEnumerator SlideWallsDown()
    {
        Vector3[] startPositions = new Vector3[walls.Length];
        Vector3[] targetPositions = new Vector3[walls.Length];
        
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                startPositions[i] = walls[i].transform.position;
                targetPositions[i] = startPositions[i];
                targetPositions[i].y = -10f;
            }
        }
        
        float elapsed = 0;
        
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            
            for (int i = 0; i < walls.Length; i++)
            {
                if (walls[i] != null)
                {
                    Vector3 newPos = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                    walls[i].transform.position = newPos;
                }
            }
            
            yield return null;
        }
        
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                Vector3 finalPos = walls[i].transform.position;
                finalPos.y = -10f;
                walls[i].transform.position = finalPos;
            }
        }
    }
    
    IEnumerator FadeOutTutorial()
    {
        CanvasGroup canvasGroup = targetCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = targetCanvas.AddComponent<CanvasGroup>();
        
        float duration = 0.5f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsed / duration);
            yield return null;
        }
        
        targetCanvas.SetActive(false);
    }
}