using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointScript : MonoBehaviour
{
    private RespawnScript[] allRespawnScripts;
    private BoxCollider boxCollider;
    public int checkpointIndex = 1;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        allRespawnScripts = FindObjectsByType<RespawnScript>(FindObjectsSortMode.None);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (RespawnScript respawn in allRespawnScripts)
            {
                respawn.respawnPoint = this.gameObject;
            }

            if (SaveLoadCheckpoint.Instance != null)
            {
                SaveLoadCheckpoint.Instance.SaveCheckpoint(
                    SceneManager.GetActiveScene().name,
                    checkpointIndex,
                    transform.position,
                    gameObject.name
                );
            }

            boxCollider.enabled = false;
        }
    }
}