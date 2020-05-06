using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    float timeScaleSave;
    Vector2 gravitySave;

    [SerializeField] bool enableMenu = true;

    [SerializeField] Image background;
    [SerializeField] GameObject[] buttons;

    public static bool isPaused = false;

    private void Awake()
    {
        timeScaleSave = Time.timeScale;
        gravitySave = Physics2D.gravity;

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Inputs.controls.Player.Pause.performed += pause => TogglePause();
        Inputs.controls.Quit.quit.performed += quit => Quit();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Debug.Log("unpause");
            Time.timeScale = timeScaleSave;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Physics2D.gravity = gravitySave;
            isPaused = false;
            if (FindObjectOfType<PlayerMovement>()) FindObjectOfType<PlayerMovement>().enabled = true;
            background.gameObject.SetActive(false);
            foreach (GameObject button in buttons)
            {
                button.SetActive(false);
            }
        }
        else
        {
            Debug.Log("pause");
            timeScaleSave = Time.timeScale;
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
            Physics2D.gravity = Vector2.zero;
            isPaused = true;
            if(FindObjectOfType<PlayerMovement>()) FindObjectOfType<PlayerMovement>().enabled = false;
            background.gameObject.SetActive(true);
            foreach (GameObject button in buttons)
            {
                button.SetActive(true);
            }
        }
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

    //public void CallBackgroundFlicker()
    //{
    //    if (Random.Range(0, 10) == 0) StartCoroutine(BackgroundFlicker());
    //}

    //IEnumerator BackgroundFlicker()
    //{
    //    float originalAlpha = background.color.a;

    //    Color c = background.color;
    //    c.a = 0.0f;
    //    background.color = c;
    //    yield return new WaitForSecondsRealtime(0.1f);
    //    c.a = backgroundOpacity;
    //    background.color = c;
    //}
}
