using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnDelay = 0.1f;

    private bool isRespawning = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void KillPlayer(GameObject player)
    {
        if (isRespawning) return;
        
        StartCoroutine(RespawnCoroutine(player));
    }

    IEnumerator RespawnCoroutine(GameObject player)
    {
        isRespawning = true;
        
        yield return new WaitForSeconds(respawnDelay);
        
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
        }
        
        isRespawning = false;
    }
}