using UnityEngine;

public class CallRandomizer : MonoBehaviour
{
    [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    float minAudioGap;

    [SerializeField]
    float maxAudioGap;

    private AudioSource attachedSource;

    [SerializeField]
    private float currentAudioTime = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAudioTime = Random.Range(minAudioGap, maxAudioGap);

        attachedSource = GetComponent<AudioSource>();
        if (attachedSource == null)
        {
            print("Error: audio source not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentAudioTime -= Time.deltaTime;
        if (currentAudioTime < 0)
        {
            currentAudioTime = Random.Range(minAudioGap, maxAudioGap);
            attachedSource.clip = clips[Random.Range(0,clips.Length)];
            attachedSource.Play();
        }
    }
}
