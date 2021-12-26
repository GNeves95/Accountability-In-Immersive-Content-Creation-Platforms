using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collisionDetection : MonoBehaviour
{
    public bool grabbing = false;
    public bool pointing = false;
    public bool scaling = false;
    public GameObject inContact = null;
    public GameObject scaleObj = null; //recquired because it might stop touching
    public Vector3 originalSize;
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    private Vector3 vecDist;

    public float dist = -9999;

    [SerializeField]
    GameObject otherController = null;

    [SerializeField]
    GameObject[] grabJoints;

    [SerializeField]
    GameObject[] pointJoints;

    [SerializeField]
    GameObject[] indexJoints;

    private Text UserBox;

    private TimelineController tc;

    public void StartScale()
    {
        scaling = true;
        scaleObj = inContact;
        Vector3 thisPos = new Vector3(gameObject.transform.parent.transform.position.x, gameObject.transform.parent.transform.position.y, gameObject.transform.parent.transform.position.z);
        Vector3 otherPos = new Vector3(otherController.transform.position.x, otherController.transform.position.y, otherController.transform.position.z);
        dist = Vector3.Distance(gameObject.transform.parent.transform.position, otherController.transform.position);
        vecDist = new Vector3(Mathf.Abs(thisPos.x - otherPos.x), Mathf.Abs(thisPos.y - otherPos.y), Mathf.Abs(thisPos.z - otherPos.z));
        inContact.gameObject.transform.SetParent(null);
    }

    public void EndScale()
    {
        scaling = false;
        scaleObj.gameObject.transform.SetParent(gameObject.transform);
        tc.scaleObject(scaleObj, UserBox.text, DateTime.Now, scaleObj.transform.localScale);
        inContact = scaleObj;
        scaleObj = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Contains("vr_glove_") && !grabbing)
        {
            inContact = other.gameObject;
            //Debug.Log("Trigger: " + other.gameObject.name);
        }
    }

    public void StartPoint()
    {
        pointing = true;
        foreach(GameObject joint in pointJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.grab = true;
            anime.Reset();
        }
        foreach (GameObject joint in indexJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.point = true;
            anime.Reset();
        }
    }

    public void EndPoint()
    {
        pointing = false;
        foreach (GameObject joint in pointJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.grab = false;
            anime.Reset();
        }
        foreach (GameObject joint in indexJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.point = false;
            anime.Reset();
        }
    }

    public void StartGrab()
    {
        //Debug.Log(gameObject.name + ": started grabbing");
        grabbing = true;
        foreach (GameObject joint in grabJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.grab = true;
            anime.Reset();
        }
        if (inContact != null)
        {
            
            /*
             * Here we need to store the initial state of the grabbed object
             */
            originalSize = inContact.transform.localScale;
            originalRotation = inContact.transform.localRotation;
            originalPosition = inContact.transform.position;

            collisionDetection otherCollControl = otherController.transform.GetChild(0).GetComponent<collisionDetection>();
            if (otherCollControl.inContact == inContact && otherCollControl.grabbing)
            {
                otherCollControl.StartScale();
                //Debug.Log("The other controller is grabbing and it should start scaling now");
                scaling = true;
                Vector3 thisPos = new Vector3(gameObject.transform.parent.transform.position.x, gameObject.transform.parent.transform.position.y, gameObject.transform.parent.transform.position.z);
                Vector3 otherPos = new Vector3(otherController.transform.position.x, otherController.transform.position.y, otherController.transform.position.z);
                dist = Vector3.Distance(gameObject.transform.parent.transform.position, otherController.transform.position);
                vecDist = new Vector3(Mathf.Abs(thisPos.x-otherPos.x), Mathf.Abs(thisPos.y-otherPos.y), Mathf.Abs(thisPos.z-otherPos.z));
                originalSize = inContact.transform.localScale;
                scaleObj = inContact;
            } else
            {
                inContact.transform.SetParent(gameObject.transform);
            }
        }
    }

    public void EndGrab()
    {
        //Debug.Log(gameObject.name + ": stopped grabbing");
        /*
         * Here we check if there is any change from initial state, and if so store the corresponding actions
         * First the translation, second rotation and this scaling
         */
        grabbing = false;
        foreach (GameObject joint in grabJoints)
        {
            GrabAnimator anime = joint.GetComponent<GrabAnimator>();
            anime.grab = false;
            anime.Reset();
        }
        if (inContact != null && !scaling)
        {
            inContact.transform.SetParent(null);
            if (originalPosition != inContact.transform.position)
            {
                tc.translateObject(inContact, UserBox.text, DateTime.Now, inContact.transform.position);
            }
            if (originalRotation != inContact.transform.localRotation)
            {
                tc.rotateObject(inContact, UserBox.text, DateTime.Now, inContact.transform.rotation);
            }
        } else if (inContact != null && scaling)
        {
            //tc.scaleObject(scaleObj, UserBox.text, DateTime.Now, scaleObj.transform.localScale);
            otherController.transform.GetChild(0).GetComponent<collisionDetection>().EndScale();
            //inContact.transform.SetParent(otherController.transform);
            //otherController.transform.GetChild(0).GetComponent<collissionDetection>().scaling = false;
        } else if (scaleObj != null && scaling)
        {
            otherController.transform.GetChild(0).GetComponent<collisionDetection>().EndScale();
        }
        dist = -9999;
        scaling = false;
        scaleObj = null;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.name.Contains("vr_glove_") && other.gameObject == inContact)
        {
            inContact = null;
            //Debug.Log("Trigger: " + other.gameObject.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.FindGameObjectsWithTag("PlayerName")[0];
        UserBox = go.GetComponent<Text>();
        tc = GameObject.FindGameObjectWithTag("TimelineController").GetComponent<TimelineController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scaling)
        {
            if (!otherController.transform.GetChild(0).GetComponent<collisionDetection>().grabbing)
            {
                scaling = false;
            } else
            {
                scaleObj.transform.localScale = originalSize * Vector3.Distance(gameObject.transform.parent.transform.position, otherController.transform.position) / dist;
                /*Vector3 thisPos = gameObject.transform.parent.transform.position;
                Vector3 otherPos = otherController.transform.position;
                Vector3 newVecDist = new Vector3(Mathf.Abs(thisPos.x - otherPos.x), Mathf.Abs(thisPos.y - otherPos.y), Mathf.Abs(thisPos.z - otherPos.z));
                //Debug.Log("Original dist: " + vecDist + "\nNew Dist: " + newVecDist);

                //scaleObj.transform.localScale = originalSize + new Vector3(newVecDist.x - vecDist.x, newVecDist.y - vecDist.y, newVecDist.z - vecDist.z);

                Vector3 ratio = new Vector3(newVecDist.x / vecDist.x, newVecDist.y / vecDist.y, newVecDist.z / vecDist.z);
                scaleObj.transform.localScale = Vector3.Cross(originalSize, ratio);*/
            }
            
        }
        /*if (!grabbing && )
        {

        }*/
    }
}
