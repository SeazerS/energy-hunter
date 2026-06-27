using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Collision Ayarlarý")]
    public float minDistance = 1f; // En yakýn mesafe
    public float maxDistance = 3f; // Normal mesafe
    public float smoothSpeed = 10f; // Yumuþak geçiþ

    [Header("Hedef")]
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

        // Robot'tan kameraya raycast
        if (Physics.Raycast(target.position, (desiredPosition - target.position).normalized, out hit, maxDistance))
        {
            // Duvar var, kamerayý yaklaþtýr
            float distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            Vector3 direction = (desiredPosition - target.position).normalized;
            Vector3 newPosition = target.position + direction * distance;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Duvar yok, normal pozisyon
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPosition, Time.deltaTime * smoothSpeed);
        }
    }
}