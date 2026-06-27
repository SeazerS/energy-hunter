using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using static UnityEngine.Rendering.DebugUI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;

    [Header("Graphics Settings")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;

    [Header("Gameplay Settings")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    void Start()
    {
        // Panel bulamazsa find ile ara
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("SettingsPanel");
            Debug.Log("?? Panel Find ile bulundu: " + settingsPanel);
        }

        LoadSettings();

        // MÜZÝK SLIDER LISTENER ? EKLE! ?
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // Hassasiyet yükle
        if (sensitivitySlider != null)
        {
            float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
            sensitivitySlider.value = savedSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(savedSensitivity);

            Debug.Log("??? Hassasiyet yüklendi: " + savedSensitivity);
        }

    }

    public void OpenSettings()
    {
        Debug.Log("?? OpenSettings çaðrýldý!");
        StartCoroutine(OpenSettingsDelayed());
    }

    System.Collections.IEnumerator OpenSettingsDelayed()
    {
        Debug.Log("? Bir frame bekleniyor...");

        yield return null; // Bir frame bekle

        Debug.Log("? Frame geçti, panel açýlýyor!");

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("?? Panel SetActive(true)");
            Debug.Log("?? Panel activeSelf: " + settingsPanel.activeSelf);
            Debug.Log("?? Panel activeInHierarchy: " + settingsPanel.activeInHierarchy);
        }
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        SaveSettings();

        // Ses çal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        float volume = musicVolumeSlider.value;
        musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";

        // AudioManager'a gönder
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }

        // PlayerPrefs'e kaydet
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        Debug.Log("?? Müzik seviyesi: " + value);
        /*musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";

        // AudioManager'a gönder ? YENÝ! ?
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }

        // PlayerPrefs'e kaydet ? YENÝ! ?
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        Debug.Log("?? Müzik seviyesi: " + volume);*/
    }

    public void OnFullscreenToggled()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnQualityChanged()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.Save();

        Debug.Log("?? Ayarlar kaydedildi!");
    }

    void LoadSettings()
    {
        // Music Volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicVolumeSlider.value = musicVolume;
        musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100) + "%"; // ?

        // Fullscreen
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;

        // Quality
        int quality = PlayerPrefs.GetInt("Quality", 2);
        qualityDropdown.value = quality;
        QualitySettings.SetQualityLevel(quality);

        // Sensitivity
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 5f);
        sensitivitySlider.value = sensitivity;
        sensitivityText.text = sensitivity.ToString("F1");

        Debug.Log("?? Ayarlar yüklendi!");
    }

    void OnSensitivityChanged(float value)
    {
        Debug.Log("?? OnSensitivityChanged çaðrýldý! Value: " + value); // ? DEBUG! ?

        UpdateSensitivityText(value);

        // PlayerPrefs'e kaydet
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        // MouseLook script'ini güncelle
        MouseLook mouseLook = Camera.main.GetComponent<MouseLook>();

        Debug.Log("?? Camera.main: " + Camera.main); // ? DEBUG! ?
        Debug.Log("?? MouseLook: " + mouseLook); // ? DEBUG! ?

        if (mouseLook != null)
        {
            mouseLook.UpdateSensitivity(value);
            Debug.Log("??? Yeni hassasiyet: " + value);
        }
        else
        {
            Debug.LogWarning("?? MouseLook script bulunamadý!"); // ? DEBUG! ?
        }
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = Mathf.RoundToInt(value).ToString();
        }
    }
}
