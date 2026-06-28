using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 20f; 
    public float bounceSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
