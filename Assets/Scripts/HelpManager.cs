using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    [Header("Help Panel")]
    public GameObject helpPanel;
    public Button helpButton;
    public Button closeButton;

    [Header("First Time")]
    public bool forceOpenOnStart = true;
    public HelpButtonPulse buttonPulse;

    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isFirstTime = false;


    void Start()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }

        if (helpButton != null)
        {
            helpButton.onClick.AddListener(OnHelpButtonClick);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseHelpPanel);
        }

        if (PlayerPrefs.GetInt("HelpOpened", 0) == 0 && forceOpenOnStart)
        {
            isFirstTime = true;
        }


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelpPanel();
        }

        //Close the ESC
        if (Input.GetKeyDown(KeyCode.Escape) && helpPanel != null && helpPanel.activeSelf)
        {
            CloseHelpPanel();
        }
    }

    void OnHelpButtonClick()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        GameObject arrow = GameObject.Find("ArrowIndicator");
        if (arrow != null)
        {
            arrow.SetActive(false);
        }

        if (buttonPulse != null)
        {
            buttonPulse.StopPulse();
        }

        ToggleHelpPanel();

        if (isFirstTime)
        {
            Debug.Log("First click");
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

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (openSound != null && AudioManager.Instance != null)
            {
                AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);
            }

        }
    }

    public void CloseHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);

            if (isFirstTime)
            {
                PlayerPrefs.SetInt("HelpOpened", 1);
                PlayerPrefs.Save();
                isFirstTime = false;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartFirstScan();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnablePlayerMovement();
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //Play Sound
            if (closeSound != null && AudioManager.Instance != null)
            {
                AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);
            }
        }
    }
}
