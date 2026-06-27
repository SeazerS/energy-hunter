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
        // HAREKET KÝLÝDÝ KONTROLÜ! ? YENÝ! ?
        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            return; // E tuþu çalýþmaz
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Obje yakýnda mý kontrol et
            bool nearObject = CheckNearObject();

            if (!nearObject)
            {
                // Obje yok - genel ses çal
                PlayInteractionSound(); // ? DEÐÝÞTÝRDÝK! ?
                Debug.Log("?? Genel E tuþu sesi (obje yok)");
            }
            else
            {
                Debug.Log("?? E basýldý - obje var (obje sesi çalacak)");
            }
        }

    }

    void PlayInteractionSound()
    {
        if (interactionSound == null) return;

        // Master volume al
        float masterVolume = 1f;

        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            masterVolume = AudioManager.Instance.audioSource.volume;
        }

        // Ses çal
        audioSource.PlayOneShot(interactionSound, masterVolume * interactionVolume);
    }

    bool CheckNearObject()
    {
        // Yakýndaki cihazlarý kontrol et
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            // playerInRange kontrolü
            float distance = Vector3.Distance(transform.position, device.transform.position);

            if (distance < 3f) // Trigger mesafesi (ayarla)
            {
                return true; // Obje var
            }
        }

        return false; // Obje yok
    }
}