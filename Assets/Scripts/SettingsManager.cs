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
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("SettingsPanel");
        }

        LoadSettings();

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sensitivitySlider != null)
        {
            float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
            sensitivitySlider.value = savedSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(savedSensitivity);

        }

    }

    public void OpenSettings()
    {
        StartCoroutine(OpenSettingsDelayed());
    }

    System.Collections.IEnumerator OpenSettingsDelayed()
    {
        yield return null;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
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
    }

    void OnSensitivityChanged(float value)
    {
        UpdateSensitivityText(value);

        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        MouseLook mouseLook = Camera.main.GetComponent<MouseLook>();

        if (mouseLook != null)
        {
            mouseLook.UpdateSensitivity(value);
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
