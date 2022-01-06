using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRInputManager : MonoBehaviour
{

    [SerializeField]
    SteamVR_Action_Boolean pointAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

    [SerializeField]
    Text user;

    GameObject leftController;

    GameObject rightController;

    // Start is called before the first frame update
    void Start()
    {
        HandCollider[] controllers = FindObjectsOfType<HandCollider>();
        Physics.IgnoreLayerCollision(6, 0, true);

        foreach (HandCollider handCollider in controllers)
        {
            GameObject handController = handCollider.gameObject;
            handController.layer = 6;
            handCollider.fingerColliders.indexColliders[0].gameObject.AddComponent<VRCollider>();
            if (handController.name.Contains("Left"))
                leftController = handController;
            if (handController.name.Contains("Right"))
                rightController = handController;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(leftGrab) && !isLeftGrabbing)
        {
            isLeftGrabbing = true;
            LeftController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().StartGrab();
        }
        else if (!Input.GetKey(leftGrab) && isLeftGrabbing)
        {
            isLeftGrabbing = false;
            LeftController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().EndGrab();
        }
        if (Input.GetKey(leftPoint) && !isLeftPointing)
        {
            isLeftPointing = true;
            LeftController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().StartPoint();
        }
        else if (!Input.GetKey(leftPoint) && isLeftPointing)
        {
            isLeftPointing = false;
            LeftController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().EndPoint();
        }
        if (Input.GetKey(rightGrab) && !isRightGrabbing)
        {
            isRightGrabbing = true;
            RightController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().StartGrab();
        }
        else if (!Input.GetKey(rightGrab) && isRightGrabbing)
        {
            isRightGrabbing = false;
            RightController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().EndGrab();
        }
        if (Input.GetKey(rightPoint) && !isRightPointing)
        {
            isRightPointing = true;
            RightController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().StartPoint();
        }
        else if (!Input.GetKey(rightPoint) && isRightPointing)
        {
            isRightPointing = false;
            RightController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().EndPoint();
        }*/

        if (pointAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            Debug.Log("Started pointing");
            leftController.GetComponentInChildren<VRCollider>().pointing = true;
        } else
        {
            leftController.GetComponentInChildren<VRCollider>().pointing = false;
        }
        if (pointAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            Debug.Log("Started pointing");
            rightController.GetComponentInChildren<VRCollider>().pointing = true;
        }
        else
        {
            rightController.GetComponentInChildren<VRCollider>().pointing = false;
        }


        if (Input.GetKeyDown("f1"))
        {
            //Debug.Log("Pressed F1");
            user.text = "User1";
        }
        else if (Input.GetKeyDown("f2"))
        {
            //Debug.Log("Pressed F2");
            user.text = "User2";
        }

        if (Input.GetKeyDown("p"))
        {
            FindObjectOfType<SerialUISummoner>().showing = !(FindObjectOfType<SerialUISummoner>().showing);
        }
    }
}
