using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamagePacket
{
    public float damage;
    public bool isPowerShot;
    public float powerShotRadius;
}


public class Arrow : Projectile
{
    [SerializeField] TrailRenderer trail = null;
    [SerializeField] float powerShotEffectRadius = 1.0f;

    bool isPowerShot = false;
    public void SetPowerShot(bool value) { isPowerShot = value; }

    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();

        layermask = 1 << 9 | 1 << 11 | 1 << 12 | 1 << 15 | 1 << 16;

        //start facing in direction of velocity
        transform.right = -rb.velocity;
        Instantiate(trail, transform);
    }

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();

        //if rigid body is not null
        if (rb)
        {
            //update arrow to face the same direction as current velocity
            transform.right = -rb.velocity;
        }
    }

    private void OnBecameInvisible()
    {
        if(gameObject.activeSelf) StartCoroutine(DeleteDelay(5.0f));
    }

    private void OnBecameVisible()
    {
        //really don't like not specifically stopping DeleteDelay(), this feels like it could cause some issues down the road
        StopAllCoroutines();
    }

    IEnumerator DeleteDelay(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }


    /// <summary>
    /// Handles logic for when the projectile detects collision with a valid object
    /// </summary>
    /// <param name="collision">the collider of the object that collision has been detected with</param>
    /// <param name="collisionPoint">the exact point at which the projectile hit the collider</param>
    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {
        Debug.Log("the heck");
        //if arrow hits an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            //parent arrow to enemy and apply damage to enemy
            //transform.parent = collision.gameObject.GetComponentsInChildren<Transform>()[1]; //have to parent arrow to root object so it moves with animations, DISGUSTIN'
            transform.parent = collision.gameObject.transform;
            DamagePacket packet;
            packet.damage = 0.5f;
            packet.isPowerShot = isPowerShot;
            packet.powerShotRadius = powerShotEffectRadius;
            collision.gameObject.GetComponent<Enemy>().SendMessage("TakeDamage", packet);
            FindObjectOfType<PlayerMovement>().AddFlow(0.15f);
            Debug.Log("the heck");
        }

        //if arrow hits an interactable
        else if (collision.gameObject.tag == "Interactable")
        {
            //parent arrow to interactable
            transform.parent = collision.gameObject.transform;
        }

        else if(collision.gameObject.tag == "Button")
        {
            Debug.Log("you idiot! you fool! you bumbling bafoon! you've disabled buttons for the target mini-game playtesting thing and forgotten! at last your hubris has become your undoing!");
            //collision.gameObject.GetComponent<ShootButton>().Activate();
        }

        Vector3 randomization = (Vector2)rb.velocity.normalized * Random.Range(-0.25f, 0.25f);

        //destroy arrow's rigid body and arrow component
        Destroy(rb);
        Destroy(this);

        //snap arrow to point of collision
        transform.position = collisionPoint + randomization;

        GameObject.FindObjectOfType<SoundManager>().Play("ArrowHit");
        GetComponent<Animator>().Play("wiggle");



        ///Horrible, terrible, no good, very bad code that makes the targets mini-game work please god please delete this later
        //using the button tag because it's inactive in the rest of the game and I'm a toddler who refuses to make a new tag for this
        if (collision.gameObject.tag == "Button")
        {
            Debug.Log("the heck");
            FindObjectOfType<TargetManager>().Delete(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
