using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetEvent : MonoBehaviour
{
    public int PlanetID = 0;
    public float _time = 0;
    public Vector3 directional;
    public GameObject[] Planets;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _time += Time.deltaTime;
        if (_time > 10)
        {
            if (PlanetID > Planets.Length - 1)
            {
                PlanetID = 0;
            }
            Planets[PlanetID].transform.Translate(directional);

            if(_time > 40)
            {
                PlanetID += 1;
                _time = 0;
            }


            
        }
    }
}
