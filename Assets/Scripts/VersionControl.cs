using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VersionTimeline;

public class VersionControl : MonoBehaviour
{
    public string id;

    public List<TimelineEvent> events;

    public bool scale = false;

    public bool rotate = false;

    VersionControl()
    {
        events = new List<TimelineEvent>();
    }

    VersionControl(string Id)
    {
        events = new List<TimelineEvent>();
        this.id = Id;
    }
    // Start is called before the first frame update
    void Start()
    {
        //events = new List<TimelineEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
