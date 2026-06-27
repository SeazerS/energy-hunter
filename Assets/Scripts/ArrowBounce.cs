using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    [Header("Bounce Ayarlarý")]
    public float bounceHeight = 20f; // Kaç piksel yukarý-aþaðý
    public float bounceSpeed = 2f;   // Hýz

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Sin dalgasý ile yukarý-aþaðý hareket
        float newY = startPos.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
