using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }
}
