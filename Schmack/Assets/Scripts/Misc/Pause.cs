using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    float backgroundOpacity;

    float timeScaleSave;

    [SerializeField] bool enableMenu = true;

    [SerializeField] Image background;
    [SerializeField] GameObject[] buttons;

    Controls controls;
    Animator animator;

    public static bool isPaused = false;

    private void Awake()
    {
        controls = new Controls();
        controls.Quit.quit.performed += quit => Quit();
        controls.Player.Pause.performed += pause => TogglePause();

        timeScaleSave = Time.timeScale;

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        animator = GetComponent<Animator>();
        animator.SetBool("isPaused", isPaused);
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundOpacity = background.color.a;
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = timeScaleSave;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            isPaused = false;
            if (FindObjectOfType<PlayerMovement>()) FindObjectOfType<PlayerMovement>().enabled = true;
            foreach (GameObject button in buttons)
            {
                button.SetActive(false);
            }
        }
        else
        {
            timeScaleSave = Time.timeScale;
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
            isPaused = true;
            if(FindObjectOfType<PlayerMovement>()) FindObjectOfType<PlayerMovement>().enabled = false;
            foreach (GameObject button in buttons)
            {
                button.SetActive(true);
            }
        }
        animator.SetBool("isPaused", isPaused);
    }

    public void CallShowPause()
    {
        StartCoroutine(ShowPause());
    }

    IEnumerator ShowPause()
    {
        float lerpVal = 0.0f;
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.unscaledDeltaTime;
            animator.Play("ShowPause", 0, lerpVal);
            yield return null;
        }
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

    public void Restart()
    {
        TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        TogglePause();
        SceneManager.LoadScene(0);
    }

    public void Quit()
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
