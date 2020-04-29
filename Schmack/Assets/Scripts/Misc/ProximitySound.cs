using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    [SerializeField] float volumeMultiplier = 1.0f;
    [SerializeField] float fallOff = 1.0f;

    AudioSource source;
    Transform player;
    float distToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector3.Distance(player.position, transform.position);
        source.volume = 1 / (distToPlayer * fallOff);
        source.volume *= volumeMultiplier;
    }
}
