using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    public static PauseManager Instance; // ? EKLE!

    void Awake()
    {
        // Singleton ? EKLE!
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        // Panel baþlangýçta kapalý
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    void Update()
    {
        // ESC tuþuna basýldý mý?
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                // Settings açýksa, Pause'a dön
               CloseSettings();
            }
            else if (isPaused)
            {
                // Pause açýksa, oyuna dön
                Resume();
            }
            else
            {
                // Hiçbiri açýk deðilse, Pause aç
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true; // ? EKLE!

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("?? Oyun duraklatýldý");
    }

    public void Resume()
    {
        isPaused = false; // ? EKLE!

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("?? Oyun devam ediyor");

        // Ses çal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void OpenSettings()
    {
        // Pause panel'i gizle
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Settings panel'i aç
        StartCoroutine(OpenSettingsDelayed());

        Debug.Log("?? Ayarlar açýlýyor...");

        // Ses çal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // SettingsManager'ý çaðýr ? GÜNCELLE! ?
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager != null)
        {
            settingsManager.OpenSettings();
            Debug.Log("?? Settings açýldý");
        }
    }

    System.Collections.IEnumerator OpenSettingsDelayed()
    {
        yield return null; // Bir frame bekle

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        // Settings panel'i kapat
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Pause panel'i tekrar aç
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Debug.Log("?? Ayarlar kapandý, Pause'a döndü");

        // Ses çal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void QuitGame()
    {
        Debug.Log("?? Oyundan çýkýlýyor...");

        // Ses çal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
