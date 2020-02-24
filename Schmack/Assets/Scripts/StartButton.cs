using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Arrow")
        {
            GameObject.Find("start").GetComponent<SceneSwitch>().LoadScene(1);
        }
    }
}
