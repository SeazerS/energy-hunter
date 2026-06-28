using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public GameObject nextButton;

    [Header("Tutorial Messages")]
    private string[] tutorialMessages = new string[]
    {
        "Hoþ geldin Enerji Avcýsý!\nEnerjini boþa harcayan cihazlarý bul ve kapat!",

        "WASD tuþlarý ile hareket et\nMouse ile etrafýna bak",

        "Cihazlara yaklaþ ve [E] tuþu ile kapat\nKýrmýzý = Çok enerji harcar\nTuruncu = Orta enerji\nYeþil = Gerekli/Kapalý",

        "[TAB] tuþu ile SCAN kullan!\nTüm cihazlarýn yerlerini görebilirsin\n(2 kere kullanabilirsin ve oyun baþlarken ilk scan açýlacak ve sana bütün açýk ve açýk olmayan cihazlarý gösterecek)",

        "Gereksiz tüm cihazlarý kapat!\nAma gerekli olanlarý kapatma!\nBaþarýlar!"
    };

    private int currentStep = 0;
    private bool tutorialActive = true;

    void Start()
    {
        ShowTutorial();
    }

    void ShowTutorial()
    {
        if (currentStep < tutorialMessages.Length)
        {
            tutorialPanel.SetActive(true);
            tutorialText.text = tutorialMessages[currentStep];

            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            EndTutorial();
        }
    }

    public void NextStep()
    {
        currentStep++;
        ShowTutorial();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = false;

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
}
