using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float Speed;
    public float RotationSpeed;
    public Rigidbody Raketa;
    void FixedUpdate()
    {
        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");
        if (V > 0)
        {
            Raketa.GetComponent<Rigidbody>().velocity = V * Speed * transform.forward;
        }
        Raketa.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, H, 0f) * RotationSpeed;
    }
}
