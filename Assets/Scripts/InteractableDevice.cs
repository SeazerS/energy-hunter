using UnityEngine;
using TMPro;
using System.Collections;

public class InteractableDevice : MonoBehaviour
{
    [Header("Device Information")]
    public string deviceName = "Cihaz";
    public float kWhSavings = 10f;
    public bool isOn = true;

    [Header("Device Status")]
    public bool isNecessary = false;
    public bool alreadyTurnedOff = false;
    public string DeviceName = "Device";

    [Header("Connected Device")]
    public GameObject connectedDevice;

    public enum DeviceType { Light, EmissionScreen, HideObject, Remote }
    [Header("Device Type")]
    public DeviceType deviceType = DeviceType.Light;

    [Header("Emission Screen")]
    public Material screenMaterial;

    [Header("Material")]
    public Material objectMaterial;
    public float darknessMultiplier = 0.4f;
    private Color originalBaseColor;
    private bool baseColorSaved = false;

    [Header("Computer Screen Material Replacement")]
    public bool hasScreenMaterial = false;
    public Renderer screenRenderer;
    public int screenMaterialIndex = 1;
    public Material screenOnMaterial;
    public Material screenOffMaterial;

    [Header("Animation Settings")]
    public float interactionDelay = 0.4f;

    [Header("Feedback")]
    public GameObject glowIndicator;
    public ParticleSystem turnOffEffect;
    public AudioClip turnOffSound;

    [Header("UI")]
    public GameObject interactPopup;
    public TextMeshProUGUI popupText;
    public GameObject keyIcon;

    private bool playerInRange = false;
    private bool isInteracting = false;
    private Color originalEmission;
    private bool emissionSaved = false;

    void Start()
    {
        if (objectMaterial != null && objectMaterial.HasProperty("_BaseColor"))
        {
            Color currentBaseColor = objectMaterial.GetColor("_BaseColor");
            float brightness = currentBaseColor.r + currentBaseColor.g + currentBaseColor.b;
            originalBaseColor = brightness < 2.5f ? Color.white : currentBaseColor;
            baseColorSaved = true;

            if (isOn)
                objectMaterial.SetColor("_BaseColor", originalBaseColor);
            else
            {
                Color darkColor = originalBaseColor * darknessMultiplier;
                darkColor.a = originalBaseColor.a;
                objectMaterial.SetColor("_BaseColor", darkColor);
            }
        }

        if (deviceType == DeviceType.EmissionScreen && screenMaterial != null && screenMaterial.HasProperty("_EmissionColor"))
        {
            Color currentEmission = screenMaterial.GetColor("_EmissionColor");
            originalEmission = (currentEmission == Color.black || currentEmission.maxColorComponent < 0.1f)
                ? new Color(0.29f, 0.56f, 0.89f) * 2f
                : currentEmission;
            emissionSaved = true;

            screenMaterial.SetColor("_EmissionColor", isOn ? originalEmission : Color.black);
        }

        if (deviceType == DeviceType.Light && connectedDevice != null)
        {
            Light lightComponent = connectedDevice.GetComponent<Light>();
            if (lightComponent != null)
                lightComponent.enabled = isOn;
        }

        if (glowIndicator != null)
            glowIndicator.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isInteracting)
        {
            if (!isNecessary && !alreadyTurnedOff && isOn)
                StartCoroutine(InteractWithDelay());
            else
                TurnOff();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (interactPopup != null)
        {
            interactPopup.SetActive(true);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPopup();

            if (popupText != null)
            {
                if (isNecessary)
                {
                    popupText.text = deviceName + " Gerekli cihaz \nkapama!";
                    if (keyIcon != null) keyIcon.SetActive(false);
                }
                else if (alreadyTurnedOff)
                {
                    popupText.text = deviceName + " Zaten kapalý \ndokunma!";
                    if (keyIcon != null) keyIcon.SetActive(false);
                }
                else if (isOn)
                {
                    popupText.text = deviceName + "\n" + kWhSavings + " kWh";
                    if (keyIcon != null) keyIcon.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactPopup != null)
        {
            interactPopup.SetActive(false);
            if (keyIcon != null) keyIcon.SetActive(true);
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
        if (isNecessary)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
            return;
        }

        if (alreadyTurnedOff)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
            return;
        }

        if (connectedDevice != null && isOn)
        {
            isOn = false;

            switch (deviceType)
            {
                case DeviceType.Light:
                    Light lightComponent = connectedDevice.GetComponent<Light>();
                    if (lightComponent != null) lightComponent.enabled = false;
                    break;
                case DeviceType.EmissionScreen:
                    if (screenMaterial != null) screenMaterial.SetColor("_EmissionColor", Color.black);
                    break;
                case DeviceType.HideObject:
                    connectedDevice.SetActive(false);
                    break;
            }

            if (hasScreenMaterial && screenRenderer != null)
                ChangeScreenMaterial(false);

            if (objectMaterial != null && baseColorSaved)
            {
                Color darkColor = originalBaseColor * darknessMultiplier;
                darkColor.a = originalBaseColor.a;
                objectMaterial.SetColor("_BaseColor", darkColor);
            }

            if (glowIndicator != null && !isNecessary && !alreadyTurnedOff)
                glowIndicator.SetActive(false);

            if (interactPopup != null)
                interactPopup.SetActive(false);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayDeviceTurnOff();

            if (GameManager.Instance != null)
                GameManager.Instance.DeviceClosed(kWhSavings);
        }
    }

    void ChangeScreenMaterial(bool turnOn)
    {
        if (screenRenderer == null) return;

        Material[] materials = screenRenderer.materials;
        if (screenMaterialIndex >= materials.Length) return;

        materials[screenMaterialIndex] = turnOn ? screenOnMaterial : screenOffMaterial;
        screenRenderer.materials = materials;
    }
}