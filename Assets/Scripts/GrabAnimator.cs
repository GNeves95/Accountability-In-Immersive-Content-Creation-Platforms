using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAnimator : MonoBehaviour
{
    [SerializeField]
    Vector3 targetAngle;

    [SerializeField]
    Vector3 pointAngle;

    private float animationDuration = 0.5f;

    /*[SerializeField]
    bool invert = false;*/

    private Vector3 initialAngles;

    private Vector3 delta;
    private Vector3 deltaPoint;

    public bool grab = false;
    private bool wasGrabbing = false;
    public bool point = false;
    private bool wasPointing = false;

    public float cumulative = 0f;

    public void Reset()
    {
        cumulative = 0f;
    }

    private float conv(float num)
    {
        return (num < 0) ? 360 + num : num;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialAngles = gameObject.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        delta = (targetAngle - initialAngles) / animationDuration;
        deltaPoint = (pointAngle - initialAngles) / animationDuration;
        Vector3 currAngle = gameObject.transform.localEulerAngles;
        
        if (grab)
        {
            wasGrabbing = true;
            Vector3 convTarg = new Vector3(conv(targetAngle.x), conv(targetAngle.y), conv(targetAngle.z));
            float dif = Mathf.Abs(Vector3.Distance(convTarg, currAngle));
            if (dif > 10 && cumulative < animationDuration)
            {
                cumulative += Time.deltaTime;
                //Debug.Log(dif + " curr: " + currAngle + " target: " + convTarg + " delta:" + delta);
                gameObject.transform.localEulerAngles += Time.deltaTime * targetAngle/animationDuration;
            }
        } else if (wasGrabbing && !grab)
        {
            cumulative += Time.deltaTime;
            Vector3 convTarg = new Vector3(conv(initialAngles.x), conv(initialAngles.y), conv(initialAngles.z));
            float dif = Mathf.Abs(Vector3.Distance(convTarg, currAngle));
            if (dif > 10 && cumulative < animationDuration)
            {
                //Debug.Log(dif + " curr: " + currAngle + " target: " + convTarg);
                gameObject.transform.localEulerAngles -= Time.deltaTime * targetAngle / animationDuration;
            } else
            {
                wasGrabbing = false;
            }
        }
        if (point)
        {
            wasPointing = true;
            Vector3 convTarg = new Vector3(conv(pointAngle.x), conv(pointAngle.y), conv(pointAngle.z));
            float dif = Mathf.Abs(Vector3.Distance(convTarg, currAngle));
            if (dif > 10 && cumulative < animationDuration)
            {
                cumulative += Time.deltaTime;
                //Debug.Log(dif + " curr: " + currAngle + " target: " + convTarg + " delta:" + delta);
                gameObject.transform.localEulerAngles += Time.deltaTime * pointAngle / animationDuration;
            }
        }
        else if (wasPointing && !point)
        {
            cumulative += Time.deltaTime;
            Vector3 convTarg = new Vector3(conv(initialAngles.x), conv(initialAngles.y), conv(initialAngles.z));
            float dif = Mathf.Abs(Vector3.Distance(convTarg, currAngle));
            if (dif > 10 && cumulative < animationDuration)
            {
                //Debug.Log(dif + " curr: " + currAngle + " target: " + convTarg);
                gameObject.transform.localEulerAngles -= Time.deltaTime * pointAngle / animationDuration;
            } else
            {
                wasPointing = false;
            }
        }

        /*int prefix = (invert)?-1:1;

        switch(axis)
        {
            case 'x':
                if (grab && prefix * currAngle.x < prefix * (initialAngles.x + prefix*targetAngle))
                {
                    currAngle.x += prefix * animationSpeed;
                }
                else if (!grab && prefix * currAngle.x > prefix * (initialAngles.x))
                {
                    currAngle.x -= prefix * animationSpeed;
                }
                break;
            case 'y':
                if (grab && prefix * currAngle.y < prefix * (initialAngles.y + prefix * targetAngle))
                {
                    currAngle.y += prefix * animationSpeed;
                }
                else if (!grab && prefix * currAngle.y > prefix * (initialAngles.y))
                {
                    currAngle.y -= prefix * animationSpeed;
                }

                break;
            case 'z':
                if (grab && prefix * currAngle.z < prefix * (initialAngles.z + prefix * targetAngle))
                {
                    currAngle.z += prefix * animationSpeed;
                }
                else if (!grab && prefix * currAngle.z > prefix * (initialAngles.z))
                {
                    currAngle.z -= prefix * animationSpeed;
                }

                break;
            default:
                break;
        }


        if (grab && currAngle.y < initialAngles.y + targetAngle)
        {
            currAngle.y += animationSpeed;
            gameObject.transform.localEulerAngles = currAngle;
        } else if (!grab && currAngle.y > initialAngles.y)
        {
            currAngle.y -=  animationSpeed;
            gameObject.transform.localEulerAngles = currAngle;
        }*/
    }
}
