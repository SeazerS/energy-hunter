using UnityEngine;

public class MainMenuAudioManager : MonoBehaviour
{
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton YOK! ?
        // DontDestroyOnLoad YOK! ?

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("? AudioSource bulunamadý!");
        }
        else
        {
            Debug.Log("? MainMenuAudioManager hazýr!");
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
            Debug.Log("?? Ses çalýyor: " + clip.name);
        }
    }

    public void PlayButtonClick()
    {
        Debug.Log("?? PlayButtonClick çaðrýldý!");
        PlaySound(buttonClick, 0.7f);
    }

    public void PlayButtonHover()
    {
        Debug.Log("?? PlayButtonHover çaðrýldý!");
        PlaySound(buttonHover, 0.5f);
    }
}
