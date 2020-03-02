using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : ShootButton
{
    public override void Activate()
    {
        SceneSwitch.LoadScene(1);
    }
}
