using UnityEngine;
using System.Collections.Generic;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Direction")]
    [Tooltip("The starting point of the conveyor direction")]
    public Transform startPoint;

    [Tooltip("The ending point of the conveyor direction")]
    public Transform endPoint;

    [Header("Force Settings")]
    [Tooltip("Speed of the conveyor belt")]
    public float conveyorSpeed = 3f;

    [Header("Object Types")]
    [Tooltip("Push the player")]
    public bool affectPlayer = true;

    [Tooltip("Push rigidbody objects")]
    public bool affectRigidbodies = true;

    private Vector3 moveDirection;
    private List<Rigidbody> rigidbodiesOnBelt = new List<Rigidbody>();

    public Vector3 ConveyorVelocity { get; private set; }

    private void Start()
    {
        CalculateDirection();
    }

    private void Update()
    {
        ConveyorVelocity = moveDirection * conveyorSpeed;

        if (affectRigidbodies)
        {
            PushRigidbodies();
        }
    }

    private void CalculateDirection()
    {
        if (startPoint != null && endPoint != null)
        {
            moveDirection = (endPoint.position - startPoint.position).normalized;
        }
        else
        {
            moveDirection = transform.forward;
            Debug.LogWarning($"ConveyorBelt on {gameObject.name}: No direction points assigned, using forward direction");
        }
    }

    private void PushRigidbodies()
    {
        for (int i = rigidbodiesOnBelt.Count - 1; i >= 0; i--)
        {
            if (rigidbodiesOnBelt[i] == null)
            {
                rigidbodiesOnBelt.RemoveAt(i);
                continue;
            }

            Rigidbody rb = rigidbodiesOnBelt[i];
            if (!rb.isKinematic)
            {
                Vector3 force = moveDirection * conveyorSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + force);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (affectRigidbodies)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && !rigidbodiesOnBelt.Contains(rb))
            {
                rigidbodiesOnBelt.Add(rb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rigidbodiesOnBelt.Remove(rb);
        }
    }
}