using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    public float minDistance = 1f;
    public float maxDistance = 3f; 
    public float smoothSpeed = 10f;

    [Header("Target")]
    public Transform target; // Robot

    private Vector3 defaultLocalPosition;

    void Start()
    {
        defaultLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(defaultLocalPosition);
        RaycastHit hit;

        if (Physics.Raycast(target.position, (desiredPosition - target.position).normalized, out hit, maxDistance))
        {
            float distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            Vector3 direction = (desiredPosition - target.position).normalized;
            Vector3 newPosition = target.position + direction * distance;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPosition, Time.deltaTime * smoothSpeed);
        }
    }
}