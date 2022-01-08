using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VersionTimeline;
using SplineMesh;

public class TimelineController : MonoBehaviour
{
    public enum Scenes { Scene1, Scene2, Scene3};

    public Scenes scene = Scenes.Scene1;

    public Importer initiator;

    [SerializeField]
    bool debug = false;

    [SerializeField]
    bool transparentObjects = false;

    [SerializeField]
    bool diffView = false;

    [SerializeField]
    bool sceneView = false;

    [SerializeField]
    GameObject cube;

    [SerializeField]
    GameObject cylinder;

    [SerializeField]
    float width = 0.01f;

    [SerializeField]
    GameObject splinePrefab;

    public GameObject currentObject = null;

    public bool showTimeline = false;

    public TimelineEvent[] currentTimeline;

    public List<TimelineEvent> sceneTimeline;

    public int selectedEvent;

    public Material mat;

    GameObject historyTable = null;

    [SerializeField]
    GameObject tablePrefab;

    [SerializeField]
    GameObject evenRow;

    [SerializeField]
    GameObject oddRow;

    [SerializeField]
    GameObject overlayPrefab;

    [SerializeField]
    GameObject eventPrefab;

    [SerializeField]
    GameObject leftFramePrefab;

    [SerializeField]
    GameObject rightFramePrefab;

    [SerializeField]
    Material newGhostMat;

    [SerializeField]
    Material oldGhostMat;

    [SerializeField]
    Material sceneGhostMat;

    [SerializeField]
    GameObject hiddenObjectsContainer;

