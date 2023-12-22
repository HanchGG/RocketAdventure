using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class Heart : MonoBehaviour
{
    public Transform RaketaTransform;
    public GameObject Raketa;
    public GameObject HP;
    private float Timer;

    void Start()
    {
        Raketa = GameObject.FindWithTag("Raketa");
        RaketaTransform = Raketa.transform;
        HP = GameObject.FindWithTag("HP");
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 30)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Raketa")
        {
            HP.GetComponent<HP>().Health +=25;
            Destroy(gameObject);
        }
    }
}