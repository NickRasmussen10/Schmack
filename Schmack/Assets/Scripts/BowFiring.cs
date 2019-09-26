using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowFiring : MonoBehaviour
{
    PlayerMovement playerMovement;
    Vector3 direction;
    [SerializeField] float knockback;
    float powerInput;
    bool isDrawnBack = false;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxis("RightHorizontal");
        direction.y = Input.GetAxis("RightVertical");

        powerInput = Input.GetAxis("Fire1");
        if(powerInput > 0.01f)
        {
            isDrawnBack = true;
        }
        else if(powerInput < 0.01f && isDrawnBack)
        {
            isDrawnBack = false;
            playerMovement.AddBowKnockback(-direction, knockback);
        }

        Debug.DrawLine(transform.position, transform.position + (direction * 5f));
    }
}
