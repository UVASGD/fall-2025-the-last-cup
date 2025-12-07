using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlySwarmController : SwarmController
{
    [Header("Location Control")]
    [SerializeField] private List<GameObject> locations = new List<GameObject>();
    [SerializeField] private float transitionDuration = 2f;

    private int currentLocationIndex = 0;
    private bool isMoving = false;
    private Collider swarmCollider;

    private void Awake()
    {
        SetupSwarmCollider();
    }

    private void SetupSwarmCollider()
    {
        swarmCollider = GetComponent<Collider>();
        if (swarmCollider == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = swarmSize;
            boxCollider.isTrigger = true;
            swarmCollider = boxCollider;
        }
        else
        {
            swarmCollider.isTrigger = true;
        }
    }


    private void Start()
    {
        if (locations.Count == 0)
        {
            swarmCenter = transform.position;
        }
        else
        {
            transform.position = locations[0].transform.position;
            swarmCenter = locations[0].transform.position;
            currentLocationIndex = 0;
        }

        SpawnFlies();
        AudioManager.audioManagerInstance.PlayLoopingSFX(AudioManager.audioManagerInstance.flies, true, 1, 5, transform);
    }

    private void SpawnFlies()
    {
        for (int i = 0; i < maxAmount; i++)
        {
            Vector3 randomPos = GetRandomPositionInBounds();
            GameObject fly = Instantiate(prefab, randomPos, Random.rotation, transform);

            objects.Add(fly);
            targetPositions.Add(GetRandomPositionInBounds());
        }
    }

    private void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            MoveFly(i);
        }
    }

    private void MoveFly(int index)
    {
        GameObject fly = objects[index];
        if (fly == null) return;

        Vector3 targetPos = targetPositions[index];
        Vector3 currentPos = fly.transform.localPosition;

        float noise = Mathf.PerlinNoise(
            Time.time * noiseSpeed + index,
            Time.time * noiseSpeed
        ) * noiseScale;

        Vector3 noiseOffset = new Vector3(
            Mathf.Sin(Time.time + index) * noise,
            Mathf.Cos(Time.time * 1.5f + index) * noise,
            Mathf.Sin(Time.time * 0.8f + index) * noise
        );

        Vector3 desiredLocalPos = targetPos + noiseOffset;
        Vector3 direction = (desiredLocalPos - currentPos).normalized;

        fly.transform.localPosition = Vector3.Lerp(
            currentPos,
            desiredLocalPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(currentPos, targetPos) < 0.5f)
        {
            targetPositions[index] = GetRandomPositionInBounds();
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            fly.transform.rotation = Quaternion.Slerp(
                fly.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving) return;

        WaterProjectile waterProjectile = other.GetComponent<WaterProjectile>();
        if (waterProjectile != null)
        {
            Debug.Log("Swarm hit by water projectile!");
            MoveToNextLocation();
        }
    }

    private void MoveToNextLocation()
    {
        if (isMoving) return;

        if (locations.Count == 0)
        {
            Debug.LogWarning("No locations assigned to fly swarm!");
            return;
        }

        isMoving = true;

        currentLocationIndex++;
        if (currentLocationIndex >= locations.Count)
        {
            currentLocationIndex = 0;
        }

        Vector3 targetLocation = locations[currentLocationIndex].transform.position;
        StartCoroutine(TransitionToLocation(targetLocation));
    }

    private IEnumerator TransitionToLocation(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        Debug.Log($"Moving swarm from {startPosition} to {targetPosition}");

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            swarmCenter = transform.position;

            yield return null;
        }

        transform.position = targetPosition;
        swarmCenter = targetPosition;

        isMoving = false;

        Debug.Log($"Swarm arrived at location {currentLocationIndex}");
    }

    protected new Vector3 GetRandomPositionInBounds()
    {
        float radius = Mathf.Min(swarmSize.x, swarmSize.y, swarmSize.z) / 2f;

        Vector3 randomPoint;
        do
        {
            randomPoint = Random.insideUnitSphere * radius;
        }
        while (randomPoint.y < 0f);

        Vector3 scale = new Vector3(
            swarmSize.x / (radius * 2f),
            swarmSize.y / (radius * 2f),
            swarmSize.z / (radius * 2f)
        );

        return Vector3.Scale(randomPoint, scale);
    }

    private new void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? transform.position : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, swarmSize);

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireCube(center + Vector3.up * swarmSize.y / 4f, new Vector3(swarmSize.x, swarmSize.y / 2f, swarmSize.z));

        if (locations != null && locations.Count > 0)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null)
                {
                    Gizmos.color = i == 0 ? Color.green : Color.cyan;
                    Gizmos.DrawWireSphere(locations[i].transform.position, 0.5f);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(locations[i].transform.position, swarmSize);

                    Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                    Gizmos.DrawWireCube(locations[i].transform.position + Vector3.up * swarmSize.y / 4f, new Vector3(swarmSize.x, swarmSize.y / 2f, swarmSize.z));

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
    }
}