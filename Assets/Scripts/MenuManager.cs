using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        // Show the Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            if (!AudioManager.Instance.audioSource.isPlaying)
            {
                AudioManager.Instance.audioSource.Play();
            }

            //Settings The Volume
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            AudioManager.Instance.SetMusicVolume(savedVolume);
        }
    }

    public void StartGame()
    {
        if (FadeManager.Instance != null)
        {
            StartCoroutine(PlayGameWithFade());
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator PlayGameWithFade()
    {
        yield return FadeManager.Instance.FadeIn();
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}