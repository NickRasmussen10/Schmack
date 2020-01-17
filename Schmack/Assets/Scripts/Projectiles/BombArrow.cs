using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : Arrow
{
    [SerializeField] float explosiveRange = 5.0f;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (hasHit)
        {

        }
    }
}
