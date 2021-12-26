using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Initiator : MonoBehaviour
{
    [SerializeField]
    GameObject MockHMDPrefab;

    [SerializeField]
    GameObject SteamVRPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (XRSettings.isDeviceActive && XRSettings.loadedDeviceName == "MockHMD Display")
        {
            Instantiate(MockHMDPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Debug.Log("MockHMD Display");
        } else
        {
            Debug.LogError("SteamVR not yet implemented");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
