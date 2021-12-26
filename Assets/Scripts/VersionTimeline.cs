using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VersionTimeline
{
    [Serializable]
    public class TimelineEvent
    {
        public GameObject parent;

        public Snapshot snapshot;

        [SerializeField]
        public string type;
        [SerializeField]
        public long when;
        [SerializeField]
        public string user;
        public TimelineEvent()
        {

        }

        public DateTime getTime()
        {
            return DateTime.FromFileTimeUtc(when);
        }
    }

    [Serializable]
    public class Import : TimelineEvent
    {
        [SerializeField]
        public Vector3 coordinates;
        [SerializeField]
        public Quaternion rotation;

        public Import() { this.type = "import"; }

        public Import(string user) : base()
        {
            this.type = "import";
            this.when = DateTime.Now.ToFileTimeUtc();
            this.user = user;
        }

        public Import(DateTime date, string user) : base()
        {
            this.type = "import";
            this.when = date.ToFileTimeUtc();
            this.user = user;
        }
    }

    [Serializable]
    public class Translation : TimelineEvent
    {
        [SerializeField]
        public Vector3 coordinates;
        public Translation() { this.type = "translation"; }
        public Translation(string user) : base()
        {
            this.type = "translation";
            this.when = DateTime.Now.ToFileTimeUtc();
            this.user = user;
        }

        public Translation(DateTime date, string user) : base()
        {
            this.type = "translation";
            this.when = date.ToFileTimeUtc();
            this.user = user;
        }
    }

    [Serializable]
    public class Scale : TimelineEvent
    {
        [SerializeField]
        public Vector3 scale;
        public Scale() { this.type = "scale"; }
        public Scale(string user) : base()
        {
            this.type = "scale";
            this.when = DateTime.Now.ToFileTimeUtc();
            this.user = user;
        }

        public Scale(DateTime date, string user) : base()
        {
            this.type = "scale";
            this.when = date.ToFileTimeUtc();
            this.user = user;
        }
    }

    [Serializable]
    public class Rotation : TimelineEvent
    {
        [SerializeField]
        public Quaternion rotation;
        public Rotation() { this.type = "rotation"; }
        public Rotation(string user) : base()
        {
            //super();
            this.type = "rotation";
            this.when = DateTime.Now.ToFileTimeUtc();
            this.user = user;
        }

        public Rotation(DateTime date, string user) : base()
        {
            //super();
            this.type = "rotation";
            this.when = date.ToFileTimeUtc();
            this.user = user;
        }
    }
}
