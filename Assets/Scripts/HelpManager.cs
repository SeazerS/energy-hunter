using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    [Header("Help Panel")]
    public GameObject helpPanel;
    public Button helpButton;
    public Button closeButton;

    [Header("First Time")]
    public bool forceOpenOnStart = true; // Ýlk açýlýþta zorla aç
    public HelpButtonPulse buttonPulse; // Pulse script referansý

    [Header("Audio (Ýsteðe Baðlý)")]
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isFirstTime = false;


    void Start()
    {
        Debug.Log("? HelpManager.Start() çalýþtý");

        // Panel baþlangýçta kapalý
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }

        // Buton eventleri
        if (helpButton != null)
        {
            helpButton.onClick.AddListener(OnHelpButtonClick);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseHelpPanel);
        }

        // ÝLK KEZ MÝ KONTROL ET ?
        if (PlayerPrefs.GetInt("HelpOpened", 0) == 0 && forceOpenOnStart)
        {
            isFirstTime = true;
            //Invoke("OpenHelpPanel", 1f); // 1 saniye sonra otomatik aç
        }


    }

    void Update()
    {
        // H tuþu ile de açýlabilir (opsiyonel)
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelpPanel();
        }

        // ESC ile kapat (sadece help açýksa)
        if (Input.GetKeyDown(KeyCode.Escape) && helpPanel != null && helpPanel.activeSelf)
        {
            CloseHelpPanel();
        }
    }

    void OnHelpButtonClick()
    {
        Debug.Log("? Help butonu TIKlandý!"); // ? DEBUG! ?

        Debug.Log("? Cursor visible: " + Cursor.visible); // ? DEBUG! ?
        Debug.Log("? Cursor lockState: " + Cursor.lockState); // ? DEBUG! ?

        // SES ÇAL! ? YENÝ! ?
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // OK'U GÝZLE - HER ZAMAN! ? DÜZELTME! ?
        GameObject arrow = GameObject.Find("ArrowIndicator");
        if (arrow != null)
        {
            arrow.SetActive(false);
            Debug.Log("?? Ok gizlendi");
        }

        // PULSE DURDUR (ilk týklamada) ? ZATEN VAR ?
        if (buttonPulse != null)
        {
            buttonPulse.StopPulse();
            Debug.Log("?? Pulse durduruldu");
        }

        ToggleHelpPanel();

        // ÝLK KEZ ÝSE: Sadece kayýt
        if (isFirstTime)
        {
            // Ok ve pulse zaten yukarýda durduruldu ?
            Debug.Log("? Ýlk týklama kaydedildi");
        }
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            bool isActive = helpPanel.activeSelf;

            if (isActive)
            {
                CloseHelpPanel();
            }
            else
            {
                OpenHelpPanel();
            }
        }
    }

    public void OpenHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);

            // Cursor göster
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Oyunu DURDUR (isteðe baðlý)
            // Time.timeScale = 0f;

            // Ses çal
            if (openSound != null && AudioManager.Instance != null)
            {
                AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);
            }

            Debug.Log("? Help paneli açýldý");
        }
    }

    public void CloseHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);

            // Ýlk kez kapatýldý, kaydet
            if (isFirstTime)
            {
                PlayerPrefs.SetInt("HelpOpened", 1);
                PlayerPrefs.Save();
                isFirstTime = false;

                Debug.Log("? Help ilk kez kapatýldý, bir daha otomatik açýlmayacak");
            }

            // ÝLK SCAN'Ý BAÞLAT! ? YENÝ! ?
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartFirstScan();
                Debug.Log("? Help kapatýldý, ilk scan tetiklendi");
            }

            // Hareket aç
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnablePlayerMovement();
            }


            Debug.Log("? Help ilk kez kapatýldý");

            // Cursor gizle (oyun devam ediyorsa)
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;

            // CURSOR KÝLÝTLE! ? YENÝ! ?
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("?? Cursor kilitlendi, oyun baþladý!");

            // Ses çal
            if (closeSound != null && AudioManager.Instance != null)
            {
                AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);
            }

            Debug.Log("? Help paneli kapandý");
        }
    }
}
