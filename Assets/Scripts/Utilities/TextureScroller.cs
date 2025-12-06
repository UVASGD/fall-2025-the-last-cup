using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("Speed of texture scrolling on X axis")]
    public float scrollSpeedX = 0f;

    [Tooltip("Speed of texture scrolling on Y axis")]
    public float scrollSpeedY = 1f;

    [Header("Optional: Direction-Based Scrolling")]
    [Tooltip("Use direction points instead of manual X/Y values")]
    public bool useDirectionPoints = false;

    [Tooltip("Starting point for direction calculation")]
    public Transform startPoint;

    [Tooltip("Ending point for direction calculation")]
    public Transform endPoint;

    [Tooltip("Speed multiplier when using direction points")]
    public float directionSpeed = 1f;

    [Header("Renderer Settings")]
    [Tooltip("The renderer with the material to scroll")]
    public Renderer targetRenderer;

    [Tooltip("Material texture property name (_BaseMap for URP, _MainTex for Built-in)")]
    public string texturePropertyName = "_BaseMap";

    [Header("Advanced")]
    [Tooltip("Use shared material (affects all objects with this material)")]
    public bool useSharedMaterial = false;

    private Material scrollMaterial;
    private Vector2 scrollDirection;

    private void Start()
    {
        InitializeMaterial();

        if (useDirectionPoints)
        {
            CalculateDirectionFromPoints();
        }
        else
        {
            scrollDirection = new Vector2(scrollSpeedX, scrollSpeedY);
        }
    }

    private void Update()
    {
        ScrollTexture();
    }

    private void InitializeMaterial()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer != null)
        {
            scrollMaterial = useSharedMaterial ? targetRenderer.sharedMaterial : targetRenderer.material;
        }
        else
        {
            Debug.LogWarning($"TextureScroller on {gameObject.name}: No renderer found!");
        }
    }

    private void CalculateDirectionFromPoints()
    {
        if (startPoint != null && endPoint != null)
        {
            Vector3 worldDirection = (endPoint.position - startPoint.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(worldDirection);

            scrollDirection = new Vector2(localDirection.x, localDirection.z) * directionSpeed;
        }
        else
        {
            scrollDirection = new Vector2(scrollSpeedX, scrollSpeedY);
        }
    }

    private void ScrollTexture()
    {
        if (scrollMaterial != null)
        {
            Vector2 offset = scrollMaterial.GetTextureOffset(texturePropertyName);
            offset += scrollDirection * Time.deltaTime;
            scrollMaterial.SetTextureOffset(texturePropertyName, offset);
        }
    }

    public void SetScrollSpeed(float x, float y)
    {
        scrollSpeedX = x;
        scrollSpeedY = y;
        scrollDirection = new Vector2(x, y);
    }

    public void SetScrollSpeed(Vector2 speed)
    {
        scrollDirection = speed;
        scrollSpeedX = speed.x;
        scrollSpeedY = speed.y;
    }

    private void OnDestroy()
    {
        if (!useSharedMaterial && scrollMaterial != null)
        {
            Destroy(scrollMaterial);
        }
    }
}