using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Temp UI stuff
    /// </summary>
    /// 
    [SerializeField] Text bowText = null;
    [SerializeField] List<Image> displayArrows = new List<Image>();
    [SerializeField] SpriteRenderer bigsad;

    PlayerMovement playerMovement = null;
    [SerializeField] GameObject rotator = null;

    [SerializeField] float maxHealth = 1.0f;
    float health;

    [SerializeField] GameObject[] bows = null;
    public GameObject currentBow;
    Bow bowScript;

    public SpriteRenderer bowSprite;

    CameraManager cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        health = maxHealth;

        currentBow = bows[0];
        bowScript = currentBow.GetComponent<Bow>();
        bowText.text = currentBow.name;
        cameraManager = FindObjectOfType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health<=0.5f)
        {
            bowSprite.color = Color.red;
        }

        if(health == 0)
        {
            Die();
        }

        if(bowScript.inFlow != playerMovement.inFlow)
        {
            bowScript.inFlow = playerMovement.inFlow;
        }

        if (GetComponent<Rigidbody2D>().velocity.x > 2.0f && bowScript.state != Bow.State.drawn)
        {
            rotator.transform.right = Vector2.right;
        }
        else
        {
            rotator.transform.right = (rotator.transform.position + (Vector3)bowScript.direction) - rotator.transform.position;
            if (transform.localScale.x == -1) rotator.transform.right *= -1;
        }

        DisplayArrowCount();
    }

    void DisplayArrowCount()
    {
        for(int i = 1; i <= displayArrows.Count; i++)
        {
            if(i > bowScript.numArrows)
            {
                displayArrows[i-1].enabled = false;
            }
            else
            {
                displayArrows[i-1].enabled = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Vector2 knockback = (gameObject.transform.position - collision.gameObject.transform.position).normalized * collision.gameObject.GetComponent<Enemy>().GetKnockback();
            //gameObject.GetComponent<PlayerMovement>().AddKnockback(knockback, false);
            //TakeDamage(25);
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
        bowText.text = currentBow.name;
        currentBow.SetActive(true);
    }


    public void TakeDamage(float damage)
    {
        if (health < 0) return;

        health -= damage;
        if (health < 0) health = 0;
        cameraManager.SendMessage("CallDisplayDamage", 5.0f);
    }

    private void Die()
    {
        health = -1;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        cameraManager.SendMessage("CallDisplayDeath", transform.position);
    }
}
