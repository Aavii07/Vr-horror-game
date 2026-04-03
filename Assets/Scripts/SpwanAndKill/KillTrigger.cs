using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.transform.root.gameObject;
            RespawnManager.Instance?.KillPlayer(player);
        }
    }
}