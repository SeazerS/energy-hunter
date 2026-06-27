using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("Fade Panel")]
    public CanvasGroup fadeCanvasGroup; // FadePanel'in CanvasGroup'u
    public float fadeDuration = 1f; // Fade süresi (saniye)

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arasý kalýcý
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Baþlangýçta fade açýk (ekran kararýk)
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f; // Tam kararýk
        }

        // Oyun baþladýðýnda fade out yap (açýlýr)
        StartCoroutine(FadeOut());
    }

    // Fade Out: Ekran açýlýr (kararýktan açýða)
    public IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // Time.timeScale'den baðýmsýz
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);

        Debug.Log("? Fade Out tamamlandý");
    }

    // Fade In: Ekran kararýr (açýktan kararýða)
    public IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration); // ? 1f = TAM OPAK
            yield return null;
        }

        // SON FRAME TAM 1 OLSUN! ? EKLE
        fadeCanvasGroup.alpha = 1f; // ? ÖNEMLÝ!

        Debug.Log("? Fade In tamamlandý (Alpha: " + fadeCanvasGroup.alpha + ")");
    }

    // Fade In ? Action ? Fade Out
    public IEnumerator FadeInAndOut(System.Action onComplete)
    {
        yield return FadeIn(); // Kararýr
        onComplete?.Invoke(); // Action çalýþtýr (sahne yükle)
        yield return FadeOut(); // Açýlýr
    }
}
