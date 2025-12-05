using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    private void loadScene()
    {
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
        {
            Debug.LogWarning(sceneName + " doesnt Exists");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadScene();
        }

    }
}
