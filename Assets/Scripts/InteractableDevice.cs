using UnityEngine;
using TMPro; // ? YENÝ! TextMeshPro için
using System.Collections;

public class InteractableDevice : MonoBehaviour
{
    [Header("Cihaz Bilgileri")]
    public string deviceName = "Cihaz";
    public float kWhSavings = 10f;
    public bool isOn = true;

    [Header("Cihaz Durumu")]
    public bool isNecessary = false;
    public bool alreadyTurnedOff = false;
    public string DeviceName = "Cihaz";

    [Header("Baðlý Cihaz")]
    public GameObject connectedDevice;

    public enum DeviceType { Light, EmissionScreen, HideObject, Remote }
    [Header("Cihaz Tipi")]
    public DeviceType deviceType = DeviceType.Light;

    [Header("Emission Screen Ýçin")]
    public Material screenMaterial;

    [Header("Material Renk Deðiþimi")]
    public Material objectMaterial;
    public float darknessMultiplier = 0.4f;
    private Color originalBaseColor;
    private bool baseColorSaved = false;

    [Header("Bilgisayar Ekraný Material Deðiþimi")]
    public bool hasScreenMaterial = false; // ? DEÐÝÞTÝ!
    public Renderer screenRenderer;
    public int screenMaterialIndex = 1; // Element 1
    public Material screenOnMaterial; // ? DEÐÝÞTÝ! (Renkli ekran - Material.003)
    public Material screenOffMaterial; // ? DEÐÝÞTÝ! (Siyah material)

    [Header("Animasyon Ayarý")]
    public float interactionDelay = 0.4f;

    [Header("Görsel Feedback")]
    public GameObject glowIndicator;
    public ParticleSystem turnOffEffect;
    public AudioClip turnOffSound;

    [Header("UI")] // ? YENÝ BÖLÜM!
    public GameObject interactPopup; // Canvas'taki popup
    public TextMeshProUGUI popupText; // Popup içindeki text
    public GameObject keyIcon; // ? YENÝ! KeyIcon referansý


    private bool playerInRange = false;
    private bool isInteracting = false;
    private Color originalEmission;
    private bool emissionSaved = false;

