using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controllable : MonoBehaviour
{
    Controller controller;

    public bool isActivated;
    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public void Begin()
    {
        isActivated = true;
        StopCoroutine("Deactivate");
        StartCoroutine("Activate");
    }

    public void End()
    {
        isActivated = false;
        StopCoroutine("Activate");
        StartCoroutine("Deactivate");
    }


    protected abstract IEnumerator Activate();

    protected abstract IEnumerator Deactivate();
}
