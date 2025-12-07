using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BirdController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private BirdAnimationManager birdAnimationManager;

    [Header("Location Control")]
    [SerializeField] private List<GameObject> locations = new List<GameObject>();

    [Header("Movement")]
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float preFlightRotationDuration = 0.5f;
    [SerializeField] private AnimationCurve flightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private int currentLocationIndex = 0;
    private bool isMoving = false;
    private Collider birdCollider;

    private AudioSource birdLoopingSource;

    private void Awake()
    {
        SetupCollider();

        if (birdAnimationManager == null)
        {
            birdAnimationManager = GetComponentInChildren<BirdAnimationManager>();
        }
    }

    private void SetupCollider()
    {
        birdCollider = GetComponent<Collider>();
        if (birdCollider == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 2.0f;
            sphereCollider.isTrigger = true;
            birdCollider = sphereCollider;
        }
        else
        {
            birdCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        if (locations.Count > 0)
        {
            transform.position = locations[0].transform.position;
            ResetRotation();
            currentLocationIndex = 0;
        }

        if (birdAnimationManager != null)
        {
            birdAnimationManager.Fly(false);
        }

        birdLoopingSource = AudioManager.audioManagerInstance.PlayLoopingSFX(AudioManager.audioManagerInstance.bird, true, 1, 5, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving) return;

        WaterProjectile waterProjectile = other.GetComponent<WaterProjectile>();
        if (waterProjectile != null)
        {
            Debug.Log("Bird hit by water projectile!");
            isMoving = true;

            if (birdLoopingSource != null)
            {
                AudioManager.audioManagerInstance.StopLoopingSFX(birdLoopingSource);
                birdLoopingSource = null;
            }

            AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.birdAttacked);

            if (birdAnimationManager != null)
            {
                birdAnimationManager.Ruffle();
                birdAnimationManager.Fly(true);
            }

            StartCoroutine(MoveToNextLocation());
        }
    }

    private IEnumerator MoveToNextLocation()
    {
        if (locations.Count == 0)
        {
            Debug.LogWarning("No locations assigned to bird!");
            isMoving = false;

            if (birdAnimationManager != null)
            {
                birdAnimationManager.Fly(false);
            }

            yield break;
        }

        currentLocationIndex++;
        if (currentLocationIndex >= locations.Count)
        {
            currentLocationIndex = 0;
        }

        GameObject targetLocation = locations[currentLocationIndex];
        yield return StartCoroutine(FlyToLocation(targetLocation));
    }

    private IEnumerator FlyToLocation(GameObject targetLocation)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetLocation.transform.position;

        Vector3 directionToTarget = (targetPosition - startPosition).normalized;
        Quaternion targetFlightRotation = directionToTarget != Vector3.zero
            ? Quaternion.LookRotation(directionToTarget)
            : transform.rotation;

        Quaternion startRotation = transform.rotation;
        float rotationElapsed = 0f;

        Debug.Log("Starting rotation towards flight direction...");
        while (rotationElapsed < preFlightRotationDuration)
        {
            rotationElapsed += Time.deltaTime;
            float rotationT = rotationElapsed / preFlightRotationDuration;

            transform.rotation = Quaternion.Slerp(startRotation, targetFlightRotation, rotationT);

            yield return null;
        }

        transform.rotation = targetFlightRotation;

        Debug.Log($"Rotation complete, flying from {startPosition} to {targetPosition}");

        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / flySpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveT = flightCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);

            yield return null;
        }

        transform.position = targetPosition;

        ResetRotation();

        if (birdAnimationManager != null)
        {
            birdAnimationManager.Fly(false);
        }

        isMoving = false;
        birdLoopingSource = AudioManager.audioManagerInstance.PlayLoopingSFX(AudioManager.audioManagerInstance.bird, true, 1, 5, transform);

        Debug.Log($"Bird arrived at location {currentLocationIndex}");
    }

    private void ResetRotation()
    {
        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (locations != null && locations.Count > 0)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null)
                {
                    Gizmos.color = i == 0 ? Color.green : Color.cyan;
                    Gizmos.DrawWireSphere(locations[i].transform.position, 0.3f);

                    Vector3 forward = locations[i].transform.forward * 0.5f;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(locations[i].transform.position, forward);

                    if (i < locations.Count - 1 && locations[i + 1] != null)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(locations[i].transform.position, locations[i + 1].transform.position);
                    }
                    else if (i == locations.Count - 1 && locations[0] != null)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(locations[i].transform.position, locations[0].transform.position);
                    }
                }
            }
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}