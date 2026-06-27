using UnityEngine;

public class Socket : MonoBehaviour
{
    [Header("Baðlý Cihaz")]
    public GameObject connectedDevice;

    [Header("Cihaz Bilgileri")]
    public string deviceName = "Lamba";
    public float kWhSavings = 15f;

    [Header("Durum")]
    public bool isOn = true;

    public void TurnOff()
    {
        if (connectedDevice != null && isOn)
        {
            isOn = false;

            Debug.Log("?? " + deviceName + " kapatýlýyor!"); // ? DEBUG

            // Light component bul ve söndür
            Light lightComponent = connectedDevice.GetComponent<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = false;
                Debug.Log("?? Light söndürüldü!");
            }
            else
            {
                Debug.LogWarning("?? Light component bulunamadý!");
            }

            Debug.Log("? " + deviceName + " kapatýldý! +" + kWhSavings + " kWh");
        }
        else if (!isOn)
        {
            Debug.Log("? " + deviceName + " zaten kapalý!");
        }
        else
        {
            Debug.LogError("? Connected Device baðlý deðil!");
        }
    }

    public bool IsWasteful()
    {
        return isOn;
    }

    public string GetDeviceInfo()
    {
        return deviceName + "\n" + kWhSavings + " kWh tasarruf";
    }
}
