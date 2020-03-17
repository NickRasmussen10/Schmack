using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIndicator : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.size = new Vector2(Mathf.Lerp(0.0f, 2.5f, Inputs.controls.Player.Draw.ReadValue<float>()), 0.75f);
    }
}
