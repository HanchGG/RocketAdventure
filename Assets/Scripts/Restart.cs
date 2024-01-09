using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public float Timer;
    public ScoreManager scoreManager = new ScoreManager();
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
            if (scoreManager.data.hScore > scoreManager.data.OldhScore)
            {
                Timer = 0;
                SceneManager.LoadScene(2);
            }
            else
            {
                Timer = 0;
                SceneManager.LoadScene(0);
            }

        }
    }

}
