using UnityEngine;

public class BuildRocket : TaskTrigger
{
    public GameObject thingToShow;
    public GameObject ThingToShow2;
    public GameObject ThingToShow3;
    public GameObject thingToHide;

    [Header("LightOnSound")]
    public AudioClip lightsOnSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    public void OnButtonClick()
    {
        thingToShow.SetActive(true);
        ThingToShow2.SetActive(true);
        ThingToShow3.SetActive(true);
        thingToHide.SetActive(false);

        GameObject soundObject = new GameObject("TempSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = lightsOnSound;
        audioSource.volume = soundVolume;
        audioSource.Play();
        Destroy(soundObject, lightsOnSound.length);

        CompleteThisTask();
    }
}