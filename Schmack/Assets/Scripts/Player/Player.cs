using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Temp UI stuff
    /// </summary>
    /// 
    [SerializeField] Text bowText = null;
    //[SerializeField] Canvas canvas = null;
    //[SerializeField] Sprite displayArrow = null;
    [SerializeField] List<Image> displayArrows = new List<Image>();


    [SerializeField] float maxHealth = 100;
    float health;

    [SerializeField] GameObject[] bows = null;
    public GameObject currentBow;
    Bow bowScript;

    [SerializeField] GameObject pref_indicator = null;
    [SerializeField] float indicatorDistance = 5.0f;
    GameObject indicator;

    public SpriteRenderer bowSprite;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        currentBow = bows[0];
        bowScript = currentBow.GetComponent<Bow>();
        bowText.text = currentBow.name;
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
        //for(int i = displayArrows.Count - 1; i >= 0; i--)
        //{
        //    if (i < bowScript.numArrows)
        //    {
        //        Destroy(displayArrows[i]);
        //        displayArrows.RemoveAt(i);
        //    }
        //}
        //for(int i = 0; i < bowScript.numArrows; i++)
        //{
        //    if(i > displayArrows.Count - 1)
        //    {
        //        GameObject dispArrow = new GameObject();
        //        dispArrow.AddComponent<Image>();
        //        dispArrow.GetComponent<Image>().sprite = displayArrow;
        //        Vector2 position = new Vector2(-700, 300);
        //        position.x += i * displayArrow.rect.width;
        //        dispArrow.transform.position = position;
        //        dispArrow.transform.SetParent(canvas.transform);
        //    }
        //}
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
        bowText.text = currentBow.name;
        currentBow.SetActive(true);

        //change this to a better method when you're not 2 hours from a meeting
        if(currentBow.name == "Power Bow")
        {
            indicatorDistance = 3;
        }
        else
        {
            indicatorDistance = 5;
        }
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
