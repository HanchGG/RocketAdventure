using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameOfKey : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log(Input.inputString);
        }
    }
}
