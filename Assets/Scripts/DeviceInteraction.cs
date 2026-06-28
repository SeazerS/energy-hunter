using UnityEngine;
using TMPro;

public class DeviceInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI deviceNameText;
    public TextMeshProUGUI deviceInfoText;

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    private int closedDevices = 0;
    private int totalDevices = 8;

    [Header("Robot")]
    public Animator robotAnimator;

    private Socket currentSocket;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        UpdateScore();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckClick();
    }

    void CheckClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Socket"))
            {
                Socket socket = hit.collider.GetComponent<Socket>();
                if (socket != null && socket.IsWasteful())
                    ShowPopup(socket);
            }
        }
    }

    void ShowPopup(Socket socket)
    {
        currentSocket = socket;

        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (deviceNameText != null)
            deviceNameText.text = socket.deviceName;

        if (deviceInfoText != null)
            deviceInfoText.text = "Korundu: " + socket.kWhSavings + " kWh\nDurum: Gerekli kapama.";
    }

    public void CloseDevice()
    {
        if (currentSocket == null) return;

        if (robotAnimator != null)
            robotAnimator.SetTrigger("Interact");

        currentSocket.TurnOff();
        closedDevices++;
        UpdateScore();

        if (popupPanel != null)
            popupPanel.SetActive(false);

        currentSocket = null;
        CheckLevelComplete();
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Kapatýldý: " + closedDevices + "/" + totalDevices;
    }

    void CheckLevelComplete()
    {
        if (closedDevices >= totalDevices)
            Debug.Log("Seviye Tamamlandý!");
    }
}