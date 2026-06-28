using UnityEngine;

public class Socket : MonoBehaviour
{
    [Header("Connected Device")]
    public GameObject connectedDevice;

    [Header("Device Information")]
    public string deviceName = "Lamba";
    public float kWhSavings = 15f;

    [Header("Statu")]
    public bool isOn = true;

    public void TurnOff()
    {
        if (connectedDevice != null && isOn)
        {
            isOn = false;

            Light lightComponent = connectedDevice.GetComponent<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = false;
            }
        }
    }

    public bool IsWasteful()
    {
        return isOn;
    }

    public string GetDeviceInfo()
    {
        return deviceName + "\n" + kWhSavings + " kWh saving";
    }
}
