using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : Arrow
{
    [SerializeField] float explosiveForce = 1.0f;
    [SerializeField] float radius = 10.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (hasHit)
        {
            hasHit = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody2D rb = colliders[i].GetComponent<Rigidbody2D>();
                if (rb)
                {

                    rb.AddForce((rb.gameObject.transform.position - transform.position) * explosiveForce, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-5.0f, 5.0f));

                }
            }
        }
    }
}
