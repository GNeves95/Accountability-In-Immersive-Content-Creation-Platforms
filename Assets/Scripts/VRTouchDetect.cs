using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VRTouchDetect : MonoBehaviour
{ 
    private TimelineController tc;
    // Start is called before the first frame update
    void Start()
    {
        tc = GameObject.FindGameObjectWithTag("TimelineController").GetComponent<TimelineController>();
    }

    private void OnHandHoverBegin(Hand hand)
    {
        //GrabTypes gt = hand.GetGrabStarting();
        bool pointing = hand.gameObject.GetComponent<VRCollider>().pointing;
        //Debug.Log(gt);
        if (pointing)
            tc.handleObjSelection(gameObject);

    }
}
