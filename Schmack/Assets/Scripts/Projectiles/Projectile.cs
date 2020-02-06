using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    //[SerializeField] ParticleSystem PS_Hit = null;
    [SerializeField] TrailRenderer trail = null;

    protected Rigidbody2D rb;
    protected Collider2D collider;

    protected RaycastHit2D raycastHit;
    int layermask = 1 << 9 | 1 << 11 | 1 << 12;

    protected void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (GetComponent<BoxCollider2D>()) collider = gameObject.GetComponent<BoxCollider2D>();
        else if (GetComponent<CircleCollider2D>()) collider = GetComponent<CircleCollider2D>();
        Instantiate(trail, transform);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //layermask = LayerMask.GetMask("environment");
    }

    // Update is called once per frame
    protected void Update()
    {
        if (rb)
            raycastHit = Physics2D.Raycast(transform.position, rb.velocity.normalized, rb.velocity.sqrMagnitude * 0.001f, layermask);

        if(raycastHit.collider != null)
        {
            TriggerHit(raycastHit.collider, raycastHit.point);
        }
    }

    protected abstract void TriggerHit(Collider2D collision, Vector3 collisionPoint);

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Instantiate(PS_Hit, transform.position, Quaternion.identity);

    //    TriggerHit(collision);
    //}
}
