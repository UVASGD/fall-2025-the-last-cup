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
            StartCoroutine(RespawnPlayer());
        }
    }

    private System.Collections.IEnumerator RespawnPlayer()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            Debug.Log("========== RESPAWN STARTED ==========");

            DeathScreen.StartFade();

            Debug.Log("[RespawnScript] Setting ignore triggers to TRUE");
            player.SendMessage("SetIgnoreTriggers", true, SendMessageOptions.DontRequireReceiver);

            Debug.Log("[RespawnScript] Calling ClearConveyor...");
            player.SendMessage("ClearConveyor", SendMessageOptions.DontRequireReceiver);

            Debug.Log("[RespawnScript] Removing player from all conveyors...");
            RemovePlayerFromAllConveyors();

            Debug.Log("[RespawnScript] Disabling CharacterController...");
            controller.enabled = false;

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Debug.Log($"[RespawnScript] Clearing rigidbody velocities. Current linear: {playerRb.linearVelocity}, angular: {playerRb.angularVelocity}");
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }

            Debug.Log($"[RespawnScript] Teleporting from {player.transform.position} to {respawnPoint.transform.position}");
            player.transform.position = respawnPoint.transform.position;
            player.transform.rotation = respawnPoint.transform.rotation;

            Collider[] overlappingColliders = Physics.OverlapSphere(respawnPoint.transform.position, 2f);
            foreach (Collider col in overlappingColliders)
            {
                if (col.CompareTag("ConveyorBelt"))
                {
                    Debug.LogError($"[RespawnScript] WARNING: Respawn point '{respawnPoint.name}' is INSIDE conveyor belt trigger: {col.name}!");
                }
            }

            Debug.Log("[RespawnScript] Waiting for FixedUpdate...");
            yield return new WaitForFixedUpdate();

            Debug.Log("[RespawnScript] Calling ResetAllMovement...");
            player.SendMessage("ResetAllMovement", SendMessageOptions.DontRequireReceiver);

            Debug.Log("[RespawnScript] Re-enabling CharacterController...");
            controller.enabled = true;

            yield return new WaitForSeconds(0.5f);

            Debug.Log("[RespawnScript] Setting ignore triggers to FALSE");
            player.SendMessage("SetIgnoreTriggers", false, SendMessageOptions.DontRequireReceiver);

            Debug.Log("========== RESPAWN COMPLETED ==========");
        }
    }

    private void RemovePlayerFromAllConveyors()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            ConveyorBelt[] allConveyors = FindObjectsByType<ConveyorBelt>(FindObjectsSortMode.None);
            Debug.Log($"[RespawnScript] Found {allConveyors.Length} conveyor belts in scene");

            foreach (ConveyorBelt conveyor in allConveyors)
            {
                Debug.Log($"[RespawnScript] Removing player from conveyor: {conveyor.name}");
                conveyor.RemoveRigidbody(playerRb);
            }
        }
        else
        {
            Debug.LogError("[RespawnScript] Cannot remove player from conveyors - player has no Rigidbody!");
        }
    }
}