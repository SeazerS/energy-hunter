using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private CharacterController controller;

    [Header("Audio")]
    public AudioSource walkAudioSource; // Yürüme sesi AudioSource

    void Start()
    {
        // Animator bul
        animator = GetComponent<Animator>();

        // DEBUG! ? EKLE
        if (animator != null)
        {
            Debug.Log("? Animator BULUNDU!");
        }
        else
        {
            Debug.LogError("? Animator BULUNAMADI!");
        }

        // CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

        // Baþlangýçta volume yükle ?
        UpdateWalkVolume();
    }

    void Update()
    {
        // HAREKET KÝLÝDÝ KONTROLÜ
        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            Debug.Log("?? Hareket kilitli! canPlayerMove = false"); // ? DEBUG! ?
            return;
        }

        Debug.Log("? Hareket aktif! canPlayerMove = true"); // ? DEBUG! (her frame çok olur, kaldýrabilirsin)

        MoveRobot();

        // DEBUG: E tuþu test ? ZATENVARDI
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("?? E tuþuna basýldý!");

            if (animator != null)
            {
                animator.SetTrigger("Interact");
                Debug.Log("? Interact tetiklendi!");
            }
            else
            {
                Debug.LogError("? Animator yok, tetiklenemedi!");
            }

        }

        // Yürüme sesi çalarken volume'u güncelle ? EKLE! ?
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.volume = AudioManager.masterVolume;
        }
    }

    void MoveRobot()
    {
        // WASD input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Kamera bazlý hareket
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical) + (cameraRight * horizontal);

        // Hareket var mý?
        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection.Normalize();

            // Hareket et
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Dönüþ
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Animasyon: Walk
            if (animator != null)
            {
                Debug.Log("?? isWalking = TRUE"); // ? DEBUG EKLE!
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            // Animasyon: Idle
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }

        // Yerçekimi
        controller.Move(Vector3.down * 2f * Time.deltaTime);
    }

    void LateUpdate()
    {
        // AudioManager'dan master volume'u al ve uygula ? YENÝ! ?
        if (walkAudioSource != null)
        {
            walkAudioSource.volume = AudioManager.masterVolume;
        }
    }

    // YENÝ FONKSÝYON! ?
    public void UpdateWalkVolume()
    {
        if (walkAudioSource != null)
        {
            // AudioManager'dan volume al
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float volumeMultiplier = 3f;
            float finalVolume = savedVolume * volumeMultiplier;
            finalVolume = Mathf.Clamp(finalVolume, 0f, 1f);

            walkAudioSource.volume = finalVolume;
            Debug.Log("?? Yürüme sesi güncellendi: " + walkAudioSource.volume);
        }
    }
}