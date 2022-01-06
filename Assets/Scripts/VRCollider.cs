using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCollider : MonoBehaviour
{

    public bool pointing = false;

    public GameObject inContact;

    public TimelineController tc;

    public SerialUISummoner ImportPanel;

    public Text UserBox;

    public GameObject[] Prefabs;

    private void OnCollisionEnter(Collision other)
    {
        inContact = other.gameObject;
        if (pointing)
        {
            if (other.gameObject.name.Equals("Panel") && ImportPanel.showing)
            {
                string importName = other.transform.GetChild(0).GetComponent<Text>().text;
                /*Debug.Log(importName);
                if (importName.Equals("Cube"))
                {
                    //Instantiate()
                }*/
                foreach (GameObject go in Prefabs)
                {
                    if (go.name.Equals(importName))
                    {
                        Transform tran = transform;
                        GameObject newInstance = Instantiate(go, tran.position + Camera.main.transform.forward, tran.rotation);
                        newInstance.tag = "SceneObject";
                        newInstance.name = importName;
                        //Debug.Log(UserBox.text + " imported " + importName);
                        //import trigger
                        tc.importObject(newInstance, UserBox.text, DateTime.Now);
                        break;
                    }
                }
                ImportPanel.GetComponent<SerialUISummoner>().showing = false;
            }
            else
            {
                VersionControl vc = other.gameObject.GetComponent<VersionControl>();
                if (vc != null || other.gameObject.tag.Equals("Snapshot"))
                {
                    tc.handleObjSelection(other.gameObject);
                }
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject != inContact)
            inContact = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject ip = GameObject.FindGameObjectWithTag("ImportPanel");
        if (ip)
            ImportPanel = ip.GetComponent<SerialUISummoner>();
        GameObject go = GameObject.FindGameObjectWithTag("PlayerName");
        if (go)
            UserBox = go.GetComponent<Text>();
        tc = GameObject.FindGameObjectWithTag("TimelineController").GetComponent<TimelineController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
