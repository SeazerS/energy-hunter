using UnityEngine;

public class RobotSounds : MonoBehaviour
{
    [Header("Walk Loop Sound")]
    public AudioClip walkLoopSound;
    public float walkVolume = 0.4f; 

    [Header("Beep Voice Sound")]
    public AudioClip beepSound;
    public float minBeepInterval = 5f;
    public float maxBeepInterval = 15f;
    public float beepVolume = 0.6f; 

    private AudioSource walkAudioSource;
    private AudioSource beepAudioSource;
    private float nextBeepTime;

    void Start()
    {
        // Walk AudioSource
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.clip = walkLoopSound;
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
        walkAudioSource.volume = GetMasterVolume() * walkVolume;
        walkAudioSource.pitch = 1f;

        Debug.Log("?? Walk AudioSource - Loop: " + walkAudioSource.loop);

        // Beep AudioSource
        beepAudioSource = gameObject.AddComponent<AudioSource>();
        beepAudioSource.playOnAwake = false;
        beepAudioSource.volume = GetMasterVolume() * beepVolume; 

        ScheduleNextBeep();
    }

    void Update()
    {
        UpdateWalkSound();
        UpdateBeepSound();
    }

    void UpdateWalkSound()
    {
        if (walkAudioSource == null) return;

        // HAREKET KÝLÝDÝ - SES ÇALMA!
        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            // Hareket kapalý, sesi durdur
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
            return;
        }

        // INPUT KONTROLÜ
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Herhangi bir tuþa basýlý mý?
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        // DEBUG (her saniyede bir)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("?? Input - H: " + horizontal.ToString("F2") + " V: " + vertical.ToString("F2") + " Moving: " + isMoving);
        }

        if (isMoving)
        {
            // Hareket var - ses çal
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
                Debug.Log("?? WALK SESÝ BAÞLADI!");
            }

            // MASTER VOLUME
            float masterVolume = GetMasterVolume();

            // Input gücüne göre volume ayarla
            float inputStrength = Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical));
            walkAudioSource.volume = masterVolume * walkVolume * inputStrength; 

            // Pitch hafif varyasyon 
            walkAudioSource.pitch = 0.95f + (inputStrength * 0.1f);
        }
        else
        {
            // Hareket yok - ses durdur
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
                Debug.Log("?? WALK SESÝ DURDU!");
            }
        }
    }

    void UpdateBeepSound()
    {
        if (beepAudioSource == null || beepSound == null) return;

        if (Time.time >= nextBeepTime)
        {
            // MASTER VOLUME
            float masterVolume = GetMasterVolume();

            beepAudioSource.PlayOneShot(beepSound, masterVolume * beepVolume); 
            Debug.Log("?? Výýýn! Volume: " + (masterVolume * beepVolume));

            ScheduleNextBeep();
        }
    }

    void ScheduleNextBeep()
    {
        nextBeepTime = Time.time + Random.Range(minBeepInterval, maxBeepInterval);
    }

    // YENÝ FONKSÝYON
    float GetMasterVolume()
    {
        // AudioManager'dan master volume al
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            return AudioManager.Instance.audioSource.volume;
        }

        // AudioManager yoksa varsayýlan
        return 1f;
    }
}