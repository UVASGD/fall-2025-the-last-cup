using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem.XR;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioMixer audioMixer;

    [Header("Audio Clips")]
    [Header("Menus")]
    public AudioClip menuBackground;
    public AudioClip button;

    [Header("Level Music")]
    public AudioClip section1BackgroundMusic;
    // public AudioClip section2BackgroundMusic;
    // public AudioClip section3BackgroundMusic;
    // public AudioClip section4BackgroundMusic;
    public AudioClip creditsBackgroundMusic;

    [Header("Movement and Mechanics")]
    public AudioClip[] footsteps;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public AudioClip rotate;
    public AudioClip scoopable;
    public AudioClip equipment;
    public AudioClip squirt;
    public AudioClip jetpack;
    public AudioClip zipline;

    [Header("Environmental Hazards")]
    public AudioClip fire;
    public AudioClip flies;
    public AudioClip mouse;
    public AudioClip mouseAttacked;
    public AudioClip bird;
    public AudioClip birdAttacked;

    [Header("Misc")]
    public AudioClip door;
    public AudioClip sprinklers;
    public AudioClip hydraulicPress;
    public AudioClip checkpoint;
    public AudioClip push;

    public static AudioManager audioManagerInstance;


    private void Awake()
    {
        if (audioManagerInstance == null)
        {
            audioManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        /*
        switch (sceneIndex)
        {
            case 0:
                PlayMusic(menuBackground);
                break;
            case 1:
                PlayMusic(section1BackgroundMusic);
                break;
            case 2:
                PlayMusic(section2BackgroundMusic);
                break;
            case 3:
                PlayMusic(section3BackgroundMusic);
                break;
            case 4:
                PlayMusic(section4BackgroundMusic);
                break;
            case 5:
                PlayMusic(creditsBackgroundMusic);
                break;
        }
        */

        switch (sceneIndex)
        {
            case 0:
                PlayMusic(menuBackground);
                break;
            case 1:
                PlayMusic(section1BackgroundMusic);
                break;
            case 2:
                PlayMusic(creditsBackgroundMusic);
                break;
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxSource != null && sfxClip != null)
        {
            sfxSource.clip = sfxClip;
            sfxSource.PlayOneShot(sfxClip);
        }
    }

    public AudioSource PlayLoopingSFX(AudioClip sfxClip, bool spatial3D, int minDistance, int maxDistance, Transform attachTo = null)
    {
        if (sfxClip == null) return null;

        GameObject targetObject = attachTo != null ? attachTo.gameObject : gameObject;
        AudioSource source = targetObject.AddComponent<AudioSource>();
        source.clip = sfxClip;
        source.loop = true;
        source.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;

        if (spatial3D)
        {
            source.spatialBlend = 1.0f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();

        return source;
    }

    public void StopLoopingSFX(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source);
        }
    }


    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    public void PlayFootstep(Vector3 position)
    {
        if (footsteps.Length > 0 && sfxSource != null)
        {
            int index = UnityEngine.Random.Range(0, footsteps.Length);

            sfxSource.transform.position = position;
            sfxSource.PlayOneShot(footsteps[index], FootstepAudioVolume);
        }
    }
}