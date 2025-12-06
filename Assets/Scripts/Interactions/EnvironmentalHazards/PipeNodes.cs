using UnityEngine;

public class PipeNodes : MonoBehaviour
{
    // Represents parts of a pipe that rats go to, 
    // This needs to contain at least two nodes
    [SerializeField]
    public Transform[] pipeNodes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pipeNodes.Length < 2)
        {
            print("ERROR: insufficient pipe nodes given");
            Destroy(this);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
