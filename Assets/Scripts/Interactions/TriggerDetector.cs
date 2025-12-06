using UnityEngine;

public class PressTriggerDetector : MonoBehaviour
{
    private HydraulicPress hydraulicPress;

    private void Awake()
    {
        hydraulicPress = GetComponentInParent<HydraulicPress>();

        if (hydraulicPress == null)
        {
            Debug.LogError("PressTriggerDetector could not find HydraulicPress in parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hydraulicPress != null)
        {
            hydraulicPress.OnPlayerEnterCrushZone(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && hydraulicPress != null)
        {
            hydraulicPress.OnPlayerExitCrushZone(other.gameObject);
        }
    }
}