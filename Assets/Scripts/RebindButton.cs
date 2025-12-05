using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [Header("References")]
    public string actionName;
    public int bindingIndex = 0;
    public Button rebindButton;
    public TextMeshProUGUI bindingText;
    public GameObject waitingForInputPanel;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        rebindButton.onClick.AddListener(StartRebinding);
        UpdateBindingDisplay();
    }

    private void OnEnable()
    {
        UpdateBindingDisplay();
    }

    public void UpdateBindingDisplay()
    {
        if (InputRebindingManager.Instance == null) return;

        InputAction action = InputRebindingManager.Instance.GetAction(actionName);
        if (action == null) return;

        string bindingDisplayString = action.GetBindingDisplayString(bindingIndex);
        bindingText.text = bindingDisplayString;
    }

    private void StartRebinding()
    {
        if (InputRebindingManager.Instance == null) return;

        InputAction action = InputRebindingManager.Instance.GetAction(actionName);
        if (action == null) return;

        action.Disable();

        if (waitingForInputPanel != null)
            waitingForInputPanel.SetActive(true);

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .OnCancel(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        if (waitingForInputPanel != null)
            waitingForInputPanel.SetActive(false);

        rebindingOperation.Dispose();

        if (InputRebindingManager.Instance == null) return;

        InputAction action = InputRebindingManager.Instance.GetAction(actionName);
        if (action != null)
            action.Enable();

        UpdateBindingDisplay();
        InputRebindingManager.Instance.SaveRebinds();
    }

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }
}