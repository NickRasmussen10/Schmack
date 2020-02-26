using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Quit.quit.performed += quit => Quit();
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
