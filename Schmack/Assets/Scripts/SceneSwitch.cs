using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneSwitch
{
    public static void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);

    }
}
