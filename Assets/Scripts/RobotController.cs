using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Walking Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private CharacterController controller;

    [Header("Audio")]
    public AudioSource walkAudioSource;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            Debug.Log("There is the animator");
        }
        else
        {
            Debug.LogError("There is no animator");
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

        UpdateWalkVolume();
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            Debug.Log("Movemant lock! canPlayerMove = false");
            return;
        }

        Debug.Log("Movement active! canPlayerMove = true");

        MoveRobot();

        if (Input.GetKeyDown(KeyCode.E))
        {

            if (animator != null)
            {
                animator.SetTrigger("Interact");
            }

        }

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

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical) + (cameraRight * horizontal);

        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection.Normalize();

            //Movemant
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            //Rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Animasyon: Walk
            if (animator != null)
            {
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

        //Gravity
        controller.Move(Vector3.down * 2f * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (walkAudioSource != null)
        {
            walkAudioSource.volume = AudioManager.masterVolume;
        }
    }

    public void UpdateWalkVolume()
    {
        if (walkAudioSource != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float volumeMultiplier = 3f;
            float finalVolume = savedVolume * volumeMultiplier;
            finalVolume = Mathf.Clamp(finalVolume, 0f, 1f);

            walkAudioSource.volume = finalVolume;
        }
    }
}