    void Start()
    {    
        // ???????????????????????????????????????????
        // BASE COLOR GERÝ YÜKLEME (Buzdolabý, vs.)
        // ???????????????????????????????????????????
        if (objectMaterial != null)
        {
            if (objectMaterial.HasProperty("_BaseColor"))
            {
                Color currentBaseColor = objectMaterial.GetColor("_BaseColor");

                // Brightness hesapla (RGB toplamý)
                float brightness = currentBaseColor.r + currentBaseColor.g + currentBaseColor.b;

                // DÜZELTME: Threshold 2.5 yap (beyaz = 3.0) ? DEÐÝÞTÝ!
                if (brightness < 2.5f)
                {
                    // Karanlýksa varsayýlan beyaz kullan
                    originalBaseColor = Color.white;
                    Debug.Log("?? " + deviceName + " base color karanlýktý (" + brightness + "), beyaza döndü!");
                }
                else
                {
                    // Zaten parlaksa, bu rengi sakla
                    originalBaseColor = currentBaseColor;
                    Debug.Log("? " + deviceName + " base color parlak (" + brightness + "), saklandý!");
                }

                baseColorSaved = true;

                // Cihaz AÇIKSA (isOn = true), orijinal rengi yükle
                if (isOn)
                {
                    objectMaterial.SetColor("_BaseColor", originalBaseColor);
                    Debug.Log("?? " + deviceName + " base color açýk hale getirildi: " + originalBaseColor);
                }
                else
                {
                    // Kapalýysa karanlýk yap
                    Color darkColor = originalBaseColor * darknessMultiplier;
                    darkColor.a = originalBaseColor.a;
                    objectMaterial.SetColor("_BaseColor", darkColor);
                    Debug.Log("?? " + deviceName + " base color kapalý halde baþladý!");
                }
            }
        }

        // ???????????????????????????????????????????
        // EMISSION GERÝ YÜKLEME (TV, Bilgisayar vs.)
        // ???????????????????????????????????????????
        if (deviceType == DeviceType.EmissionScreen && screenMaterial != null)
        {
            if (screenMaterial.HasProperty("_EmissionColor"))
            {
                Color currentEmission = screenMaterial.GetColor("_EmissionColor");

                // Eðer emission siyahsa (kapalý kalmýþsa), varsayýlan renge dön
                if (currentEmission == Color.black || currentEmission.maxColorComponent < 0.1f)
                {
                    // Varsayýlan mavi emission (TV için)
                    originalEmission = new Color(0.29f, 0.56f, 0.89f) * 2f; // #4A90E2
                    Debug.Log("?? " + deviceName + " emission siyahtý, varsayýlana döndü!");
                }
                else
                {
                    // Zaten renk varsa, bunu sakla
                    originalEmission = currentEmission;
                }

                emissionSaved = true;

                // Cihaz AÇIKSA, emission'ý aktif et
                if (isOn)
                {
                    screenMaterial.SetColor("_EmissionColor", originalEmission);
                    Debug.Log("?? " + deviceName + " emission açýk hale getirildi!");
                }
                else
                {
                    // Kapalýysa siyah yap
                    screenMaterial.SetColor("_EmissionColor", Color.black);
                    Debug.Log("?? " + deviceName + " emission kapalý halde baþladý!");
                }
            }
        }

        // ???????????????????????????????????????????
        // LIGHT GERÝ YÜKLEME (Lambalar)
        // ???????????????????????????????????????????
        if (deviceType == DeviceType.Light && connectedDevice != null)
        {
            Light lightComponent = connectedDevice.GetComponent<Light>();
            if (lightComponent != null)
            {
                // isOn durumuna göre açýk/kapalý yap
                lightComponent.enabled = isOn;

                if (isOn)
                {
                    Debug.Log("?? " + deviceName + " light açýk hale getirildi!");
                }
                else
                {
                    Debug.Log("?? " + deviceName + " light kapalý halde baþladý!");
                }
            }
        }

        // TÜM GLOW'LAR BAÞLANGIÇTA GÝZLÝ ? DEÐÝÞTÝ!
        if (glowIndicator != null)
        {
            glowIndicator.SetActive(false); // Hepsi gizli!
            Debug.Log("? " + deviceName + " glow baþlangýçta gizli");
        }

    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isInteracting)
        {
            Debug.Log("?? E tuþuna basýldý! Device: " + deviceName); // ? EKLE!
            TurnOff();

            if (isNecessary)
            {
                Debug.Log("?? " + deviceName + " gerekli! Kapatýlamaz!");
            }
            else if (alreadyTurnedOff)
            {
                Debug.Log("? " + deviceName + " zaten kapalý!");
            }
            else if (isOn)
            {
                StartCoroutine(InteractWithDelay());
            }
            else
            {
                Debug.Log("? " + deviceName + " zaten kapalý!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPopup != null)
            {
                interactPopup.SetActive(true);

                // POPUP SESÝ (HER CÝHAZDA AYNI) ? DEÐÝÞTÝ!
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPopup();
                    Debug.Log("?? Popup sesi!");
                }


                // Popup text ayarlarý (zaten var)
                if (popupText != null)
                {
                    if (isNecessary)
                    {
                        popupText.text = deviceName + " Gerekli cihaz\nKapatma!";

                        if (keyIcon != null)
                        {
                            keyIcon.SetActive(false);
                        }
                    }
                    else if (alreadyTurnedOff)
                    {
                        popupText.text = deviceName + " Zaten kapalý\nDokunma!";

                        if (keyIcon != null)
                        {
                            keyIcon.SetActive(false);
                        }
                    }
                    else if (isOn)
                    {
                        popupText.text = deviceName + "\n" + kWhSavings + " kWh";

                        if (keyIcon != null)
                        {
                            keyIcon.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("?? " + deviceName + " menzilden çýktý!");

            if (interactPopup != null)
            {
                interactPopup.SetActive(false);

                // ICON TEKRAR GÖSTER (default) ? YENÝ!
                if (keyIcon != null)
                {
                    keyIcon.SetActive(true);
                }
            }
        }
    }

    IEnumerator InteractWithDelay()
    {
        isInteracting = true;

        yield return new WaitForSeconds(interactionDelay);

        TurnOff();

        isInteracting = false;
    }    

    void TurnOff()
    {
        // YEÞÝL CÝHAZLARA E BASILINCA ERROR!

        if (isNecessary)
        {
            // GEREKLI CÝHAZ - KAPATMA!
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
            Debug.Log("?? Gerekli cihaz kapatýlamaz!");
            return; // Fonksiyonu bitir
        }

        if (alreadyTurnedOff)
        {
            // ZATEN KAPALI - DOKUNMA!
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
            Debug.Log("?? Zaten kapalý!");
            return; // Fonksiyonu bitir
        }

        // NORMAL CÝHAZ - KAPAT
        if (connectedDevice != null && isOn)
        {
            isOn = false;

            Debug.Log("?? " + deviceName + " kapatýlýyor!");

            // Cihaz tipine göre kapat
            switch (deviceType)
            {
                case DeviceType.Light:
                    Light lightComponent = connectedDevice.GetComponent<Light>();
                    if (lightComponent != null)
                    {
                        lightComponent.enabled = false;
                    }
                    break;

                case DeviceType.EmissionScreen:
                    if (screenMaterial != null)
                    {
                        screenMaterial.SetColor("_EmissionColor", Color.black);
                    }
                    break;

                case DeviceType.HideObject:
                    connectedDevice.SetActive(false);
                    break;
            }

            // BÝLGÝSAYAR EKRANI MATERIAL DEÐÝÞÝMÝ
            if (hasScreenMaterial && screenRenderer != null) // ? DEÐÝÞTÝ!
            {
                ChangeScreenMaterial(false); // ? DEÐÝÞTÝ!
            }

            // Material karart
            if (objectMaterial != null && baseColorSaved)
            {
                Color darkColor = originalBaseColor * darknessMultiplier;
                darkColor.a = originalBaseColor.a;
                objectMaterial.SetColor("_BaseColor", darkColor);
            }

            // Glow gizle
            if (glowIndicator != null && !isNecessary && !alreadyTurnedOff)
            {
                glowIndicator.SetActive(false);
            }

            // Popup gizle
            if (interactPopup != null)
            {
                interactPopup.SetActive(false);
            }

            // Kapatma sesi
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeviceTurnOff();
                Debug.Log("?? Kapatma sesi çalýyor!");
            }

            // GameManager'a bildir
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DeviceClosed(kWhSavings);
            }
        }
    }

    void ChangeScreenMaterial(bool turnOn) // ? DEÐÝÞTÝ!
    {
        if (screenRenderer == null)
        {
            Debug.LogWarning("?? Screen Renderer atanmamýþ!");
            return;
        }

        Material[] materials = screenRenderer.materials;

        if (screenMaterialIndex >= materials.Length)
        {
            Debug.LogError("? Material index hatalý! Index: " + screenMaterialIndex + ", Toplam: " + materials.Length);
            return;
        }

        if (turnOn)
        {
            // Açýk ekran (renkli material)
            if (screenOnMaterial != null)
            {
                materials[screenMaterialIndex] = screenOnMaterial; // ? DEÐÝÞTÝ!
                Debug.Log("? Ekran açýk material");
            }
        }
        else
        {
            // Kapalý ekran (siyah material)
            if (screenOffMaterial != null)
            {
                materials[screenMaterialIndex] = screenOffMaterial; // ? DEÐÝÞTÝ!
                Debug.Log("? Ekran kapalý material (siyah)");
            }
        }

        screenRenderer.materials = materials; // Material array'i uygula
    }

}