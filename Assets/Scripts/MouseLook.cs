using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("Camera Limit")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("Hedef")]
    public Transform playerBody; // Robot

    private float xRotation = 0f;

    void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        else
        {
            mouseSensitivity = 5f;
        }
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        float sensitivityMultiplier = 0.3f;

        //Mouse movemant
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }
}
