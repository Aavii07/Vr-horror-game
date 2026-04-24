using UnityEngine;
using System.Collections;

public class BoardRocket : TaskTrigger
{
    public GameObject playerXR;
    public Camera rocketCamera;
    public GameObject thingToHide;
    public GameObject thingToShow;

    public Transform rocketRoot;

    public AudioClip TakeOffSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    private AudioSource rocketAudio;

    public float startSpeed = 2f;
    public float acceleration = 1.5f;

    private float currentSpeed;

    public void SwitchToRocket()
    {
        playerXR.SetActive(false);

        rocketCamera.gameObject.SetActive(true);
        rocketCamera.enabled = true;

        thingToHide.SetActive(false);
        thingToShow.SetActive(true);

        // takeoff audio
        rocketAudio = rocketRoot.gameObject.AddComponent<AudioSource>();
        rocketAudio.clip = TakeOffSound;
        rocketAudio.volume = soundVolume;
        rocketAudio.spatialBlend = 1f;
        rocketAudio.Play();

        StartCoroutine(LaunchRocket());
        CompleteThisTask();
    }

    IEnumerator LaunchRocket()
    {
        yield return new WaitForSeconds(1.0f);

        currentSpeed = startSpeed;

        while (true)
        {
            rocketRoot.position += Vector3.up * currentSpeed * Time.deltaTime;
            currentSpeed += acceleration * Time.deltaTime;

            yield return null;
        }
    }
}