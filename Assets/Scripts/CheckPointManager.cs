using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void PlayFromLastCheckpoint()
    {
        if (SaveLoadCheckpoint.Instance != null && SaveLoadCheckpoint.Instance.HasSavedCheckpoint())
        {
            SaveLoadCheckpoint.Instance.LoadCheckpointAndStartGame();
        }
        else
        {
            LoadScene(1);
        }
    }
}