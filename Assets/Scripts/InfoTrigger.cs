using UnityEngine;
using TMPro;

public class InfoTrigger : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private GameObject textDisplay;
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Content")]
    [TextArea(3, 10)]
    [SerializeField] private string displayText = "Enter your text here";

    private void Start()
    {
        if (textDisplay != null)
        {
            textDisplay.SetActive(false);
        }

        if (textComponent != null)
        {
            textComponent.text = displayText;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (textDisplay != null)
            {
                textDisplay.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (textDisplay != null)
            {
                textDisplay.SetActive(false);
            }
        }
    }
}