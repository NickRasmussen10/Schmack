using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    Controls controls;

    float timeScaleSave;

    private void Awake()
    {
        controls = new Controls();
        controls.Quit.quit.performed += quit => Quit();
        controls.Player.Pause.performed += pause => TogglePause();

        timeScaleSave = Time.timeScale;
    }

    void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    void TogglePause()
    {
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = timeScaleSave;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            timeScaleSave = Time.timeScale;
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
        }
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
