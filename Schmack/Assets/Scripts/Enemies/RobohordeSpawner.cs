using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobohordeSpawner : MonoBehaviour
{
    [SerializeField] GameObject pref_robohorde;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnRate = 5.0f;
    [SerializeField] float visionRange = 1.0f;

    public float direction = 1.0f;

    [SerializeField] Transform[] pathway;
    RobohordeManager roboHorde;
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
            if(c_spawn == null && roboHorde == null)
            {
                c_spawn = StartCoroutine(Spawn());
            }
        }

        if(roboHorde != null)
        {
            if (!roboHorde.IsAlive()) roboHorde = null;
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
        animator.SetBool("isOpen", true);
        yield return new WaitForSeconds(0.75f);
        roboHorde = Instantiate(pref_robohorde, spawnPoint.position, Quaternion.identity).GetComponent<RobohordeManager>();
        RobohordeManager manager = roboHorde.GetComponent<RobohordeManager>();
        manager.SetPath(pathway);
        manager.SpawnLeader(new Vector3(direction * 50, 0.0f, 0.0f));
        for (int i = 0; i < manager.hordeSize; i++)
        {
            yield return new WaitForSeconds(0.075f);
            manager.SpawnFollower(new Vector3(direction * Random.Range(15, 25), Random.Range(-5.0f, 5.0f), 0.0f));
        }

        manager.EnableAttack();
        animator.SetBool("isOpen", false);
        c_spawn = null;
    }
}
