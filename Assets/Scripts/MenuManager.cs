using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("? MenuManager baþlatýldý");

        // Cursor göster
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ANA MENÜ MÜZÝÐÝNÝ BAÞLAT! ? YENÝ! ?
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            // Eðer müzik çalmýyorsa baþlat
            if (!AudioManager.Instance.audioSource.isPlaying)
            {
                AudioManager.Instance.audioSource.Play();
                Debug.Log("?? Ana menü müziði baþlatýldý");
            }

            // Volume'u ayarla
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            AudioManager.Instance.SetMusicVolume(savedVolume);
        }
    }

    public void StartGame()
    {
        Debug.Log("?? StartGame çaðrýldý!");

        

        // Fade ile sahne geçiþi
        if (FadeManager.Instance != null)
        {
            StartCoroutine(PlayGameWithFade());
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
            Debug.Log("?? Scene yükleniyor: SampleScene"); // ? EKLE! ?
        }
    }

    IEnumerator PlayGameWithFade()
    {
        yield return FadeManager.Instance.FadeIn();
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("?? Oyundan çýkýlýyor...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}