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

        if (GameManager.Instance != null && !GameManager.Instance.canPlayerMove)
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
            return;
        }

        // INPUT
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        if (isMoving)
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }

            // MASTER VOLUME
            float masterVolume = GetMasterVolume();

            float inputStrength = Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical));
            walkAudioSource.volume = masterVolume * walkVolume * inputStrength; 

            walkAudioSource.pitch = 0.95f + (inputStrength * 0.1f);
        }
        else
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
    }

    void UpdateBeepSound()
    {
        if (beepAudioSource == null || beepSound == null) return;

        if (Time.time >= nextBeepTime)
        {
            float masterVolume = GetMasterVolume();

            beepAudioSource.PlayOneShot(beepSound, masterVolume * beepVolume); 

            ScheduleNextBeep();
        }
    }

    void ScheduleNextBeep()
    {
        nextBeepTime = Time.time + Random.Range(minBeepInterval, maxBeepInterval);
    }

    float GetMasterVolume()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            return AudioManager.Instance.audioSource.volume;
        }
        return 1f;
    }
}