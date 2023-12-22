using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI.WebControls.WebParts;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private float Timer;
    private float HealthTimer;
    private float BottleTimer;
    public GameObject Asteroid;
    public GameObject Asteroid1;
    public GameObject Bottle;
    public GameObject Heart;
    public Transform Raketa;
    public Transform Look;
    GameObject OneMoreAsteroid;
    public float Speed = 10000;// for mass 1kg

    void FixedUpdate()
    {
        Timer += Time.deltaTime;
        HealthTimer += Time.deltaTime;
        BottleTimer += Time.deltaTime;

        if (Timer > 0.5f)
        {
            Timer = 0;
            int Rnd = Random.Range(1, 5);
            int Rand = Random.Range(1, 3);
            if (Rand == 1)
              
            {
                if (Rnd == 1)
                {
                    OneMoreAsteroid = Instantiate(Asteroid, new Vector3(90, 10, Random.Range(-45f, 45f)), Quaternion.identity);
                    AsteroidSpawn(Asteroid, new Vector3(90, 10, Random.Range(-45f, 45f)));
                }
                else if (Rnd == 2) AsteroidSpawn(Asteroid, new Vector3(-43, 10, Random.Range(-45f, 45f)));
                else if (Rnd == 3) AsteroidSpawn(Asteroid, new Vector3(Random.Range(-90f, 90f), 10, 45));
                else if (Rnd == 4) AsteroidSpawn(Asteroid, new Vector3(Random.Range(-90f, 90f), 10, -45));
            }
            else if (Rand == 2)
            {
                if (Rnd == 1) AsteroidSpawn(Asteroid1, new Vector3(90, 10, Random.Range(-45f, 45f)));
                else if (Rnd == 2) AsteroidSpawn(Asteroid1, new Vector3(90, 10, Random.Range(-45f, 45f)));
                else if (Rnd == 3) AsteroidSpawn(Asteroid1, new Vector3(Random.Range(-90f, 90f), 10, 45));
                else if (Rnd == 4) AsteroidSpawn(Asteroid1, new Vector3(Random.Range(-90f, 90f), 10, -45));
            }
        }

        if (HealthTimer > 14)
        {
            HealthTimer = 0;
            int Rnd = Random.Range(1, 5);
            if (Rnd == 1) OtherSpawn(Heart, new Vector3(90, 14, Random.Range(-45f, 45f)), true);
            else if (Rnd == 2) OtherSpawn(Heart, new Vector3(-90, 14, Random.Range(-45f, 45f)), true);
            else if (Rnd == 3) OtherSpawn(Heart, new Vector3(Random.Range(-90f, 90f), 14, 45), true);
            else if (Rnd == 4) OtherSpawn(Heart,new Vector3(Random.Range(-90f, 90f), 14, -45), true);
        }
        if (BottleTimer > 5f)
        {
            BottleTimer = 0;
            int rando = Random.Range(1, 5);
            if (rando == 1) OtherSpawn(Bottle, new Vector3(90, 14, Random.Range(-45f, 45f)), false);
            else if (rando == 2) OtherSpawn(Bottle, new Vector3(-90, 14, Random.Range(-45f, 45f)), false);
            else if (rando == 3) OtherSpawn(Bottle, new Vector3(Random.Range(-90f, 90f), 14, 45), false);
            else if (rando == 4) OtherSpawn(Bottle, new Vector3(Random.Range(-90f, 90f), 14, -45), false);
        }
        
    }
    public void AsteroidSpawn(GameObject _object, Vector3 a)
    {
        OneMoreAsteroid = Instantiate(_object, a, Quaternion.identity);
        OneMoreAsteroid.transform.LookAt(Raketa);
        OneMoreAsteroid.GetComponent<Rigidbody>().velocity = Speed * Time.deltaTime * OneMoreAsteroid.transform.forward;
    }
    public void OtherSpawn(GameObject _object, Vector3 a, bool k) 
    {
            GameObject OneMoreHeart = Instantiate(_object, a, Quaternion.identity);
            OneMoreHeart.transform.LookAt(Look);
            OneMoreHeart.GetComponent<Rigidbody>().velocity = 0.5f * Speed * Time.deltaTime * OneMoreHeart.transform.forward;
        if (k == true)
        {
            OneMoreHeart.transform.Rotate(0, 90, 90);
        }
        else OneMoreHeart.transform.Rotate(90, 0, 0);

     
    }
}
