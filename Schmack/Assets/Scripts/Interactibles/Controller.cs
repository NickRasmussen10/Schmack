using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject[] GO_controllables = null;
    protected Controllable[] controllables;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        controllables = new Controllable[GO_controllables.Length];
        for(int c = 0; c < GO_controllables.Length; c++)
        {
            controllables[c] = GO_controllables[c].GetComponent<Controllable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Activate()
    {
        foreach (Controllable c in controllables)
        {
            if (c.isActivated)
            {
                c.End();
            }
            else
            {
                c.Begin();
            }
        }
    }
}
