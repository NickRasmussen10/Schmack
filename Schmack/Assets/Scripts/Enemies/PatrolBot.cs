using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBot : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float attackRange;
    [SerializeField] Transform head;
    [SerializeField] UnityEngine.Experimental.Rendering.LWRP.Light2D light;
    [SerializeField] GameObject pref_Projectile;

    Transform player;

    Vector2 direction = Vector2.left;
    float movementLimitor = 1.0f;

    enum State
    {
        patrol,
        attack,
        dead
    }
    State state;

    Coroutine activeCoroutine;
    CollisionPacket coll_ground;
    CollisionPacket coll_wall;

    Animator anim;

    float health = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        state = State.patrol;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.patrol:
                Move();
                if (GetSeesPlayer())
                {
                    state = State.attack;
                    StartCoroutine(HandleAttack());
                }
                break;
            case State.attack:
                if (!GetSeesPlayer())
                {
                    state = State.patrol;
                }
                break;
            case State.dead:
                break;
            default:
                break;
        }
        UpdateAnimationValues();

        //if ((head.eulerAngles.z > 90 && direction.x != -1) ||
        //   (head.eulerAngles.z < 90 && direction.x != 1))
        //{
        //    Debug.Log("whoop! other way");
        //    FlipDirection();
        //}
    }

    void TakeDamage(float damage) { health -= damage; }


    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime * movementLimitor);
    }

    IEnumerator Turn()
    {
        //anim.SetBool("turn", false);
        //float originZ = head.eulerAngles.z;
        //float lerpVal = 0.0f;

        ////left to right
        //if (direction.x == -1)
        //{
        //    while (lerpVal < 1.0f)
        //    {
        //        lerpVal += Time.deltaTime;
        //        if (lerpVal > 1.0f) lerpVal = 1.0f;

        //        head.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(originZ, 0.0f, lerpVal));
        //        yield return null;
        //    }
        //}

        ////right to left
        //else
        //{
        //    while (lerpVal < 1.0f)
        //    {
        //        lerpVal += Time.deltaTime;
        //        if (lerpVal > 1.0f) lerpVal = 1.0f;

        //        head.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(originZ, 180.0f, lerpVal));
        //        yield return null;
        //    }
        //}

        //FlipDirection();
        yield return null;
    }

    IEnumerator HandleAttack()
    {
        float lerpVal = 0.0f;
        float originAngle = light.pointLightOuterAngle;
        Vector3 vecToPlayer = player.position - transform.position;

        while (state == State.attack)
        {
            if (lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime;
                if (lerpVal > 1.0f) lerpVal = 1.0f;

                light.pointLightOuterAngle = Mathf.Lerp(originAngle, 25, lerpVal);
            }
            else
            {
                //Instantiate(pref_Projectile, light.transform.position, Quaternion.identity);
                //yield return new WaitForSeconds(1.0f);
            }
            TrackPlayer();
            vecToPlayer = player.position - transform.position;
            if (vecToPlayer.x >= 0 && direction.x != 1) FlipDirection();
            else if (vecToPlayer.x < 0 && direction.x != -1) FlipDirection();
            if (Vector3.Distance(player.position, transform.position) > attackRange)
            {
                transform.Translate(new Vector3(vecToPlayer.x > 0 ? 1 : -1, 0.0f, 0.0f) * speed * Time.deltaTime * movementLimitor);
            }

            yield return null;
        }

        lerpVal = 0.0f;
        float originZ = head.eulerAngles.z;
        originAngle = light.pointLightOuterAngle;

        while (lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            light.pointLightOuterAngle = Mathf.Lerp(originAngle, 110, lerpVal);
            head.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(originZ, direction.x == -1 ? 180.0f : 0.0f, lerpVal));
            yield return null;
        }

        activeCoroutine = null;
        yield return null;
    }


    public void FlipDirection()
    {
        Debug.Log("flipped direction");
        direction.x *= -1;
        //foreach (Transform transform in collisionReporters)
        //{
        //    Vector3 scale = transform.localScale;
        //    scale.x *= -1;
        //    transform.localScale = scale;
        //}
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        anim.SetBool("turn", false);
    }

    bool GetSeesPlayer()
    {
        Vector3 vecToPlayer = player.position - light.transform.position;

        if (vecToPlayer.sqrMagnitude < light.pointLightOuterRadius * light.pointLightOuterRadius)
        {
            float theta = Mathf.Acos(Vector2.Dot(head.right, vecToPlayer) / (head.right.magnitude * vecToPlayer.magnitude)) * Mathf.Rad2Deg;
            if (theta < light.pointLightOuterAngle / 2)
            {
                RaycastHit2D ray = Physics2D.Raycast(light.transform.position, vecToPlayer.normalized, Vector2.Distance(light.transform.position, player.position), LayerMask.GetMask("environment"));
                if (ray.collider == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void TrackPlayer()
    {
        head.right = (player.position - head.position).normalized;
    }


    void UpdateAnimationValues()
    {
        if ((!coll_ground.isColliding || (coll_wall.isColliding && coll_wall.collider.gameObject.layer == 9)) && /*!anim.GetCurrentAnimatorStateInfo(0).IsName("turn")*/ state == State.patrol)
        {
            anim.SetTrigger("turn");
        }
        anim.SetFloat("health", health);
        //if ((!coll_ground.isColliding || (coll_wall.isColliding && coll_wall.collider.gameObject.layer == 9)) && !anim.GetBool("turn"))
        //{
        //    anim.SetBool("turn", true);
        //}
    }

    void GetCollisionReportGround(CollisionPacket packet)
    {
        coll_ground = packet;
        if (!packet.isColliding)
        {
            movementLimitor = 0.0f;
        }
        else
        {
            movementLimitor = 1.0f;
        }
    }

    void GetCollisionReportWall(CollisionPacket packet)
    {
        coll_wall = packet;
        if (coll_wall.isColliding)
        {
            movementLimitor = 0.0f;
        }
        else
        {
            movementLimitor = 1.0f;
        }
    }

    void GetCollisionReportTop(CollisionPacket packet)
    {
        //packet.collider.gameObject.transform.parent = packet.isColliding ? transform : null;
    }

    void Die()
    {
        state = State.dead;
        CollisionReporter[] collisionReporters = gameObject.GetComponentsInChildren<CollisionReporter>();
        foreach (CollisionReporter cr in collisionReporters)
        {
            Destroy(cr.gameObject);
        }
    }
}
