using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Projectile : MonoBehaviour
{
    protected const float GRAVITY = 1f;

    //physics vectors
    protected Vector2 position;
    protected Vector2 velocity;
    protected Vector2 acceleration;

    //physics varaibles
    [SerializeField] float mass;
    [SerializeField] float coefFriction;

    //array to keep track of collisions in all 4 cardinal directions
    protected RaycastHit2D[] raycastHits;



    // Start is called before the first frame update
    protected void Start()
    {
        position = transform.position;
        acceleration = Vector2.zero;
        raycastHits = new RaycastHit2D[4];
        SetRaycasts(LayerMask.GetMask("environment"));
    }

    // Update is called once per frame
    protected void Update()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity;
        SetRaycasts(LayerMask.GetMask("environment"));

        Gravity();
        AddFriction();
        SnapPositions();

        velocity = Vector2.zero;
        transform.position = position;
    }

    /// <summary>
    /// handles player falling to the ground, including slower fall for wall sticking
    /// </summary>
    void Gravity()
    {
        if (raycastHits[0].collider != null)
        {
            acceleration.y = 0;
        }
        else
        {
            acceleration.y -= GRAVITY;
        }
    }

    /// <summary>
    /// applies a force to slow horizontal motion if player is on the ground
    /// </summary>
    void AddFriction()
    {
        if (raycastHits[0].collider != null && Mathf.Abs(acceleration.x) > 0)
        {
            AddForce(-velocity.x * coefFriction, 0);
        }
    }

    protected void AddForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    protected void AddForce(float x, float y)
    {
        AddForce(new Vector2(x, y));
    }


    /// <summary>
    /// snaps the player's position to perfectly adjacent to any platform they are colliding with (avoids overlapping)
    /// </summary>
    void SnapPositions()
    {
        if (raycastHits[0].collider != null)
        {
            position.y = raycastHits[0].collider.transform.position.y +
                raycastHits[0].collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }

        if (raycastHits[1].collider != null)
        {
            position.x = raycastHits[1].collider.transform.position.x +
                raycastHits[1].collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }

        if (raycastHits[2].collider != null)
        {
            position.x = raycastHits[2].collider.transform.position.x -
                raycastHits[2].collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }

        if (raycastHits[3].collider != null)
        {
            position.y = raycastHits[3].collider.transform.position.y -
                raycastHits[3].collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
    }

    /// <summary>
    /// casts rays in all 4 cardinal directions from player for collision detection
    /// </summary>
    /// <param name="layerMask"></param>
    void SetRaycasts(LayerMask layerMask)
    {
        if (raycastHits[0].collider == null && acceleration.y == 0)
        {
            Vector3 dest = new Vector3();
            dest = transform.position + (Vector3.down * (0.5f - velocity.y * 2));
            Debug.DrawLine(position, dest, Color.magenta);
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f - velocity.y * 2, layerMask);
        }
        else
        {
            Vector3 dest = new Vector3();
            dest = transform.position + (Vector3.down * 0.5f);
            Debug.DrawLine(position, dest, Color.magenta);
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f, layerMask);
        }

        Debug.DrawLine(position, position + Vector2.up * (0.5f + velocity.y), Color.magenta);
        raycastHits[3] = Physics2D.Raycast(position, Vector2.up, 0.5f + velocity.y, layerMask);




        if (raycastHits[1].collider != null)
        {
            Vector3 dest1 = new Vector3();
            dest1 = transform.position + (Vector3.left * 0.5f);
            Debug.DrawLine(position, dest1, Color.magenta);
            raycastHits[1] = Physics2D.Raycast(position, Vector2.left, 0.5f, layerMask);
        }
        else
        {
            Vector3 dest1 = new Vector3();
            dest1 = transform.position + (Vector3.left * (0.5f + velocity.x));
            Debug.DrawLine(position, dest1, Color.magenta);
            raycastHits[1] = Physics2D.Raycast(position, Vector2.left, 0.5f + velocity.x, layerMask);
        }

        if (raycastHits[2].collider != null)
        {
            Vector3 dest2 = new Vector3();
            dest2 = transform.position + (Vector3.right * 0.5f);
            Debug.DrawLine(position, dest2, Color.magenta);
            raycastHits[2] = Physics2D.Raycast(position, Vector2.right, 0.5f, layerMask);
        }
        else
        {
            Vector3 dest2 = new Vector3();
            dest2 = transform.position + (Vector3.right * (0.5f + velocity.x));
            Debug.DrawLine(position, dest2, Color.magenta);
            raycastHits[2] = Physics2D.Raycast(position, Vector2.right, 0.5f + velocity.x, layerMask);
        }
    }
}
