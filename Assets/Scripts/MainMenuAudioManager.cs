using UnityEngine;

public class MainMenuAudioManager : MonoBehaviour
{
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClick, 0.7f);
    }

    public void PlayButtonHover()
    {
        PlaySound(buttonHover, 0.5f);
    }
}
