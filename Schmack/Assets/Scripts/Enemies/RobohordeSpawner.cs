using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobohordeSpawner : MonoBehaviour
{
    [SerializeField] GameObject pref_robohorde;
    [SerializeField] float spawnRate = 5.0f;
    [SerializeField] float visionRange = 1.0f;

    public float direction = 1.0f;

    [SerializeField] Transform[] pathway;
    GameObject roboHorde;
    Transform player;
    Animator animator;

    Coroutine c_spawn;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if((player.position - transform.position).sqrMagnitude < visionRange * visionRange)
        {
            if(c_spawn == null)
            {
                c_spawn = StartCoroutine(Spawn());
            }
        }
        else if(c_spawn != null)
        {
            StopCoroutine(c_spawn);
            c_spawn = null;
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
        animator.SetBool("isOpen", true);
        yield return new WaitForSeconds(1.0f);
        Instantiate(pref_robohorde, transform.position + (Vector3)((direction == 1.0f ? Vector2.right : Vector2.left) * 5.0f), Quaternion.identity).GetComponent<RobohordeManager>().SetPath(pathway);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isOpen", false);
    }
}
