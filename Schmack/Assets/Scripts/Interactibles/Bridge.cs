using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Controllable
{
    [SerializeField] float openingSpeed = 5.0f;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    EdgeCollider2D edgeCollider;
    RaycastHit2D raycast;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Activate()
    {
        isActivated = true;
        while(isActivated)
        {
            Vector3 temp = spriteRenderer.size;
            temp.x -= Time.deltaTime * openingSpeed;
            spriteRenderer.size = temp;

            temp = boxCollider.offset;
            temp.x = spriteRenderer.size.x / 2;
            boxCollider.offset = temp;

            temp = boxCollider.size;
            temp.x = -spriteRenderer.size.x;
            boxCollider.size = temp;

            //this raycast isn't detecting collisions what the frick?
            raycast = Physics2D.Raycast(new Vector3(transform.position.x - spriteRenderer.size.x, transform.position.y, transform.position.z), Vector2.right, 0.05f);
            Debug.DrawLine(new Vector3(transform.position.x - spriteRenderer.size.x, transform.position.y, transform.position.z), new Vector3(transform.position.x - spriteRenderer.size.x, transform.position.y, transform.position.z) + new Vector3(0.05f, 0.0f, 0.0f));
            if (raycast.collider != null)
            {
                isActivated = false;
            }

            //edgeCollider.points.SetValue(new Vector2(spriteRenderer.size.x, 0.5f), 0);
            //edgeCollider.points.SetValue(new Vector2(spriteRenderer.size.x, -0.5f), 1);
            //temp = edgeCollider.points[0];
            //Debug.Log(temp);
            //temp.x = spriteRenderer.size.x;
            //edgeCollider.points[0] = temp;

            //temp = edgeCollider.points[1];
            //temp.x = spriteRenderer.size.x;
            //edgeCollider.points[1] = temp;

            yield return null;
        }
    }

    protected override IEnumerator Deactivate()
    {
        isActivated = false;
        yield return null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("E D G Y");
        isActivated = false;
    }
}
