using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialUISummoner : MonoBehaviour
{
    protected Animator[] children;
    public bool showing = false;
    private bool shown = false;
    public float delay = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Animator>();
        foreach (Animator child in children)
        {
            child.SetBool("Shown", showing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            if (shown) return;
            StartCoroutine("ActivateInTurn");
        } else
        {
            if (!shown) return;
            StartCoroutine("DeactivateInTurn");
        }
    }

    public IEnumerator ActivateInTurn()
    {
        yield return new WaitForSeconds(delay);
        foreach (Animator child in children)
        {
            child.SetBool("Shown", true);
            yield return new WaitForSeconds(delay);
        }
        shown = true;
    }

    public IEnumerator DeactivateInTurn()
    {
        yield return new WaitForSeconds(delay);
        foreach (Animator child in children)
        {
            child.SetBool("Shown", false);
            yield return new WaitForSeconds(delay);
        }
        shown = false;
    }
}
