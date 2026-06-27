using UnityEngine;

public class HelpButtonPulse : MonoBehaviour
{
    [Header("Pulse Ayarlarý")]
    public float pulseSpeed = 2f;
    public float pulseScale = 1.3f;

    private Vector3 originalScale;
    private bool isPulsing = true;

    void Start()
    {
        originalScale = transform.localScale;

        // Ýlk kez mi kontrol et
        if (PlayerPrefs.GetInt("HelpOpened", 0) == 1)
        {
            // Daha önce açýlmýþ, pulse yapma
            isPulsing = false;
        }
    }

    void Update()
    {
        if (isPulsing)
        {
            // Pulse animasyonu (büyüme-küçülme)
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
            transform.localScale = originalScale * scale;
        }
    }

    public void StopPulse()
    {
        isPulsing = false;
        transform.localScale = originalScale;

        // Bir daha pulse yapma
        PlayerPrefs.SetInt("HelpOpened", 1);
        PlayerPrefs.Save();
    }
}
