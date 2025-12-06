using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Direction")]
    [Tooltip("The starting point of the conveyor direction")]
    public Transform startPoint;

    [Tooltip("The ending point of the conveyor direction")]
    public Transform endPoint;

    [Header("Force Settings")]
    [Tooltip("Force applied to push objects on the belt")]
    public float pushForce = 5f;

    [Header("Object Types")]
    [Tooltip("Push the player")]
    public bool affectPlayer = true;

    [Tooltip("Push rigidbody objects")]
    public bool affectRigidbodies = true;

    private Vector3 moveDirection;
    private CharacterController playerController;

    private void Start()
    {
        CalculateDirection();
    }

    private void FixedUpdate()
    {
        if (playerController != null && affectPlayer)
        {
            PushPlayer();
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

    private void PushPlayer()
    {
        if (playerController != null && playerController.enabled)
        {
            Vector3 pushVelocity = moveDirection * pushForce * Time.fixedDeltaTime;
            playerController.Move(pushVelocity);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (affectPlayer && other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                playerController = controller;
            }
        }

        if (affectRigidbodies)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForce(moveDirection * pushForce, ForceMode.Force);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawSphere(startPoint.position, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint.position, 0.2f);

            Gizmos.color = Color.yellow;
            Vector3 direction = (endPoint.position - startPoint.position).normalized;
            Vector3 midPoint = (startPoint.position + endPoint.position) * 0.5f;
            Gizmos.DrawRay(midPoint, direction * 2f);
        }
    }
}