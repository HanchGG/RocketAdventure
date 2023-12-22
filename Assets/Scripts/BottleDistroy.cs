using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class BottleDistroy : MonoBehaviour
{
    public GameObject Bottle;
    public int score;
    [SerializeField] TextMeshProUGUI ScoreText;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {

        }

    }
    private void Update()
    {
        ScoreText.text = score.ToString();
    }
}
