using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class Asteroid : MonoBehaviour
{
    // Скрипт который удаляет астероиды при столкновении, наносит урон когда это необходимо.
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
    private void OnCollisionEnter(Collision other) //It works when objects collidе only with a rocket
    {
        if (other.gameObject.tag == "Raketa")
            HP.GetComponent<HP>().Score++;
        Instantiate(Particles, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}