using UnityEngine;

public class PipeDropoff : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private Transform pipePuzzle;
    [SerializeField] private int numPipesFound = 0;
    [SerializeField] private int totalPipesNeeded = 4;

    [Header("Door Settings")]
    [SerializeField] private Transform exitDoor;

    [Header("Sprinkler Settings")]
    [SerializeField] private ParticleSystem[] sprinklerParticles;
    [SerializeField] private GameObject[] fires;

    private bool puzzleCompleted = false;
    private bool doorIsOpening = false;
    private Quaternion doorOpenRotation;

    private void Start()
    {
        if (exitDoor != null)
        {
            // Modified this line below for door in playground scene, but can be modified for another scene
            doorOpenRotation = Quaternion.Euler(-90f, 90, 0f);
        }

        if (sprinklerParticles != null)
        {
            for (int i = 0; i < sprinklerParticles.Length; i++)
            {
                sprinklerParticles[i].Stop();
            }
        }
        else
        {
            Debug.LogWarning("Sprinkler particle system reference is missing!");
        }
    }

    private void Update()
    {
        if (doorIsOpening && exitDoor != null)
        {
            exitDoor.rotation = doorOpenRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (puzzleCompleted) return;

        if (other.CompareTag("Pipe"))
        {
            string pipeName = other.gameObject.name;
            // Debug.Log($"Pipe detected: {pipeName}");

            Transform matchingPipe = pipePuzzle.Find(pipeName);

            if (matchingPipe != null)
            {
                matchingPipe.gameObject.SetActive(true);
                numPipesFound++;

                // Debug.Log($"Activated {pipeName}. Total pipes found: {numPipesFound}/{totalPipesNeeded}");

                Destroy(other.gameObject);

                if (numPipesFound >= totalPipesNeeded)
                {
                    OnPuzzleComplete();
                }
            }
            else
            {
                Debug.LogWarning($"No matching pipe found in PipePuzzle for: {pipeName}");
            }
        }
    }

    private void OnPuzzleComplete()
    {
        // Debug.Log("Pipe puzzle completed!");
        puzzleCompleted = true;

        OpenDoor();
        ActivateSprinkler();
    }

    private void OpenDoor()
    {
        if (exitDoor != null)
        {
            AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.door);
            doorIsOpening = true;
        }
        else
        {
            Debug.LogWarning("Exit door reference is missing!");
        }
    }

    private void ActivateSprinkler()
    {
        if (sprinklerParticles != null)
        {
            AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.sprinklers);
            for (int i = 0; i < sprinklerParticles.Length; i++)
            {
                sprinklerParticles[i].Play();
            }
        }
        else
        {
            Debug.LogWarning("Sprinkler particle system references are missing!");
        }

        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                // Destroy fire rather than have set to inactive
                Destroy(fires[i]);
            }
        }
        else
        {
            Debug.LogWarning("Fire references are missing!");
        }
    }
}