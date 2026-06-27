using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Hassasiyet")]
    public float mouseSensitivity = 100f; // Settings'ten deðiþecek

    [Header("Kamera Limitleri")]
    public float minVerticalAngle = -30f; // Aþaðý bakýþ limiti
    public float maxVerticalAngle = 60f;  // Yukarý bakýþ limiti

    [Header("Hedef")]
    public Transform playerBody; // Robot

    private float xRotation = 0f; // Yukarý-aþaðý rotasyon

    void Start()
    {
        // Cursor kilitle
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // Settings'ten hassasiyeti yükle ? GÜNCELLE! ?
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        else
        {
            mouseSensitivity = 5f; // ? Varsayýlan 5 ?
        }

        Debug.Log("??? Hassasiyet yüklendi: " + mouseSensitivity);
    }

    void Update()
    {
        // Pause kontrolünü KALDIR, sadece cursor kontrolü yap
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            // Cursor unlock ise (pause veya help açýk), kamera kontrolü yok
            return;
        }

        float sensitivityMultiplier = 0.3f; // Hassasiyet çarpaný

        // Fare hareketi
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        // Yukarý-aþaðý bakýþ (kamera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Saða-sola bakýþ (robot)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    // Settings'ten hassasiyet güncelleme
    public void UpdateSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();

        Debug.Log("??? Hassasiyet güncellendi: " + mouseSensitivity);
    }
}
