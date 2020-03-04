using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TargetManager : MonoBehaviour
{
    [SerializeField] List<GameObject> targets = new List<GameObject>();
    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Targets: " + targets.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Delete(GameObject target)
    {
        for(int i = targets.Count - 1; i >= 0; i--)
        {
            if(targets[i] == target)
            {
                Destroy(targets[i]);
                targets.RemoveAt(i);
            }
        }

        if(targets.Count > 0) text.text = "Targets: " + targets.Count;
    }
}
