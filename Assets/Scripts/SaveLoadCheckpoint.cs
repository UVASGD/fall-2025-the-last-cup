using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadCheckpoint : MonoBehaviour
{
    private const string LAST_CHECKPOINT_SCENE = "LastCheckpointScene";
    private const string LAST_CHECKPOINT_INDEX = "LastCheckpointIndex";
    private const string LAST_CHECKPOINT_POS_X = "LastCheckpointPosX";
    private const string LAST_CHECKPOINT_POS_Y = "LastCheckpointPosY";
    private const string LAST_CHECKPOINT_POS_Z = "LastCheckpointPosZ";
    private const string LAST_CHECKPOINT_NAME = "LastCheckpointName";

    public static SaveLoadCheckpoint Instance { get; private set; }

    private CheckpointData currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCheckpoint(string sceneName, int checkpointIndex, Vector3 position, string checkpointName)
    {
        currentCheckpoint = new CheckpointData(sceneName, checkpointIndex, position, checkpointName);

        PlayerPrefs.SetString(LAST_CHECKPOINT_SCENE, sceneName);
        PlayerPrefs.SetInt(LAST_CHECKPOINT_INDEX, checkpointIndex);
        PlayerPrefs.SetFloat(LAST_CHECKPOINT_POS_X, position.x);
        PlayerPrefs.SetFloat(LAST_CHECKPOINT_POS_Y, position.y);
        PlayerPrefs.SetFloat(LAST_CHECKPOINT_POS_Z, position.z);
        PlayerPrefs.SetString(LAST_CHECKPOINT_NAME, checkpointName);
        PlayerPrefs.Save();
    }

    public CheckpointData LoadLastCheckpoint()
    {
        if (!HasSavedCheckpoint())
        {
            return null;
        }

        string sceneName = PlayerPrefs.GetString(LAST_CHECKPOINT_SCENE);
        int checkpointIndex = PlayerPrefs.GetInt(LAST_CHECKPOINT_INDEX);
        Vector3 position = new Vector3(
            PlayerPrefs.GetFloat(LAST_CHECKPOINT_POS_X),
            PlayerPrefs.GetFloat(LAST_CHECKPOINT_POS_Y),
            PlayerPrefs.GetFloat(LAST_CHECKPOINT_POS_Z)
        );
        string checkpointName = PlayerPrefs.GetString(LAST_CHECKPOINT_NAME);

        return new CheckpointData(sceneName, checkpointIndex, position, checkpointName);
    }

    public bool HasSavedCheckpoint()
    {
        return PlayerPrefs.HasKey(LAST_CHECKPOINT_SCENE);
    }

    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_SCENE);
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_INDEX);
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_POS_X);
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_POS_Y);
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_POS_Z);
        PlayerPrefs.DeleteKey(LAST_CHECKPOINT_NAME);
        PlayerPrefs.Save();
        currentCheckpoint = null;
    }

    public void LoadCheckpointAndStartGame()
    {
        CheckpointData checkpoint = LoadLastCheckpoint();
        if (checkpoint != null)
        {
            SceneManager.LoadScene(checkpoint.sceneName);
        }
    }

    public void LoadSpecificCheckpoint(string sceneName, int checkpointIndex)
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("CheckPoint");

        foreach (GameObject cp in checkpoints)
        {
            if (cp.name.Contains(checkpointIndex.ToString()))
            {
                SaveCheckpoint(sceneName, checkpointIndex, cp.transform.position, cp.name);
                SceneManager.LoadScene(sceneName);
                return;
            }
        }

        SceneManager.LoadScene(sceneName);
    }
}