using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public float Timer;

    void Start()
    {

    }


    void Update()
    {
        if (Timer >= 1)
        {
            Timer += Time.deltaTime;
        }
        if (Timer > 2)
        {
            Timer = 0;
            SceneManager.LoadScene(0);
        }
    }

}
