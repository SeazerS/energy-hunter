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
    public int initialScanUses = 2; // Inspector'dan ayarla (Ev: 2, Okul: 4)
    public float autoScanDuration = 10f; // YENÝ! Ýlk otomatik scan süresi
    public float manualScanDuration = 3f; // TAB scan süresi
                                          // public float scanDuration = 10f;
    public TextMeshProUGUI scanTimerText; // Countdown UI
    public TextMeshProUGUI scanHintText; // TAB: Tekrar Tara
    public AudioClip scanSound;

    [Header("Skor")]
    private int closedDevices = 0;
    public int totalDevices = 8;
    private float totalKWh = 0f;

    [Header("Süre")]
    private float gameTime = 0f;
    private bool gameActive = true;

    [Header("Game State")]
    public bool canPlayerMove = false; // 


    // SCAN KONTROL 
    private bool scanActive = false;
    private float scanTimer = 0f;
    private int scanUsesLeft; // 2 kere kullanýlabilir (baþlangýç + 1 TAB)
    private bool isFirstScan = true; //  Ýlk scan mý???

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

        // SÜRE RESET EKLE
        gameTime = 0f;
        gameActive = true;

        // HAREKET KÝLÝTLÝ BAÞLA!
        canPlayerMove = false;


        // Scan hakkýný ayarla 
        scanUsesLeft = initialScanUses;

        UpdateUI();

        // ANA MENÜ MÜZÝÐÝNÝ DURDUR! 
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
        {
            AudioManager.Instance.audioSource.Stop();
            Debug.Log("?? Ana menü müziði durduruldu");
        }

        // SAHNE KONTROLÜ! 
        string currentScene = SceneManager.GetActiveScene().name;

        Debug.Log("Yüklenen sahne: " + currentScene); // ? DEBUG! 

        if (currentScene == "School_Level") // OKUL SEVÝYESÝ
        {
            // OKUL - OTOMATÝK BAÞLAT ?
            canPlayerMove = true;
            Debug.Log(" canPlayerMove = " + canPlayerMove); // DEBUG! ?

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(InitialScan());

            Debug.Log(" Okul seviyesi - Otomatik baþladý!");
        }
        else // EV SEVÝYESÝ (SampleScene)
        {
            // EV - HELP BEKLENÝYOR ?
            canPlayerMove = false;
            Debug.Log("?canPlayerMove = " + canPlayerMove); // ? DEBUG! ?

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log(" Ev seviyesi - Help bekliyor");
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
                    scanTimerText.text = "Tarama: " + Mathf.CeilToInt(scanTimer).ToString() + "s";
                }

                // Süre bitti mi?
                if (scanTimer <= 0f)
                {
                    EndScan();
                }
            }
        }

        // TAB ÝLE TEKRAR KULLAN (1 kere hak varsa)
        if (Input.GetKeyDown(KeyCode.Tab) && scanUsesLeft > 0 && !scanActive && gameActive)
        {
            ActivateScan();
        }
    }

    IEnumerator InitialScan()
    {
        Debug.Log("Oyun baþladý - Ýlk scan 1 saniye sonra!");

        // 1 saniye bekle (oyun yüklenmesi için)
        yield return new WaitForSeconds(1f);

        // Ýlk scan'i baþlat
        ActivateScan();
    }

    public void ActivateScan()
    {
        if (scanActive || scanUsesLeft <= 0) return;

        Debug.Log("Scan baþladý!(Kalan hak: " + (scanUsesLeft - 1) + ")");

        scanActive = true;
        //scanTimer = scanDuration;

        // ÝLK SCAN 
        if (isFirstScan)
        {
            scanTimer = autoScanDuration; // Ýlk scan uzun
            isFirstScan = false;
            Debug.Log("? Otomatik scan: " + autoScanDuration + " saniye");
        }
        else
        {
            scanTimer = manualScanDuration; // TAB scan kýsa
            Debug.Log("? Manuel scan: " + manualScanDuration + " saniye");
        }

        scanUsesLeft--; // Hak azalt
                        


        // TÜM GLOWLARI GÖSTER

        ShowAllGlows();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScan();
        }

        // Ses efekti
        if (scanSound != null)
        {
            AudioSource.PlayClipAtPoint(scanSound, Camera.main.transform.position);
        }

        // Timer UI göster
        if (scanTimerText != null)
        {
            scanTimerText.gameObject.SetActive(true);
        }

        // Hint UI güncelle
        UpdateScanHint();




    }

    void EndScan()
    {
        scanActive = false;

        // TÜM GLOW'LARI GÝZLE (yeþiller de!)
        HideAllGlows();

        // Timer UI gizle
        if (scanTimerText != null)
        {
            scanTimerText.gameObject.SetActive(false);
        }

        // Hint UI güncelle
        UpdateScanHint();
    }

    void ShowAllGlows()
    {
        // TÜM cihazlarýn glow'larýný göster
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            if (device.glowIndicator != null)
            {
                device.glowIndicator.SetActive(true);
            }
        }

        Debug.Log("?? " + devices.Length + " cihazýn glow'u gösterildi!");
    }

    void HideAllGlows()
    {
        // TÜM glow'larý gizle (yeþiller de!)
        InteractableDevice[] devices = FindObjectsOfType<InteractableDevice>();

        foreach (InteractableDevice device in devices)
        {
            if (device.glowIndicator != null)
            {
                device.glowIndicator.SetActive(false);
            }
        }

        Debug.Log("TÜM glow'lar gizlendi!");
    }

    void UpdateScanHint()
    {
        // Hint UI güncelle 
        if (scanHintText != null)
        {
            if (scanUsesLeft > 0 && !scanActive)
            {
                scanHintText.text = "[TAB] Tekrar Tara (" + scanUsesLeft + " kere kaldý)";
                scanHintText.gameObject.SetActive(true);
            }
            else if (scanUsesLeft <= 0)
            {
                scanHintText.text = "Tarama hakký bitti";
                scanHintText.gameObject.SetActive(true);

                // 3 saniye sonra gizle
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

        Debug.Log("Kapatýlan: " + closedDevices + "/" + totalDevices);
        Debug.Log("Toplam: " + totalKWh + " kWh");

        UpdateUI();

        if (closedDevices >= totalDevices)
        {
            Debug.Log("SEVÝYE TAMAMLANDI!");
            LevelComplete();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Kapatýlan: " + closedDevices + "/" + totalDevices +
                           "\nTasarruf: " + totalKWh.ToString("F0") + " kWh";
        }
    }

    void LevelComplete()
    {
        gameActive = false;
        scanActive = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // HAREKET KÝLÝTLE!
        canPlayerMove = false;
        Debug.Log("?? Level Complete: Hareket kilitlendi");

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // BAÞARI SESÝ 
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelComplete();
        }

        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        if (energyText != null)
        {
            energyText.text = totalKWh.ToString("F0") + " kWh tasarruf";
        }

        if (timeText != null)
        {
            timeText.text = "\nSüre: " + minutes + ":" + seconds.ToString("00");
        }

        Debug.Log("?? Level Complete ekraný gösterildi!");
    }

    public void RestartLevel()
    {
        Debug.Log("?? Seviye yeniden baþlatýlýyor...");
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
        Debug.Log("?? Ana menüye dönülüyor...");
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
        Debug.Log("?? Sonraki seviye yükleniyor...");

        Time.timeScale = 1f;

        // FADE ÝLE GEÇIÞ!
        if (FadeManager.Instance != null)
        {
            StartCoroutine(LoadNextLevelWithFade());
        }
        else
        {
            // Fade yoksa direkt yükle
            LoadNextLevelDirect();
        }
    }

    // YENÝ FONKSÝYON - Fade ile yükleme
    IEnumerator LoadNextLevelWithFade()
    {
        // Fade In (kararýr)
        yield return FadeManager.Instance.FadeIn();

        // Sahne yükle
        LoadNextLevelDirect();

        // NOT: Fade Out yeni sahne baþladýðýnda FadeManager.Awake() içinde otomatik olur
    }

    // YENÝ FONKSÝYON - Direkt yükleme
    void LoadNextLevelDirect()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log("? Seviye " + nextSceneIndex + " yüklendi");
        }
        else
        {
            Debug.Log("?? Tüm seviyeler tamamlandý!");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void StartFirstScan()
    {
        Debug.Log("?? Help paneli kapandý, ilk scan baþlýyor!");
        StartCoroutine(InitialScan());
    }

    // HelpManager'dan çaðrýlacak (hareket açma) 
    public void EnablePlayerMovement()
    {
        canPlayerMove = true;
        Debug.Log("? Oyuncu hareketi etkinleþtirildi!");
    }

}