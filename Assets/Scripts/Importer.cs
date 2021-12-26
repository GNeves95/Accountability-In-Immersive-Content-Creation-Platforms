using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Importer : MonoBehaviour
{
    [SerializeField]
    GameObject ParentController;

    [SerializeField]
    GameObject ImportPanel;

    [SerializeField]
    public GameObject[] Prefabs;

    private Text UserBox;

    private TimelineController tc;

    private collisionDetection parentColider;

    private SerialUISummoner importPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (parentColider != null)
        {
            if (parentColider.pointing)
            {
                if (other.gameObject.name.Equals("Panel") && importPanel.showing)
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
                            Transform tran = ParentController.transform;
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
                    if (vc != null || other.tag.Equals("Snapshot"))
                    {
                        tc.handleObjSelection(other.gameObject);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject ip = GameObject.FindGameObjectWithTag("ImportPanel");
        if (ip)
            importPanel = ip.GetComponent<SerialUISummoner>();
        GameObject go = GameObject.FindGameObjectWithTag("PlayerName");
        if (go)
            UserBox = go.GetComponent<Text>();
        tc = GameObject.FindGameObjectWithTag("TimelineController").GetComponent<TimelineController>();
        if (ParentController != null)
            parentColider = ParentController.GetComponent<collisionDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (importPanel == null)
        {
            GameObject ip = GameObject.FindGameObjectWithTag("ImportPanel");
            if (ip)
                importPanel = ip.GetComponent<SerialUISummoner>();
        }
    }
}
