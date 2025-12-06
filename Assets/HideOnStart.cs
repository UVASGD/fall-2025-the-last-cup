using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    [SerializeField]
    bool hides;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hides)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
