using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] ParticleSystem PS_Hit;

    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;

    protected void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    protected abstract void TriggerHit(Collider2D collision);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(PS_Hit, transform.position, Quaternion.identity);

        TriggerHit(collision);
    }
}
