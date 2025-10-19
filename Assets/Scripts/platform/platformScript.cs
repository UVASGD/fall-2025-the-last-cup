using UnityEngine;

public class platformScript : MonoBehaviour
{
    public float swingSpeed = 2f;       
    public float minX = -5f;            
    public float maxX = 5f;             

    private Vector3 startPos;

    void Start(){
        startPos = transform.position;  
    }

    void FixedUpdate()
    {

        float sinValue = Mathf.Sin(Time.time * swingSpeed);

        float offsetX = Mathf.Lerp(minX, maxX, (sinValue + 1f) / 2f);

        transform.position = startPos + Vector3.right * offsetX;
    }


}