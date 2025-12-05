using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnScript : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnPoint;

    private void Start()
    {
        if (SaveLoadCheckpoint.Instance != null && SaveLoadCheckpoint.Instance.HasSavedCheckpoint())
        {
            CheckpointData checkpoint = SaveLoadCheckpoint.Instance.LoadLastCheckpoint();
            if (checkpoint != null && checkpoint.sceneName == SceneManager.GetActiveScene().name)
            {
                GameObject savedCheckpoint = GameObject.Find(checkpoint.checkpointName);
                if (savedCheckpoint != null)
                {
                    respawnPoint = savedCheckpoint;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = respawnPoint.transform.position;
                controller.enabled = true;
            }
        }
    }
}