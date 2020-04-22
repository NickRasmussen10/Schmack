using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Temp UI stuff
    /// </summary>
    /// 
    Text bowText = null;
    List<Image> displayArrows = new List<Image>();

    PlayerMovement playerMovement = null;
    [SerializeField] GameObject rotator = null;

    [SerializeField] float maxHealth = 1.0f;
    float health;

    //[SerializeField] GameObject[] bows = null;
    public GameObject currentBow;
    Bow bowScript;

    [HideInInspector] public SpriteRenderer bowSprite;
    [SerializeField] Transform IKTarget;
    [SerializeField] GameObject[] IKObjects = new GameObject[2];

    [Header("Timescaling")]
    [SerializeField] float timeScaleMin = 0.1f; //the slowest the game will go on bow drawback
    [SerializeField] float timeScaleMax = 1.0f; //the fastest the game will go otherwise (1.0f is normal)
    [SerializeField] float timeDialtionTime = 2.0f;
    [SerializeField] float timeLerpMultiplier = 1.0f;
    [HideInInspector] public float timeScale;

    CameraManager cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        health = maxHealth;

        //currentBow = bows[0];
        bowScript = currentBow.GetComponent<Bow>();

        //UI
        if(GameObject.Find("BowText").GetComponent<Text>()) bowText = GameObject.Find("BowText").GetComponent<Text>();
        displayArrows.Add(GameObject.Find("Arrow (0)").GetComponent<Image>());
        displayArrows.Add(GameObject.Find("Arrow (1)").GetComponent<Image>());
        displayArrows.Add(GameObject.Find("Arrow (2)").GetComponent<Image>());

        bowText.text = currentBow.name;
        cameraManager = FindObjectOfType<CameraManager>();

        timeScale = timeScaleMax;
    }

    // Update is called once per frame
    void Update()
    {
        if(health<=0.5f)
        {
            //bowSprite.color = Color.red;
        }

        if(health == 0)
        {
            Die();
        }

        if(bowScript.inFlow != playerMovement.inFlow)
        {
            bowScript.inFlow = playerMovement.inFlow;
        }

        HandleIK();

        //if (GetComponent<Rigidbody2D>().velocity.x > 2.0f && bowScript.state != Bow.State.drawn)
        //{
        //    rotator.transform.right = Vector2.right;
        //}
        //else
        //{
        //    rotator.transform.right = (rotator.transform.position + (Vector3)bowScript.direction) - rotator.transform.position;
        //    if (transform.localScale.x == -1) rotator.transform.right *= -1;
        //}

        DisplayArrowCount();
    }

    private void LateUpdate()
    {
        //rotator.transform.up = ((rotator.transform.position + (Vector3)bowScript.direction) - rotator.transform.position) * -1;
    }

    //private void OnAnimatorMove()
    //{
        
    //    rotator.transform.up = ((rotator.transform.position + (Vector3)bowScript.direction) - rotator.transform.position) * -1;
    //    Debug.Log(rotator.transform.up);
    //}

    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("it's called");
        rotator.transform.up = ((rotator.transform.position + (Vector3)bowScript.direction) - rotator.transform.position) * -1;
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

    //void GoToNextBow()
    //{
    //    int bowIndex = -1;
    //    for(int i = 0; i < bows.Length; i++)
    //    {
    //        if (currentBow == bows[i]) bowIndex = i;
    //    }
    //    bowIndex++;
    //    if (bowIndex == bows.Length) bowIndex = 0;
    //    currentBow.SetActive(false);
    //    currentBow = bows[bowIndex];
    //    bowScript = currentBow.GetComponent<Bow>();
    //    bowText.text = currentBow.name;
    //    currentBow.SetActive(true);
    //}



    public void TakeDamage(float damage)
    {
        if (health < 0) return;

        health -= damage;
        if (health < 0) health = 0;
        cameraManager.SendMessage("CallDisplayDamage", 5.0f);
    }

    private void Die()
    {
        //health = -1;
        //gameObject.GetComponent<PlayerMovement>().enabled = false;
        //cameraManager.SendMessage("CallDisplayDeath", transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// handles the activation and deactivation of arm inverse kinematics based on aim and draw back input
    /// also handles moving IK target to reflect player's aim
    /// </summary>
    void HandleIK()
    {
        Vector2 directionInput = Inputs.controls.Player.Aim.ReadValue<Vector2>();
        float powerInput = Inputs.controls.Player.Draw.ReadValue<float>();

        
        
        if(directionInput.sqrMagnitude > 0.81f || powerInput == 1.0f)
        {
            IKTarget.position = transform.position + new Vector3(0.0f, 0.51f, 0.0f) + (Vector3)bowScript.direction * 10.0f;
        }
        else
        {
            IKTarget.position = transform.position + new Vector3(0.0f, 0.51f, 0.0f) + (transform.localScale.x < 0 ? Vector3.left : Vector3.right);
        }

        
        
        if (powerInput < 1.0f && directionInput.sqrMagnitude < 0.5f)
        {
            SetIKActive(false);
        }
        else
        {
            SetIKActive(true);
        }

        if (powerInput == 1.0f && IKObjects[1].activeSelf)
        {
            IKObjects[1].SetActive(false);
        }
    }


    //this method is called from an animation event
    void TimeDilationDown()
    {
        StartCoroutine(TimeDilation());
    }

    IEnumerator TimeDilation()
    {
        Time.timeScale = timeScaleMin;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(timeDialtionTime);

        if(Time.timeScale != 1.0f)
        {
            float lerpVal = 0.0f;
            while(lerpVal < 1.0f)
            {
                lerpVal += Time.unscaledDeltaTime * timeLerpMultiplier;
                lerpVal += lerpVal / 50; //lazy version of "exponential" growth
                if (lerpVal > 1.0f) lerpVal = 1.0f;

                Time.timeScale = Mathf.Lerp(timeScaleMin, timeScaleMax, lerpVal);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return null;
            }
        }
    }

    //this method is called from an animation event
    void TimeDilationUp()
    {
        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void SetIKActive(bool isActive)
    {
        foreach (GameObject IKObject in IKObjects)
        {
            IKObject.SetActive(isActive);
        }
    }

    void ToggleIKOn()
    {
        SetIKActive(true);
    }

    void ToggleIKOff()
    {
        SetIKActive(false);
    }

    bool GetIKActive() { return IKObjects[0].activeSelf; }
}
