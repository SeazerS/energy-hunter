using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip levelComplete;
    public AudioClip errorSound;
    public AudioClip popupOpen;

    [Header("Game Sounds")]
    public AudioClip deviceTurnOff;
    public AudioClip scanSound;

    [Header("Audio Source")]
    public AudioSource audioSource;


    [Header("Music")]
    public AudioClip menuMusic;

    // GLOBAL MASTER VOLUME 
    public static float masterVolume = 1f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // AudioSource ekle
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        // Müzik çal
        if (menuMusic != null && audioSource.clip == null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // MÜZÝK VOLUME YÜKLE
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SetMusicVolume(savedVolume);
    }

    // Genel ses çalma fonksiyonu
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            // Ana ses ile çarp!
            float finalVolume = volume * AudioListener.volume;
            audioSource.PlayOneShot(clip, finalVolume);
        }
    }

    // UI Ses fonksiyonlarý
    public void PlayButtonClick()
    {
        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick);
        }
    }

    public void PlayButtonHover()
    {
        if (buttonHover != null)
        {
            audioSource.PlayOneShot(buttonHover);
        }
    }

    public void PlayLevelComplete()
    {
        if (levelComplete != null)
        {
            audioSource.PlayOneShot(levelComplete);
        }
    }

    public void PlayError()
    {
        if (errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    public void PlayPopup()
    {
        if (popupOpen != null)
        {
            audioSource.PlayOneShot(popupOpen);
        }
    }

    // Game ses fonksiyonlarý
    public void PlayDeviceTurnOff()
    {
        if (deviceTurnOff != null)
        {
            audioSource.PlayOneShot(deviceTurnOff);
        }
    }

    public void PlayScan()
    {
        if (scanSound != null)
        {
            audioSource.PlayOneShot(scanSound);
        }
    }

    /*internal string FadeOutMusic()
    {
        throw new NotImplementedException();
    }*/

    public void SetMusicVolume(float volume)
    {
        float volumeMultiplier = 3f;
        float finalVolume = volume * volumeMultiplier;
        finalVolume = Mathf.Clamp(finalVolume, 0f, 1f);

        // TÜM AUDIOSOURCE
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            source.volume = finalVolume;
            Debug.Log("?? " + source.gameObject.name + " ? " + source.volume);
        }
    }

    // TÜM AUDIOSOURCE'LARI GÜNCELLE ? YENÝ! ?
    void UpdateAllAudioSources()
    {
        // Sahnedeki TÜM AudioSource'larý bul
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            source.volume = masterVolume;
        }

        Debug.Log("?? " + allAudioSources.Length + " AudioSource güncellendi");
    }

}
