using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    public AudioSource musicSource;

    void Start()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
        UpdateVolume();
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = AudioManager.masterVolume;
        }
    }
}