using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Sound")]
    public AudioClip interactionSound;
    public float interactionVolume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = interactionVolume;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool nearObject = CheckNearObject();

            if (!nearObject)
            {
                PlayInteractionSound();
            }
        }

    }

    void PlayInteractionSound()
    {
        if (interactionSound == null) return;

        float masterVolume = 1f;

        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            masterVolume = AudioManager.Instance.audioSource.volume;
        }
        audioSource.PlayOneShot(interactionSound, masterVolume * interactionVolume);
    }

    bool CheckNearObject()
    {
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            float distance = Vector3.Distance(transform.position, device.transform.position);

            if (distance < 3f)
            {
                return true;
            }
        }

        return false;
    }
}