using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hotkey : MonoBehaviour
{
    public KeyCode Key;
    public UnityEvent OnPresed;
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            if (OnPresed != null)
            {
                OnPresed.Invoke();
            }
        }
    }
}
