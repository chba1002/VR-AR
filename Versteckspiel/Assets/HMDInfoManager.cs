using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HMDInfoManager : MonoBehaviour
{
    void Start()
    {
        if (!XRSettings.isDeviceActive)
        {
            Debug.Log("Keine VR-Brille angeschlossen");
        }
        else if (XRSettings.isDeviceActive && XRSettings.loadedDeviceName == "Mock HMD" 
            || XRSettings.loadedDeviceName == "MockHMD Display")
        {
            Debug.Log("Unity Testumgebung für VR 'Mock HMD' wird verwemdet");
        }
        else
        {
            Debug.Log($"VR-Brille {XRSettings.loadedDeviceName} gefunden");
        }
    }

}
