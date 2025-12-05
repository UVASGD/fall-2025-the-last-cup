using UnityEngine;
using StarterAssets;

public class MouseSensitivityHandler : MonoBehaviour
{
    public static MouseSensitivityHandler Instance { get; private set; }

    private const string SENSITIVITY_KEY = "sensitivity";
    private const float DEFAULT_SENSITIVITY = 5f;

    private float currentSensitivity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSensitivity();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSensitivity()
    {
        currentSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, DEFAULT_SENSITIVITY);
    }

    public void SetSensitivity(float newSensitivity)
    {
        currentSensitivity = newSensitivity;
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, newSensitivity);
        PlayerPrefs.Save();

        ApplySensitivityToController();
    }

    public float GetSensitivity()
    {
        return currentSensitivity;
    }

    public void ApplySensitivityToController()
    {
        ThirdPersonController controller = FindAnyObjectByType<ThirdPersonController>();

        if (controller != null)
        {
            controller.lookSensitivity = new Vector2(currentSensitivity, currentSensitivity);
            Debug.Log($"Applied sensitivity {currentSensitivity} to ThirdPersonController");
        }
    }
}