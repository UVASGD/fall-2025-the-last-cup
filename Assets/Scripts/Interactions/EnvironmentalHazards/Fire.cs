using UnityEngine;

public class Fire : MonoBehaviour
{
    private AudioSource fireLoopSource;

    void Start()
    {
        fireLoopSource = AudioManager.audioManagerInstance.PlayLoopingSFX(
            AudioManager.audioManagerInstance.fire
        );
    }

    void OnDestroy()
    {
        AudioManager.audioManagerInstance.StopLoopingSFX(fireLoopSource);
        fireLoopSource = null;
    }
}