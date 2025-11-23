using UnityEngine;
using System.Collections.Generic;

public class SwarmController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int maxAmount = 10;

    [Header("Swarm Bounds")]
    protected Vector3 swarmCenter;
    [SerializeField] protected Vector3 swarmSize = new Vector3(3f, 3f, 3f);

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float noiseScale = 1f;
    [SerializeField] protected float noiseSpeed = 0.5f;

    protected List<GameObject> objects = new List<GameObject>();
    protected List<Vector3> targetPositions = new List<Vector3>();

    private void Start()
    {
        swarmCenter = gameObject.transform.position;

        SpawnObjects();
    }

    protected void SpawnObjects()
    {
        for (int i = 0; i < maxAmount; i++)
        {
            Vector3 randomPos = GetRandomPositionInBounds();
            GameObject _object = Instantiate(prefab, randomPos, Random.rotation, transform);
            objects.Add(_object);
            targetPositions.Add(GetRandomPositionInBounds());
        }
    }

    private void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            MoveObject(i);
        }
    }

    protected void MoveObject(int index)
    {
        GameObject fly = objects[index];
        Vector3 targetPos = targetPositions[index];

        float noise = Mathf.PerlinNoise(
            Time.time * noiseSpeed + index,
            Time.time * noiseSpeed
        ) * noiseScale;

        Vector3 noiseOffset = new Vector3(
            Mathf.Sin(Time.time + index) * noise,
            Mathf.Cos(Time.time * 1.5f + index) * noise,
            Mathf.Sin(Time.time * 0.8f + index) * noise
        );

        Vector3 currentPos = fly.transform.position;
        Vector3 direction = (targetPos - currentPos).normalized;

        fly.transform.position = Vector3.Lerp(
            currentPos,
            targetPos + noiseOffset,
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

    protected Vector3 GetRandomPositionInBounds()
    {
        return swarmCenter + new Vector3(
            Random.Range(-swarmSize.x / 2, swarmSize.x / 2),
            Random.Range(-swarmSize.y / 2, swarmSize.y / 2),
            Random.Range(-swarmSize.z / 2, swarmSize.z / 2)
        );
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(swarmCenter, swarmSize.y);
    }
}
