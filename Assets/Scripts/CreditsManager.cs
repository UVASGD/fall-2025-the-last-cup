using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    [Header("Credits UI Elements")]
    [Tooltip("The container that will scroll up")]
    public RectTransform creditsContainer;

    [Header("Scroll Settings")]
    [Tooltip("Speed at which credits scroll (units per second)")]
    public float scrollSpeed = 50f;

    [Tooltip("How long to wait at the end before returning to main menu (seconds)")]
    public float endWaitTime = 3f;

    [Header("Start Position")]
    [Tooltip("Starting Y position of the credits (usually below screen)")]
    public float startYPosition = -1000f;

    [Tooltip("Y position where credits should stop scrolling")]
    public float endYPosition = 2000f;

    private bool isScrolling = true;
    private bool hasFinished = false;
    private float endTimer = 0f;
    private Vector2 currentPosition;

    private void Start()
    {
        if (creditsContainer == null)
        {
            creditsContainer = GetComponent<RectTransform>();
            if (creditsContainer == null)
            {
                Debug.LogError("CreditsManager: No RectTransform found. Please assign creditsContainer.");
                return;
            }
        }

        currentPosition = creditsContainer.anchoredPosition;
        currentPosition.y = startYPosition;
        creditsContainer.anchoredPosition = currentPosition;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (isScrolling)
        {
            currentPosition.y += scrollSpeed * Time.deltaTime;
            creditsContainer.anchoredPosition = currentPosition;

            if (currentPosition.y >= endYPosition)
            {
                isScrolling = false;
                hasFinished = true;
            }
        }
        else if (hasFinished)
        {
            endTimer += Time.deltaTime;

            if (endTimer >= endWaitTime)
            {
                ReturnToMainMenu();
            }
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetScrollSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }
}