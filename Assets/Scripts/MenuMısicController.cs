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

        // İlk volume'u ayarla
        UpdateVolume();
    }

    void Update()
    {
        // Sürekli master volume'u uygula ✅
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