using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR.InteractionSystem;

public class Initiator : MonoBehaviour
{
    [SerializeField]
    GameObject MockHMDPrefab;

    [SerializeField]
    GameObject SteamVRPrefab;

    [SerializeField]
    GameObject eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (XRSettings.isDeviceActive && XRSettings.loadedDeviceName == "MockHMD Display")
        {
            Instantiate(MockHMDPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Debug.Log("MockHMD Display");
        } else
        {
            eventSystem.SetActive(false);
            //Debug.LogError("SteamVR not yet implemented");
            Instantiate(SteamVRPrefab, new Vector3(0,0,0), Quaternion.identity);

            HandCollider handCollider = FindObjectOfType<HandCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