    public void handleObjSelection(GameObject go)
    {
        if ((currentObject == null/* || currentObject != go*/) && !go.tag.Equals("Event"))
        {
            if (transparentObjects)
            {
                GameObject[] sceneObjects = GameObject.FindGameObjectsWithTag("SceneObject");

                foreach (GameObject so in sceneObjects)
                {
                    if (so == go)
                        continue;
                    Color color = so.GetComponent<Renderer>().material.color;
                    //Debug.Log("changing alpha" + color);
                    color.a = 0;
                    so.GetComponent<Renderer>().material.color = color;
                    //Debug.Log("changing alpha");
                }
            }

            Transform rows = null;

            if (debug)
            {
                historyTable = Instantiate(tablePrefab);

                Transform auxHist = historyTable.transform;

                rows = auxHist.GetChild(1).GetChild(0).GetChild(0);
            }

            //Debug.Log(rows.name);

            currentTimeline = go.GetComponent<VersionControl>().events.ToArray();

            currentObject = go;

            showTimeline = true;
            bool present = false;
            bool scale = false;
            bool rotate = false;
            Vector3 last = new Vector3(0, 0, 0);
            int i = 0;

            GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Event");

            VersionControl currVerCont = null;

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            toDestroy = GameObject.FindGameObjectsWithTag("Overlay");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            toDestroy = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            if (sceneView || transparentObjects)
            {
                GameObject[] sceneObjects = GameObject.FindGameObjectsWithTag("SceneObject");
                foreach (GameObject sceneObject in sceneObjects)
                {
                    if (sceneObject != currentObject)
                    {
                        sceneObject.transform.parent = hiddenObjectsContainer.transform;
                    }
                }
            }

            if (debug)
                historyTable.tag = "Event";

            //GameObject[] cubes;

            List<GameObject> cubes = new List<GameObject>();

            GameObject co = null;

            SplineNode PreviousNode = null;
            SplineNode CurrentNode = null;
            int iterSpline = 0;

            Spline spline = null;

            foreach (TimelineEvent eve in currentTimeline)
            {
                Snapshot snapshot = new Snapshot(go, i);
                eve.snapshot = snapshot;
                if (eve.type.Equals("import"))
                {
                    Import ev = (Import)eve;
                    Vector3 Point = ev.coordinates;
                    float dist = Mathf.Abs(Vector3.Distance(last, Point));
                    bool foundCube = false;
                    cubes.ForEach(delegate (GameObject cube) { if (Mathf.Abs(Vector3.Distance(Point, cube.transform.position)) < 4*width) { foundCube = true; co = cube;} });
                    if (!foundCube)
                    {
                        co = Instantiate(cube, Point, Quaternion.identity);
                        co.name = "event-cube~_~" + i;
                        currVerCont = co.AddComponent<VersionControl>();
                        co.transform.localScale = new Vector3(2 * width, 2 * width, 2 * width);
                        co.tag = "Event";
                        co.GetComponent<Renderer>().material = mat;
                        cubes.Add(co);
                        /*cube.GetComponent<SceneObject>().Id = "ev_" + this.ID + "_-~" + i;
                        cube.GetComponent<SceneObject>().parent = this.gameObject;
                        cube.GetComponent<SceneObject>().Eve = eve;*/

                        //TODO: use spline instead of cylinder
                        /*PreviousNode = CurrentNode;

                        CurrentNode = new SplineNode(co.transform.position, Vector3.zero);

                        if (iterSpline > 0)
                        {
                            Vector3 dir = (CurrentNode.Position - PreviousNode.Position)/10;
                            PreviousNode.Direction = dir;
                            CurrentNode.Direction = dir;
                            if (iterSpline == 1)
                            {
                                GameObject sp = Instantiate(splinePrefab);
                                spline = sp.GetComponent<Spline>();
                                spline.AddNode(PreviousNode);
                                spline.RemoveNode(spline.nodes[0]);
                            }
                            spline.AddNode(CurrentNode);
                            if (iterSpline == 1)
                                spline.RemoveNode(spline.nodes[0]);
                        }

                        iterSpline++;*/

                        if (present)
                        {
                            GameObject line = Instantiate(cylinder, (Point + last) / 2f, Quaternion.identity);
                            Vector3 dirV = Vector3.Normalize(Vector3.Normalize(Point - last) + new Vector3(0, 1, 0));
                            line.transform.rotation = new Quaternion(dirV.x, dirV.y, dirV.z, 0);
                            line.transform.localScale = new Vector3(width, Vector3.Distance(last, Point) / 2, width);
                            line.tag = "Event";
                            line.GetComponent<Renderer>().material = mat;
                        }
                        last = Point;
                        currVerCont.scale = false;
                        currVerCont.rotate = false;

                        currVerCont.id = "cube_event~_~" + i;
                    } else
                    {
                        currVerCont = co.GetComponent<VersionControl>();
                        /*PreviousNode = CurrentNode;

                        CurrentNode = new SplineNode(co.transform.position, Vector3.zero);

                        if (iterSpline > 0)
                        {
                            Vector3 dir = (CurrentNode.Position - PreviousNode.Position) / 10;
                            PreviousNode.Direction = dir;
                            CurrentNode.Direction = dir;
                            if (iterSpline == 1)
                            {
                                GameObject sp = Instantiate(splinePrefab);
                                spline = sp.GetComponent<Spline>();
                                spline.AddNode(PreviousNode);
                                spline.RemoveNode(spline.nodes[0]);
                            }
                            spline.AddNode(CurrentNode);
                            if (iterSpline == 1)
                                spline.RemoveNode(spline.nodes[0]);
                        }

                        iterSpline++;*/
                    }
                    currVerCont.events.Add(ev);

                    present = true;
                } else if (eve.type.Equals("translation"))
                {
                    Translation ev = (Translation)eve;
                    Vector3 Point = ev.coordinates;
                    float dist = Mathf.Abs(Vector3.Distance(last, Point));
                    bool foundCube = false;
                    cubes.ForEach(delegate (GameObject cube) { if (Mathf.Abs(Vector3.Distance(Point, cube.transform.position)) < 4*width) { foundCube = true; co = cube; } });
                    if (!foundCube)
                    {
                        co = Instantiate(cube, Point, Quaternion.identity);
                        co.name = "event-cube~_~" + i;
                        currVerCont = co.AddComponent<VersionControl>();
                        currVerCont.id = "cube_event~_~" + i;
                        //currVerCont.events.Add(ev);
                        co.transform.localScale = new Vector3(2 * width, 2 * width, 2 * width);
                        co.tag = "Event";
                        co.GetComponent<Renderer>().material = mat;
                        cubes.Add(co);
                        /*cube.GetComponent<SceneObject>().Id = "ev_" + this.ID + "_-~" + i;
                        cube.GetComponent<SceneObject>().parent = this.gameObject;
                        cube.GetComponent<SceneObject>().Eve = eve;*/
                        /*PreviousNode = CurrentNode;

                        CurrentNode = new SplineNode(co.transform.position, Vector3.zero);

                        if (iterSpline > 0)
                        {
                            Vector3 dir = (CurrentNode.Position - PreviousNode.Position)/10;
                            PreviousNode.Direction = dir;
                            CurrentNode.Direction = dir;
                            if (iterSpline == 1)
                            {
                                GameObject sp = Instantiate(splinePrefab);
                                spline = sp.GetComponent<Spline>();
                                spline.AddNode(PreviousNode);
                                spline.RemoveNode(spline.nodes[0]);
                            }
                            spline.AddNode(CurrentNode);
                            if (iterSpline == 1)
                                spline.RemoveNode(spline.nodes[0]);
                        }

                        iterSpline++;*/

                        if (present)
                        {
                            GameObject line = Instantiate(cylinder, (Point + last) / 2f, Quaternion.identity);
                            Vector3 dirV = Vector3.Normalize(Vector3.Normalize(Point - last) + new Vector3(0, 1, 0));
                            line.transform.rotation = new Quaternion(dirV.x, dirV.y, dirV.z, 0);
                            line.transform.localScale = new Vector3(width, Vector3.Distance(last, Point) / 2, width);
                            line.tag = "Event";
                            line.GetComponent<Renderer>().material = mat;
                        }
                        last = Point;
                        currVerCont.scale = false;
                        currVerCont.rotate = false;

                        currVerCont.id = "cube_event~_~" + i;
                    } else
                    {
                        currVerCont = co.GetComponent<VersionControl>();
                        PreviousNode = CurrentNode;

                        /*CurrentNode = new SplineNode(co.transform.position, Vector3.zero);

                        if (iterSpline > 0)
                        {
                            Vector3 dir = (CurrentNode.Position - PreviousNode.Position) / 10;
                            PreviousNode.Direction = dir;
                            CurrentNode.Direction = dir;
                            if (iterSpline == 1)
                            {
                                GameObject sp = Instantiate(splinePrefab);
                                spline = sp.GetComponent<Spline>();
                                spline.AddNode(PreviousNode);
                                spline.RemoveNode(spline.nodes[0]);
                            }
                            spline.AddNode(CurrentNode);
                            if (iterSpline == 1)
                                spline.RemoveNode(spline.nodes[0]);
                        }

                        iterSpline++;*/
                    }
                    currVerCont.events.Add(ev);
                    present = true;
                } else if (eve.type.Equals("scale"))
                {
                    if (currVerCont != null)
                    {
                        Scale ev = (Scale)eve;
                        currVerCont.events.Add(ev);
                        if (!scale && co != null)
                        {
                            scale = true;
                            co.transform.localScale *= 2f;
                        }
                    }
                }
                else if (eve.type.Equals("rotation"))
                {
                    if (currVerCont != null)
                    {
                        Rotation ev = (Rotation)eve;
                        currVerCont.events.Add(ev);
                        if (!rotate && co != null)
                        {
                            rotate = true;
                            co.transform.localRotation = Quaternion.Euler(45,0,45);
                        }
                    }
                }
                if (debug)
                {
                    GameObject newRow;
                    
                    if (i%2==0)
                    {
                        newRow = Instantiate(evenRow);
                    } else
                    {
                        newRow = Instantiate(oddRow);
                    }

                    Transform rowTran = newRow.transform;

                    rowTran.SetParent(rows);

                    rowTran.GetChild(0).GetComponent<Text>().text = eve.type;
                    rowTran.GetChild(1).GetComponent<Text>().text = eve.user;
                    rowTran.GetChild(2).GetComponent<Text>().text = DateTime.FromFileTimeUtc(eve.when).ToString();
                }
                i++;
            }
        } else if (currentObject == go && !go.tag.Equals("Event"))
        {
            showTimeline = false;
            currentTimeline = null;
            currentObject = null;
            GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Event");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }
            Destroy(historyTable);
            historyTable = null;

            toDestroy = GameObject.FindGameObjectsWithTag("Overlay");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            toDestroy = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            if (sceneView || transparentObjects)
            {
                while (hiddenObjectsContainer.transform.childCount > 0)
                {
                    
                    hiddenObjectsContainer.transform.GetChild(0).parent = null;
                }
            }
        } else if (go.tag.Equals("Event"))
        {
            GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Overlay");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            toDestroy = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }

            int eventNum = Int32.Parse(go.name.Split(new string[] { "~_~" }, StringSplitOptions.None)[1]);
            Debug.Log("Event number: " + eventNum);

            Vector3 position = go.transform.position;

            VersionControl vc = go.GetComponent<VersionControl>();

            List<TimelineEvent> events = vc.events;

            position.y += 0.5f;

            GameObject panel = Instantiate(overlayPrefab, position, Quaternion.identity);
            Transform content = panel.transform.GetChild(0);

            GameObject lefttFrame = Instantiate(leftFramePrefab);
            lefttFrame.transform.SetParent(content);
            RectTransform lftran = lefttFrame.GetComponent<RectTransform>();

            lftran.localPosition = Vector3.zero;
            lftran.localScale = new Vector3(1, 15, 10);

            events.ForEach(delegate (TimelineEvent ev) {
                GameObject eve = Instantiate(eventPrefab);
                Snapshot eveSnap = ev.snapshot;
                Snapshot snap = eve.AddComponent<Snapshot>() as Snapshot;
                snap.go = eveSnap.go;
                snap.eventNum = eveSnap.eventNum;

                //eve.AddComponent
                Transform eveTran = eve.transform;
                //eve.transform.localScale = Vector3.one;
                //eve.transform.parent = content;
                eveTran.SetParent(content);
                eveTran.localScale = Vector3.one;
                //eveTran.localPosition.z = 0;
                Vector3 newPos = eveTran.localPosition;
                newPos.z = 0;
                eveTran.localPosition = newPos;
                Transform quadTran = eve.transform.GetChild(0);
                string eventType = ev.type;
                if (eventType.Equals("rotation"))
                {
                    quadTran.localEulerAngles = new Vector3(0, 0, 45);
                } else if (eventType.Equals("scale"))
                {
                    quadTran.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                }
                Transform textTran = eve.transform.GetChild(1);
                textTran.GetComponent<Text>().text = ev.user +"\n" + ev.type + "\n" + DateTime.FromFileTimeUtc(ev.when);
            });

            GameObject rightFrame = Instantiate(rightFramePrefab);
            rightFrame.transform.SetParent(content);
            RectTransform rftran = rightFrame.GetComponent<RectTransform>();

            rftran.localPosition = Vector3.zero;
            rftran.localScale = new Vector3(1, 15, 10);
        }
        else if (go.tag.Equals("Snapshot"))
        {
            GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject td in toDestroy)
            {
                Destroy(td);
            }
            Snapshot snap = go.GetComponent<Snapshot>();

            int eNum = snap.eventNum;
            TimelineEvent[] timelineEvents = snap.go.GetComponent<VersionControl>().events.ToArray();

            int currEvent = 0;

            GameObject newInstance = null;

            GameObject oldInstance = null;

            long date = timelineEvents[eNum].when;

            foreach (TimelineEvent ev in timelineEvents) 
            {
                if (currEvent == 0)
                {
                    Import impEv = (Import)ev;
                    foreach (GameObject gObj in initiator.Prefabs)
                    {
                        if (gObj.name.Equals(snap.go.name))
                        {
                            newInstance = Instantiate(gObj, impEv.coordinates, Quaternion.identity);
                            newInstance.tag = "Ghost";
                            newInstance.GetComponent<Renderer>().material = newGhostMat;
                            if (diffView && eNum > 0)
                            {
                                oldInstance = Instantiate(gObj, impEv.coordinates, Quaternion.identity);
                                oldInstance.tag = "Ghost";
                                oldInstance.GetComponent<Renderer>().material = oldGhostMat;
                            }
                            break;
                        }
                    }
                } else
                {
                    if (ev.type.Equals("translation"))
                    {
                        newInstance.transform.position = ((Translation)ev).coordinates;
                        if (diffView)
                        {
                            if (currEvent < eNum)
                                oldInstance.transform.position = ((Translation)ev).coordinates;
                        }
                    } else if (ev.type.Equals("scale"))
                    {
                        newInstance.transform.localScale = ((Scale)ev).scale;
                        if (diffView)
                        {
                            if (currEvent < eNum)
                                oldInstance.transform.localScale = ((Scale)ev).scale;
                        }
                    }
                    else if (ev.type.Equals("rotation"))
                    {
                        newInstance.transform.rotation = ((Rotation)ev).rotation;
                        if (diffView)
                        {
                            if (currEvent < eNum)
                                oldInstance.transform.rotation = ((Rotation)ev).rotation;
                        }
                    }
                }
                if (currEvent == eNum)
                {
                    break;
                }
                currEvent++;
            }
            if (sceneView)
            {
                Transform hocTran = hiddenObjectsContainer.transform;
                for (int iter = 0; iter < hocTran.childCount; iter++)
                {
                    Transform childTran = hocTran.GetChild(iter);
                    GameObject childGobj = childTran.gameObject;
                    string prefabName = childGobj.name;
                    VersionControl vc = childGobj.GetComponent<VersionControl>();
                    TimelineEvent[] events = vc.events.ToArray();

                    bool imported = false;
                    GameObject sceneInstance = null;

                    foreach (TimelineEvent ev in events)
                    {
                        if (ev.when > date)
                        {
                            break;
                        }
                        if (!imported)
                        {
                            Import impEv = (Import)ev;
                            foreach (GameObject gObj in initiator.Prefabs)
                            {
                                if (gObj.name.Equals(snap.go.name))
                                {
                                    sceneInstance = Instantiate(gObj, impEv.coordinates, Quaternion.identity);
                                    sceneInstance.tag = "Ghost";
                                    sceneInstance.GetComponent<Renderer>().material = sceneGhostMat;
                                    break;
                                }
                            }
                            imported = true;
                        }
                        else
                        {
                            if (ev.type.Equals("translation"))
                            {
                                sceneInstance.transform.position = ((Translation)ev).coordinates;
                            }
                            else if (ev.type.Equals("scale"))
                            {
                                sceneInstance.transform.localScale = ((Scale)ev).scale;
                            }
                            else if (ev.type.Equals("rotation"))
                            {
                                sceneInstance.transform.rotation = ((Rotation)ev).rotation;
                            }
                        }
                    }
                }
            }
        }
    }

    public void importObject(GameObject go, string user, DateTime time)
    {
        //TODO: add an id, this is crucial
        Import newObj = new Import(time, user);
        newObj.coordinates = go.transform.position;
        newObj.rotation = go.transform.rotation;
        string result = JsonUtility.ToJson(newObj);
        //go.tag = "SceneObject";
        //Debug.Log(go.name);
        string id = go.name + "~_~" + time.ToFileTimeUtc();
        string name = "objects\\" + id + ".json";
        FileInfo fi = new FileInfo(name);
        if (fi.Exists)
        {
            Debug.Log(name + " already exists.");
            int i = -1;
            do
            {
                i++;
                fi = new FileInfo("objects\\" + id + i + ".json");
            } while (fi.Exists);
            id += i;
            name = "objects\\" + id + ".json";
        }
        StreamWriter sw = File.AppendText(name);

        sw.Write(result);

        sw.Close();

        VersionControl vc = go.AddComponent<VersionControl>();
        vc.id = id;
        vc.events = new List<TimelineEvent>();
        vc.events.Add(newObj);
        /*TimelineEvent aux = JsonUtility.FromJson<TimelineEvent>(result);
        Debug.Log(aux.type);*/
    }

    public void scaleObject(GameObject go, string user, DateTime time, Vector3 scale)
    {
        Scale newScale = new Scale(time, user);
        newScale.scale = scale;
        string result = JsonUtility.ToJson(newScale);
        //Debug.Log(result);
        VersionControl vc = go.GetComponent<VersionControl>();
        StreamWriter sw = File.AppendText("objects\\"+vc.id+".json");
        sw.Write(";"+result);
        sw.Close();
        vc.events.Add(newScale);
    }

    public void rotateObject(GameObject go, string user, DateTime time, Quaternion rotation)
    {
        Rotation newRot = new Rotation(time, user);
        newRot.rotation = rotation;
        string result = JsonUtility.ToJson(newRot);
        //Debug.Log(result);
        VersionControl vc = go.GetComponent<VersionControl>();
        StreamWriter sw = File.AppendText("objects\\" + vc.id + ".json");
        sw.Write(";" + result);
        sw.Close();
        vc.events.Add(newRot);
    }

    public void translateObject(GameObject go, string user, DateTime time, Vector3 position)
    {
        Translation newTran = new Translation(time, user);
        newTran.coordinates = position;
        string result = JsonUtility.ToJson(newTran);
        //Debug.Log(result);
        VersionControl vc = go.GetComponent<VersionControl>();
        StreamWriter sw = File.AppendText("objects\\" + vc.id + ".json");
        sw.Write(";" + result);
        sw.Close();
        vc.events.Add(newTran);
    }

    public void deleteObject(GameObject go, string user, DateTime time)
    {
        //TODO: not implemented yet
    }

    // Start is called before the first frame update
    void Start()
    {
        initiator = GameObject.FindGameObjectWithTag("Initiator").GetComponent<Importer>();
        sceneTimeline = new List<TimelineEvent>();
        if (Directory.Exists("objects\\"))
        {
            //Debug.Log("Folder exists");
            DirectoryInfo di = Directory.CreateDirectory("objects\\" + scene);
            //Debug.Log(di.FullName);
            string[] files = Directory.GetFiles("objects\\" + scene);
            foreach(string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.Equals(".json")) {
                    string id = Path.GetFileNameWithoutExtension(fi.Name);
                    string prefab = id.Split(new string[] { "~_~" }, StringSplitOptions.None)[0];
                    GameObject newInstance = null;
                    VersionControl vc = null;
                    foreach (GameObject go in initiator.Prefabs)
                    {
                        if (go.name.Equals(prefab))
                        {
                            //Transform tran = ParentController.transform;
                            newInstance = Instantiate(go, Vector3.one,Quaternion.identity);
                            newInstance.tag = "SceneObject";
                            newInstance.name = prefab;

                            vc = newInstance.AddComponent<VersionControl>();
                            vc.id = id;
                            vc.events = new List<TimelineEvent>();
                            break;
                        }
                    }
                    if (newInstance == null || vc == null)
                        continue;
                    string contents = File.ReadAllText(fi.FullName);
                    string[] eventStrings = contents.Split(';');
                    foreach (string es in eventStrings)
                    {
                        TimelineEvent ev = JsonUtility.FromJson<TimelineEvent>(es);
                        int i = 0;
                        if (ev.type.Equals("import"))
                        {
                            Import importEvent = JsonUtility.FromJson<Import>(es);
                            newInstance.transform.position = importEvent.coordinates;
                            newInstance.transform.localRotation = importEvent.rotation;
                            importEvent.parent = newInstance;
                            vc.events.Add(importEvent);

                            sceneTimeline.ForEach(delegate (TimelineEvent scene_event)
                            {
                                if (scene_event.when > importEvent.when)
                                {
                                    return;
                                }
                                i++;
                            });

                            sceneTimeline.Insert(i, importEvent);
                        } else if (ev.type.Equals("translation"))
                        {
                            Translation translationEvent = JsonUtility.FromJson<Translation>(es);
                            newInstance.transform.position = translationEvent.coordinates;
                            translationEvent.parent = newInstance;
                            vc.events.Add(translationEvent);

                            sceneTimeline.ForEach(delegate (TimelineEvent scene_event)
                            {
                                if (scene_event.when > translationEvent.when)
                                {
                                    return;
                                }
                                i++;
                            });

                            sceneTimeline.Insert(i, translationEvent);
                        } else if (ev.type.Equals("scale"))
                        {
                            Scale scaleEvent = JsonUtility.FromJson<Scale>(es);
                            newInstance.transform.localScale = scaleEvent.scale;
                            scaleEvent.parent = newInstance;
                            vc.events.Add(scaleEvent);

                            sceneTimeline.ForEach(delegate (TimelineEvent scene_event)
                            {
                                if (scene_event.when > scaleEvent.when)
                                {
                                    return;
                                }
                                i++;
                            });

                            sceneTimeline.Insert(i, scaleEvent);
                        } else if (ev.type.Equals("rotation"))
                        {
                            Rotation rotationEvent = JsonUtility.FromJson<Rotation>(es);
                            newInstance.transform.localRotation = rotationEvent.rotation;
                            rotationEvent.parent = newInstance;
                            vc.events.Add(rotationEvent);

                            sceneTimeline.ForEach(delegate (TimelineEvent scene_event)
                            {
                                if (scene_event.when > rotationEvent.when)
                                {
                                    return;
                                }
                                i++;
                            });

                            sceneTimeline.Insert(i, rotationEvent);
                        }
                    }
                    //Debug.Log(contents);
                }
            }
        } else
        {
            //Debug.Log("Create Directory");
            Directory.CreateDirectory("objects\\");
            //Debug.Log("Done");
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
