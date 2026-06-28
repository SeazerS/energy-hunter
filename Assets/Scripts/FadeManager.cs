using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("Fade Panel")]
    public CanvasGroup fadeCanvasGroup; 
    public float fadeDuration = 1f; // Fade Time

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
        }

        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    public IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;

        Debug.Log("? Fade In Complated (Alpha: " + fadeCanvasGroup.alpha + ")");
    }

    // Fade In ? Action ? Fade Out
    public IEnumerator FadeInAndOut(System.Action onComplete)
    {
        yield return FadeIn(); 
        onComplete?.Invoke(); // Action 
        yield return FadeOut();
    }
}
