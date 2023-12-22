using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HP : MonoBehaviour
{
    public int Health = 100;
    public float Timer;
    public int Score;
    private int LastScore;
    private GameObject Raketa;
    private GameObject Stena;
    public ScoreManager scoreManager;

    void Start()
    {
        Stena = GameObject.FindWithTag("Stena");
        Raketa = GameObject.Find("rakate");
    }


    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 10)
        {
            Timer = 0;
            Health += 10;
        }
        if (Score > LastScore)
        {
            Health -= 5;
            LastScore = Score;
        }
        if (Health > 100)
        {
            Health = 100;
        }
        if (Health <= 0)
        {
                Health = 0;
                Destroy(Raketa);
                Stena.GetComponent<Restart>().Timer = 1;
        }
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            Health += 100;
        }
        transform.localScale = new Vector3(0.25f, 0.25f, Health * 0.0025f);
    }
}
