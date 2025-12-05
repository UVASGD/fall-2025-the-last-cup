using UnityEngine;
using UnityEngine.UI;

public class ControlsUIManager : MonoBehaviour
{
    [Header("Rebind Buttons")]
    public RebindButton[] rebindButtons;

    [Header("Control Buttons")]
    public Button saveButton;
    public Button resetButton;

    private void Start()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveControls);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetControls);
    }

    public void SaveControls()
    {
        if (InputRebindingManager.Instance != null)
        {
            InputRebindingManager.Instance.SaveRebinds();
        }
    }

    public void ResetControls()
    {
        if (InputRebindingManager.Instance != null)
        {
            InputRebindingManager.Instance.ResetToDefaults();
            RefreshAllBindingDisplays();
        }
    }

    private void RefreshAllBindingDisplays()
    {
        foreach (RebindButton button in rebindButtons)
        {
            button.UpdateBindingDisplay();
        }
    }
}