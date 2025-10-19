using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Entered platform!!!");
            other.gameObject.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Exited platform!!!");
            other.gameObject.transform.parent = null;
        }
    }
}
