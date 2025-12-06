using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    static private DeathScreen instance;

    private UnityEngine.UI.Image givenImage;

    [SerializeField]
    Color startColor;

    [SerializeField]
    Color endColor;

    [SerializeField]
    float transitionTime;

    [SerializeField]
    float stayTime;

    private bool deathFade = false;

    private float currentTransitionTime = 0;

    private float currentStayTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (FindObjectsByType<DeathScreen>(FindObjectsSortMode.None).Length != 1)
        {
            print("ERROR: two DeathScreens established");
            Destroy(this);
        }
        else
        {
            givenImage = this.GetComponent<UnityEngine.UI.Image>();
            if (givenImage == null)
            {
                print("ERROR: death screen is not attached to image");
            }
            else
            {
                instance = this;
            }
        }
    }

    public static void StartFade()
    {
        if (instance != null)
        {
            instance.deathFade = true;
            instance.currentTransitionTime = instance.transitionTime;
            instance.currentStayTime = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (deathFade)
        {
            currentStayTime += Time.deltaTime;
            givenImage.color = endColor;
            if (currentStayTime > stayTime)
            {
                deathFade = false;
            }
        }
        else
        {
            if (currentTransitionTime > 0)
            {
                currentTransitionTime -= Time.deltaTime;
                givenImage.color = Color.Lerp(startColor, endColor, currentTransitionTime/transitionTime);
            }
            else
            {
                givenImage.color = startColor;
            }
        }
    }
}
