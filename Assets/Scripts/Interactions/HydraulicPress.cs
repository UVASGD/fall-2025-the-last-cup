using UnityEngine;

public class HydraulicPress : MonoBehaviour
{
    [Header("Press Components")]
    [SerializeField] private Transform press;
    [SerializeField] private Transform pressHolder;

    [Header("Movement Settings")]
    [SerializeField] private float pressDistance = 2f;
    [SerializeField] private float pressSpeed = 1f;
    [SerializeField] private float holdTimeAtBottom = 0.5f;
    [SerializeField] private float holdTimeAtTop = 1f;

    [Header("Damage Settings")]
    [SerializeField] private float crushThreshold = 0.8f;

    private Vector3 pressStartPosition;
    private Vector3 pressHolderStartPosition;
    private float currentTime;
    private PressState currentState = PressState.MovingDown;
    private bool playerInCrushZone = false;
    private GameObject playerObject;

    private enum PressState
    {
        MovingDown,
        HoldingAtBottom,
        MovingUp,
        HoldingAtTop
    }

    private void Start()
    {
        if (press != null)
        {
            pressStartPosition = press.localPosition;
        }

        if (pressHolder != null)
        {
            pressHolderStartPosition = pressHolder.localPosition;
        }

        currentTime = 0f;
    }

    private void Update()
    {
        if (press == null || pressHolder == null)
            return;

        currentTime += Time.deltaTime;

        switch (currentState)
        {
            case PressState.MovingDown:
                UpdateMovingDown();
                break;

            case PressState.HoldingAtBottom:
                UpdateHoldingAtBottom();
                break;

            case PressState.MovingUp:
                UpdateMovingUp();
                break;

            case PressState.HoldingAtTop:
                UpdateHoldingAtTop();
                break;
        }
    }

    private void UpdateMovingDown()
    {
        float progress = Mathf.Clamp01(currentTime * pressSpeed);
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        Vector3 targetPressPosition = pressStartPosition + Vector3.down * pressDistance;
        Vector3 targetHolderPosition = pressHolderStartPosition + Vector3.down * pressDistance;

        press.localPosition = Vector3.Lerp(pressStartPosition, targetPressPosition, smoothProgress);
        pressHolder.localPosition = Vector3.Lerp(pressHolderStartPosition, targetHolderPosition, smoothProgress);

        if (playerInCrushZone && smoothProgress >= crushThreshold)
        {
            CrushPlayer();
        }

        if (progress >= 1f)
        {
            currentState = PressState.HoldingAtBottom;
            currentTime = 0f;
        }
    }

    private void UpdateHoldingAtBottom()
    {
        if (currentTime >= holdTimeAtBottom)
        {
            currentState = PressState.MovingUp;
            currentTime = 0f;
        }
    }

    private void UpdateMovingUp()
    {
        float progress = Mathf.Clamp01(currentTime * pressSpeed);
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        Vector3 targetPressPosition = pressStartPosition + Vector3.down * pressDistance;
        Vector3 targetHolderPosition = pressHolderStartPosition + Vector3.down * pressDistance;

        press.localPosition = Vector3.Lerp(targetPressPosition, pressStartPosition, smoothProgress);
        pressHolder.localPosition = Vector3.Lerp(targetHolderPosition, pressHolderStartPosition, smoothProgress);

        if (progress >= 1f)
        {
            currentState = PressState.HoldingAtTop;
            currentTime = 0f;
        }
    }

    private void UpdateHoldingAtTop()
    {
        if (currentTime >= holdTimeAtTop)
        {
            currentState = PressState.MovingDown;
            currentTime = 0f;
        }
    }

    private void CrushPlayer()
    {
        if (playerObject == null)
            return;

        RespawnScript[] respawnScripts = FindObjectsByType<RespawnScript>(FindObjectsSortMode.None);

        if (respawnScripts.Length > 0)
        {
            foreach (RespawnScript respawnScript in respawnScripts)
            {
                if (respawnScript.player == playerObject && respawnScript.respawnPoint != null)
                {
                    CharacterController controller = playerObject.GetComponent<CharacterController>();
                    if (controller != null)
                    {
                        controller.enabled = false;
                        playerObject.transform.position = respawnScript.respawnPoint.transform.position;
                        controller.enabled = true;
                    }

                    playerInCrushZone = false;
                    playerObject = null;
                    return;
                }
            }
        }
    }

    public void OnPlayerEnterCrushZone(GameObject player)
    {
        playerInCrushZone = true;
        playerObject = player;
    }

    public void OnPlayerExitCrushZone(GameObject player)
    {
        playerInCrushZone = false;
        playerObject = null;
    }

    public void ResetPress()
    {
        if (press != null)
        {
            press.localPosition = pressStartPosition;
        }

        if (pressHolder != null)
        {
            pressHolder.localPosition = pressHolderStartPosition;
        }

        currentState = PressState.HoldingAtTop;
        currentTime = 0f;
        playerInCrushZone = false;
        playerObject = null;
    }
}