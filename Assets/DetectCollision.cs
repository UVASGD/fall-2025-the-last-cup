using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("??");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        // Forward trigger info if you’re using triggers
        //parent?.OnChildTrigger(other);
    }
}
