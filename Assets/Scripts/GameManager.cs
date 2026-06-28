using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI timeText;

    [Header("Scan Ability")]
    public int initialScanUses = 2;
    public float autoScanDuration = 10f;
    public float manualScanDuration = 3f;
                                         
    public TextMeshProUGUI scanTimerText;
    public TextMeshProUGUI scanHintText;
    public AudioClip scanSound;

    [Header("Score")]
    private int closedDevices = 0;
    public int totalDevices = 8;
    private float totalKWh = 0f;

    [Header("Time")]
    private float gameTime = 0f;
    private bool gameActive = true;

    [Header("Game State")]
    public bool canPlayerMove = false; 


    // SCAN Control
    private bool scanActive = false;
    private float scanTimer = 0f;
    private int scanUsesLeft;
    private bool isFirstScan = true;

    void Awake()
    {
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
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        //Time Reset
        gameTime = 0f;
        gameActive = true;

        canPlayerMove = false;
 
        scanUsesLeft = initialScanUses;

        UpdateUI();
 
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            AudioManager.Instance.audioSource.Stop();
        }

        string currentScene = SceneManager.GetActiveScene().name;

        Debug.Log("Yüklenen sahne: " + currentScene);

        if (currentScene == "School_Level")
        {
            canPlayerMove = true;
            Debug.Log(" canPlayerMove = " + canPlayerMove);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(InitialScan());
        }
        else
        {
            canPlayerMove = false;
            Debug.Log("?canPlayerMove = " + canPlayerMove);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Update()
    {
        if (gameActive)
        {
            gameTime += Time.deltaTime;

            // SCAN TIMER
            if (scanActive)
            {
                scanTimer -= Time.deltaTime;

                // Countdown UI
                if (scanTimerText != null)
                {
                    scanTimerText.text = "Scanning: " + Mathf.CeilToInt(scanTimer).ToString() + "s";
                }

                if (scanTimer <= 0f)
                {
                    EndScan();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && scanUsesLeft > 0 && !scanActive && gameActive)
        {
            ActivateScan();
        }
    }

    IEnumerator InitialScan()
    {
        yield return new WaitForSeconds(1f);

        // First Scan
        ActivateScan();
    }

    public void ActivateScan()
    {
        if (scanActive || scanUsesLeft <= 0) return;

        scanActive = true;
        //scanTimer = scanDuration;

        // First SCAN 
        if (isFirstScan)
        {
            scanTimer = autoScanDuration;
            isFirstScan = false;
        }
        else
        {
            scanTimer = manualScanDuration; // TAB scan
        }

        scanUsesLeft--;
                        


        //Show All glows

        ShowAllGlows();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScan();
        }

        //Sound effect
        if (scanSound != null)
        {
            AudioSource.PlayClipAtPoint(scanSound, Camera.main.transform.position);
        }

        //Show Timer UI
        if (scanTimerText != null)
        {
            scanTimerText.gameObject.SetActive(true);
        }

        // Hint UI
        UpdateScanHint();




    }

    void EndScan()
    {
        scanActive = false;

        HideAllGlows();

        if (scanTimerText != null)
        {
            scanTimerText.gameObject.SetActive(false);
        }

        // Hint UI
        UpdateScanHint();
    }

    void ShowAllGlows()
    {
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            if (device.glowIndicator != null)
            {
                device.glowIndicator.SetActive(true);
            }
        }
    }

    void HideAllGlows()
    {
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            if (device.glowIndicator != null)
            {
                device.glowIndicator.SetActive(false);
            }
        }
    }

    void UpdateScanHint()
    {
        if (scanHintText != null)
        {
            if (scanUsesLeft > 0 && !scanActive)
            {
                scanHintText.text = "[TAB] Yeniden tara (" + scanUsesLeft + " kaldý.)";
                scanHintText.gameObject.SetActive(true);
            }
            else if (scanUsesLeft <= 0)
            {
                scanHintText.text = "Tarama bitti.";
                scanHintText.gameObject.SetActive(true);

                StartCoroutine(HideHintAfterDelay(3f));
            }
            else
            {
                scanHintText.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (scanHintText != null)
        {
            scanHintText.gameObject.SetActive(false);
        }
    }

    public void DeviceClosed(float kWh)
    {
        closedDevices++;
        totalKWh += kWh;

        Debug.Log("Kapatýldý: " + closedDevices + "/" + totalDevices);
        Debug.Log("Toplam: " + totalKWh + " kWh");

        UpdateUI();

        if (closedDevices >= totalDevices)
        {
            LevelComplete();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Kapatýldý: " + closedDevices + "/" + totalDevices +
                           "\nKorundu: " + totalKWh.ToString("F0") + " kWh";
        }
    }

    void LevelComplete()
    {
        gameActive = false;
        scanActive = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Lock the movemant!
        canPlayerMove = false;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelComplete();
        }

        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        if (energyText != null)
        {
            energyText.text = totalKWh.ToString("F0") + " kWh korundu";
        }

        if (timeText != null)
        {
            timeText.text = "\nZaman: " + minutes + ":" + seconds.ToString("00");
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            StartCoroutine(RestartLevelWithFade());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator RestartLevelWithFade()
    {
        yield return FadeManager.Instance.FadeIn();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            StartCoroutine(LoadMainMenuWithFade());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator LoadMainMenuWithFade()
    {
        yield return FadeManager.Instance.FadeIn();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            StartCoroutine(LoadNextLevelWithFade());
        }
        else
        {
            LoadNextLevelDirect();
        }
    }

    IEnumerator LoadNextLevelWithFade()
    {
        yield return FadeManager.Instance.FadeIn();

        LoadNextLevelDirect();
    }

    void LoadNextLevelDirect()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void StartFirstScan()
    {
        StartCoroutine(InitialScan());
    }

    public void EnablePlayerMovement()
    {
        canPlayerMove = true;
    }

}