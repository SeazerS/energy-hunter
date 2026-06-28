using UnityEngine;

public class HelpButtonPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;
    public float pulseScale = 1.3f;

    private Vector3 originalScale;
    private bool isPulsing = true;

    void Start()
    {
        originalScale = transform.localScale;

        //First Controller
        if (PlayerPrefs.GetInt("HelpOpened", 0) == 1)
        {
            isPulsing = false;
        }
    }

    void Update()
    {
        if (isPulsing)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
            transform.localScale = originalScale * scale;
        }
    }

    public void StopPulse()
    {
        isPulsing = false;
        transform.localScale = originalScale;

        PlayerPrefs.SetInt("HelpOpened", 1);
        PlayerPrefs.Save();
    }
}
