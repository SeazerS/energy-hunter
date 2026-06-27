using UnityEngine;
using TMPro;

public class DeviceInteraction : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public GameObject popupPanel;
    public TextMeshProUGUI deviceNameText;
    public TextMeshProUGUI deviceInfoText;

    [Header("Skor")]
    public TextMeshProUGUI scoreText;
    private int closedDevices = 0;
    private int totalDevices = 8;

    [Header("Robot")]
    public Animator robotAnimator;

    private Socket currentSocket;

    void Start()
    {
        Debug.Log("? DeviceInteraction baþladý!");

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        UpdateScore();
    }

    void Update()
    {
        // Sol týk
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("??? Mouse týklandý!"); // ? DEBUG
            CheckClick();
        }
    }

    void CheckClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log("?? Raycast gönderiliyor..."); // ? DEBUG

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("? Çarpma: " + hit.collider.name); // ? DEBUG
            Debug.Log("??? Tag: " + hit.collider.tag); // ? DEBUG

            // Prize týklandý mý?
            if (hit.collider.CompareTag("Socket"))
            {
                Debug.Log("?? Socket bulundu!"); // ? DEBUG

                Socket socket = hit.collider.GetComponent<Socket>();

                if (socket != null)
                {
                    Debug.Log("? Socket component var!"); // ? DEBUG

                    if (socket.IsWasteful())
                    {
                        Debug.Log("?? Cihaz gereksiz, popup açýlýyor!"); // ? DEBUG
                        ShowPopup(socket);
                    }
                    else
                    {
                        Debug.Log("? Cihaz zaten kapalý!"); // ? DEBUG
                    }
                }
                else
                {
                    Debug.LogError("? Socket component YOK!"); // ? DEBUG
                }
            }
            else
            {
                Debug.Log("?? Socket deðil, baþka bir þey: " + hit.collider.tag);
            }
        }
        else
        {
            Debug.Log("? Hiçbir þeye çarpmadý!"); // ? DEBUG
        }
    }

    void ShowPopup(Socket socket)
    {
        currentSocket = socket;

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            Debug.Log("?? Popup açýldý!");
        }
        else
        {
            Debug.LogWarning("?? Popup Panel baðlý deðil!");
        }

        if (deviceNameText != null)
        {
            deviceNameText.text = socket.deviceName;
        }

        if (deviceInfoText != null)
        {
            deviceInfoText.text = "Tasarruf: " + socket.kWhSavings + " kWh\nDurum: Gereksiz açýk";
        }
    }

    public void CloseDevice()
    {
        if (currentSocket != null)
        {
            Debug.Log("?? Cihaz kapatýlýyor..."); // ? DEBUG

            // Animasyon tetikle
            if (robotAnimator != null)
            {
                robotAnimator.SetTrigger("Interact");
            }

            // Cihazý kapat
            currentSocket.TurnOff();

            // Skor artýr
            closedDevices++;
            UpdateScore();

            // Popup kapat
            if (popupPanel != null)
            {
                popupPanel.SetActive(false);
            }

            currentSocket = null;

            CheckLevelComplete();
        }
    }

    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Kapatýlan: " + closedDevices + "/" + totalDevices;
        }
    }

    void CheckLevelComplete()
    {
        if (closedDevices >= totalDevices)
        {
            Debug.Log("?? SEVÝYE TAMAMLANDI!");
        }
    }
}
