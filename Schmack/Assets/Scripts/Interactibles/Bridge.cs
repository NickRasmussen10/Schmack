using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Controllable
{
    [SerializeField] float openingSpeed = 5.0f;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    RaycastHit2D raycastHit;
    Vector3 raycastStart;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (raycastHit.collider != null)
        {
            Debug.Log("collision detected");
            StopCoroutine("Activate");
        }
    }

    protected override IEnumerator Activate()
    {
        isActivated = true;
        while (isActivated)
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

            raycastStart = transform.position;
            raycastStart.x -= spriteRenderer.size.x;
            raycastHit = Physics2D.Raycast(raycastStart, Vector2.right, 0.01f, LayerMask.GetMask("environment"));
            Debug.DrawLine(raycastStart, raycastStart + (Vector3.right * 0.1f));


            yield return null;
        }
    }

    protected override IEnumerator Deactivate()
    {
        isActivated = false;
        while(spriteRenderer.size.x < -1.0f)
        {
            Vector3 temp = spriteRenderer.size;
            temp.x += Time.deltaTime * openingSpeed;
            if (temp.x > -1.0f) temp.x = -1.0f;
            spriteRenderer.size = temp;

            temp = boxCollider.offset;
            temp.x = spriteRenderer.size.x / 2;
            boxCollider.offset = temp;

            temp = boxCollider.size;
            temp.x = -spriteRenderer.size.x;
            boxCollider.size = temp;
            
            yield return null;
        }
    }
}
