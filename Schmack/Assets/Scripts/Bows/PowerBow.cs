using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBow : Bow
{
    [Header("Accuracy")]
    [SerializeField] float inaccuracy = 0.01f;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        Debug.Log("powerbow update");
        HandleInput();
        if (powerInput == 0 && isDrawnBack)
        {
            direction.x += Random.Range(direction.x - inaccuracy, direction.x + inaccuracy);
            direction.y += Random.Range(direction.y - inaccuracy, direction.y + inaccuracy);
            direction.Normalize();
        }
        HandleFiring();
        PlaySounds();
    }
}
