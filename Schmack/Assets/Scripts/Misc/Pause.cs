using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    float backgroundOpacity;
    float buttonOpacity;

    float timeScaleSave;

    Image background;
    Image[] menu = new Image[3];

    Controls controls;
    Animator animator;

    public static bool isPaused = false;

    private void Awake()
    {
        controls = new Controls();
        controls.Quit.quit.performed += quit => Quit();
        controls.Player.Pause.performed += pause => TogglePause();

        timeScaleSave = Time.timeScale;

        animator = GetComponent<Animator>();
        animator.SetBool("isPaused", isPaused);
    }

    // Start is called before the first frame update
    void Start()
    {
        Image[] images = GetComponentsInChildren<Image>();
        background = images[0];
        menu[0] = images[1];
        menu[1] = images[2];
        menu[2] = images[3];

        backgroundOpacity = background.color.a;
        buttonOpacity = menu[0].color.a;
    }

    void TogglePause()
    {

        animator.SetBool("isPaused", isPaused);
        Debug.Log("fascinating");
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = timeScaleSave;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            isPaused = false;
        }
        else
        {
            FindObjectOfType<Pause>().CallShowPause();
            timeScaleSave = Time.timeScale;
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
            isPaused = true;
        }
    }

    public void CallShowPause()
    {
        StartCoroutine(ShowPause());
        StartCoroutine(FlickerBurst(background));
    }

    IEnumerator ShowPause()
    {
        //background.enabled = true;
        //float lerpVal = 0.01f;
        //Vector3 scale = transform.localScale;
        //Color color = background.color;
        //while(lerpVal < 1.0f)
        //{
        //    lerpVal += Time.unscaledDeltaTime * 3.0f;
        //    if (lerpVal > 1.0f) lerpVal = 1.0f;

        //    //scale menu, don't need to call lerp function because full scale is {1, 1, 1}
        //    scale.x = lerpVal;
        //    scale.y = lerpVal;
        //    transform.localScale = scale;

        //    color.a = Mathf.Lerp(0.0f, backgroundOpacity, lerpVal);
        //    background.color = color;

        //    yield return null;
        //}
        //StartCoroutine(ShowButtons());
        yield return null;
    }

    IEnumerator ShowButtons()
    {
        //foreach (Image item in menu)
        //{
        //    item.enabled = false;
        //    item.gameObject.transform.localScale = Vector3.zero;
        //    item.color = new Color(item.color.r, item.color.g, item.color.b, 0.0f);
        //}

        //float lerpVal = 0.0f;
        //Vector3 scaleOrigin = menu[0].transform.localScale;
        //Vector3 scale = Vector3.zero;
        //Vector3 scaleTarget = menu[0].transform.localScale;
        //Color c = menu[0].color;
        //while(lerpVal < 1.0f)
        //{
        //    lerpVal += Time.unscaledDeltaTime * 3.0f;
        //    if (lerpVal > 1.0f) lerpVal = 1.0f;

        //    scale.x = lerpVal;
        //    scale.y = lerpVal;
        //    menu[0].transform.localScale = scale;

        //    c.a = Mathf.Lerp(0.0f, buttonOpacity, lerpVal);
        //    menu[0].color = c;
        //    yield return null;
        //}
        yield return null;
    }

    public void CallBackgroundFlicker()
    {
        if (Random.Range(0, 10) == 0) StartCoroutine(BackgroundFlicker());
    }

    IEnumerator BackgroundFlicker()
    {
        float originalAlpha = background.color.a;

        Color c = background.color;
        c.a = 0.0f;
        background.color = c;
        yield return new WaitForSecondsRealtime(0.1f);
        c.a = backgroundOpacity;
        background.color = c;

    }

    IEnumerator FlickerBurst(Image img)
    {
        //int numFlickers = Random.Range(3, 6);
        //Color c = img.color;
        //float originalAlpha = img.color.a;
        //for(int i = 0; i < numFlickers; i++)
        //{
        //    c.a = 0.0f;
        //    img.color = c;
        //    float timer = 0.0f;
        //    while(timer < Random.Range(0.05f, 0.1f))
        //    {
        //        timer += Time.unscaledDeltaTime;
        //        yield return null;
        //    }
        //    c.a = originalAlpha;
        //    img.color = c;
        //    float waitTime = Random.Range(0.1f, 0.2f);
        //    while(timer < waitTime)
        //    {
        //        timer += Time.unscaledDeltaTime;
        //        yield return null;
        //    }

        //}
        //c.a = backgroundOpacity;
        //img.color = c;
        yield return null;
    }

    void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
