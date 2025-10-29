using UnityEngine;

public class HeightZone : MonoBehaviour
{
    [SerializeField] public ScoopableObject.ScoopType fallObjects;
    public ScoopableObject.ScoopType FallObject => fallObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
