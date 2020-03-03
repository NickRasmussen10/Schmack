using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : ShootButton
{
    public override void Activate()
    {
        SceneManager.LoadScene(1);
    }
}
