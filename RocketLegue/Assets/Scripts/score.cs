using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class score : MonoBehaviour
{
    private float Timer;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Raketa")
        {
            ScoreManager.score+=1;
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 6)
        {
            Destroy(gameObject);    
        }
    }
}