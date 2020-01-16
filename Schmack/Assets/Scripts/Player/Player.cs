﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    float health;

    [SerializeField] GameObject[] bows = null;
    public GameObject currentBow;
    Bow bowScript;

    [SerializeField] GameObject pref_indicator;
    [SerializeField] float indicatorDistance = 5.0f;
    GameObject indicator;

    public SpriteRenderer bowSprite;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        currentBow = bows[0];
        bowScript = currentBow.GetComponent<Bow>();
        indicator = Instantiate(pref_indicator);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SwapWeapon"))
        {
            GoToNextBow();
        }

        SetIndicatorPosition();

        if(health<=50)
        {
            bowSprite.color = Color.red;
        }

        if(health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Vector2 knockback = (gameObject.transform.position - collision.gameObject.transform.position).normalized * collision.gameObject.GetComponent<Enemy>().GetKnockback();
            gameObject.GetComponent<PlayerMovement>().AddKnockback(knockback, false);
            TakeDamage(25);
        }
    }

    void GoToNextBow()
    {
        int bowIndex = -1;
        for(int i = 0; i < bows.Length; i++)
        {
            if (currentBow == bows[i]) bowIndex = i;
        }
        bowIndex++;
        if (bowIndex == bows.Length) bowIndex = 0;
        currentBow.SetActive(false);
        currentBow = bows[bowIndex];
        bowScript = currentBow.GetComponent<Bow>();
        currentBow.SetActive(true);
    }

    protected void SetIndicatorPosition()
    {
        indicator.transform.position = (Vector2)transform.position + (bowScript.direction * indicatorDistance);
    }


    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneSwitch>().LoadScene(2);
    }
}
