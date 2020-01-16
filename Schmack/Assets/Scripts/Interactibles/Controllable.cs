using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controllable : MonoBehaviour
{
    [SerializeField] GameObject GO_Controller;
    Controller controller;

    protected bool isActivated;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = GO_Controller.GetComponent<Controller>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (controller.isActivated && !isActivated) StartCoroutine("Activate");
    }

    protected abstract IEnumerator Activate();
}
