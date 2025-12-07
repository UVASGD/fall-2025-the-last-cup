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
            doorOpenRotation = Quaternion.Euler(-90f, 90, 0f);
        }

        if (sprinklerParticles != null)
        {
            for (int i = 0; i < sprinklerParticles.Length; i++)
            {
                if (sprinklerParticles[i] != null)
                {
                    sprinklerParticles[i].Stop();
                }
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

            Transform matchingPipe = pipePuzzle.Find(pipeName);

            if (matchingPipe != null)
            {
                matchingPipe.gameObject.SetActive(true);
                numPipesFound++;

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
                if (sprinklerParticles[i] != null)
                {
                    sprinklerParticles[i].Play();
                }
            }
        }
        else
        {
            Debug.LogWarning("Sprinkler particle system references are missing!");
        }

        if (fires != null && fires.Length > 0)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                if (fires[i] != null)
                {
                    Destroy(fires[i]);
                }
            }
        }
        else
        {
            Debug.LogWarning("Fire references are missing or empty!");
        }
    }
}