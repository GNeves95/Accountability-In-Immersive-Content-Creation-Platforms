using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    KeyCode leftController = KeyCode.LeftShift;
    [SerializeField]
    KeyCode leftGrab = KeyCode.G;
    [SerializeField]
    KeyCode leftPoint = KeyCode.Z;
    [SerializeField]
    KeyCode rightController = KeyCode.Space;
    [SerializeField]
    KeyCode rightGrab = KeyCode.H;
    [SerializeField]
    KeyCode rightPoint = KeyCode.X;
    [SerializeField]
    GameObject LeftController;
    [SerializeField]
    GameObject RightController;
    [SerializeField]
    GameObject Camera;

    [SerializeField]
    Text user;

    [SerializeField]
    float rotationSpeed = 1000;

    public bool isLeftGrabbing = false;
    public bool isLeftPointing = false;
    public bool isRightGrabbing = false;
    public bool isRightPointing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");
        float mouseScroll = Input.mouseScrollDelta.y;
        bool leftMouse = Input.GetMouseButton(0);
        bool rightMouse = Input.GetMouseButton(1);
        bool midMouse = Input.GetMouseButton(2);
        //Vector3 cameraMov = Camera.transform.position;
        //Vector3 newMov = new Vector3(deltaX, deltaY, mouseScroll);
        if (Input.GetKey(leftController))
        {
            if (leftMouse)
            {
                LeftController.transform.Rotate(new Vector3(-deltaY * Time.deltaTime * rotationSpeed, deltaX * Time.deltaTime * rotationSpeed, mouseScroll * Time.deltaTime * rotationSpeed), Space.Self);
            }
            else
            {
                LeftController.transform.Translate(new Vector3(deltaX * Time.deltaTime, deltaY * Time.deltaTime, mouseScroll * Time.deltaTime), Space.World);
            }
        }
        if (Input.GetKey(rightController))
        {
            if (leftMouse)
            {
                RightController.transform.Rotate(new Vector3(-deltaY * Time.deltaTime * rotationSpeed, deltaX * Time.deltaTime * rotationSpeed, mouseScroll * Time.deltaTime * rotationSpeed), Space.Self);
            }
            else
            {
                RightController.transform.Translate(new Vector3(deltaX * Time.deltaTime, deltaY * Time.deltaTime, mouseScroll * Time.deltaTime), Space.World);
            }
        }
        if (rightMouse)
        {
            //TODO: For some reason, the transformations on the camera do not hold. This isn't crucial for what I'm doing, so I'll leave this for later, but need to leave this ToDo, otherwise I'll forget
            if (leftMouse)
            {
                Camera.transform.Rotate(new Vector3(-deltaY * Time.deltaTime * rotationSpeed, deltaX * Time.deltaTime * rotationSpeed, mouseScroll * Time.deltaTime * rotationSpeed), Space.Self);
            } else
            {
                Camera.transform.Translate(new Vector3(deltaX * Time.deltaTime, deltaY * Time.deltaTime, mouseScroll * Time.deltaTime), Space.World);
            }
        }
        if (Input.GetKey(leftGrab) && !isLeftGrabbing)
        {
            isLeftGrabbing = true;
            LeftController.transform.GetChild(0).gameObject.GetComponent<collisionDetection>().StartGrab();
        } else if (!Input.GetKey(leftGrab) && isLeftGrabbing)
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
            Object.FindObjectOfType<SerialUISummoner>().showing = !(Object.FindObjectOfType<SerialUISummoner>().showing);
        }
    }
}
