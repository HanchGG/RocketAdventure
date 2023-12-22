using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class Asteroid : MonoBehaviour
{
    private Transform RaketaTransform;
    private GameObject Raketa;
    private GameObject HP;
    private float Timer;
    [SerializeField] private GameObject Particles;

    void Start()
    {
        Raketa = GameObject.FindWithTag("Raketa");
        RaketaTransform = Raketa.transform;
        HP = GameObject.FindWithTag("HP");
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 15)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Raketa")
            HP.GetComponent<HP>().Score++;
        Instantiate(Particles, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}