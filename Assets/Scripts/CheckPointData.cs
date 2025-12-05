using UnityEngine;

[System.Serializable]
public class CheckpointData
{
    public string sceneName;
    public int checkpointIndex;
    public Vector3 position;
    public string checkpointName;

    public CheckpointData(string sceneName, int checkpointIndex, Vector3 position, string checkpointName)
    {
        this.sceneName = sceneName;
        this.checkpointIndex = checkpointIndex;
        this.position = position;
        this.checkpointName = checkpointName;
    }
}