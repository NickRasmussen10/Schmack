using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowFiring : MonoBehaviour
{
    PlayerMovement playerMovement;  //PlayerMovement script applied to the same GameObject as this script
    Vector3 direction;  //unit vector, where the bow is pointing
    [SerializeField] float knockback;   //the amount of force applied to the player when they fire the bow
    float powerInput;   //keeps track of whether or not the bow's trigger button has been pressed

    List<GameObject> arrows = new List<GameObject>();
    [SerializeField] GameObject pref_arrow;

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
        //store the right joystick input in direction vector
        direction.x = Input.GetAxis("RightHorizontal");
        direction.y = Input.GetAxis("RightVertical");

        //store the trigger input in powerInput
        powerInput = Input.GetAxis("Fire1");

        //give the bow a small deadzone
        if(powerInput > 0.1f)
        {
            isDrawnBack = true;
        }
        //if bow has been released from previous draw
        else if(powerInput < 0.1f && isDrawnBack)
        {
            isDrawnBack = false;
            if (!playerMovement.GetIsOnGround())
            {
                playerMovement.AddBowKnockback(direction, knockback);
            }
            GameObject newArrow = Instantiate(pref_arrow, transform.position, Quaternion.identity);
            newArrow.GetComponent<Arrow>().SetStartingAcceleration(direction * knockback * 0.5f);
            arrows.Add(newArrow);
        }

        Debug.DrawLine(transform.position, transform.position + (direction * 5f));
    }
}
