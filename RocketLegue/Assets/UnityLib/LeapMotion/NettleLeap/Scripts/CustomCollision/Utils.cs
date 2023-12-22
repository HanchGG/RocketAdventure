using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionUtils : MonoBehaviour
{
    public class Box
    {
        Vector3 Max;
        Vector3 Min;
        Box(Vector3 Max, Vector3 Min)
        {
            this.Max = Max;
            this.Min = Min;
        }
        Box()
        {
        }
    }

    [System.Serializable]
public class CustomCollisionEvent : UnityEvent<CustomCollision>
{
}
}
