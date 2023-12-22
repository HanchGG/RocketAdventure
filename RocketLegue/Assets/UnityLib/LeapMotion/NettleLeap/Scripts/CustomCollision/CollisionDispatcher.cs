using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDispatcher : MonoBehaviour
{
    public static CollisionDispatcher Instance;
    [SerializeField] bool isVerbose = false;
    private List<CustomCollider> activeColliders;
    private List<CustomCollision> collisionsInProgress;
    public bool UpdateCollisions = true;
    void Awake()
    {
        activeColliders = new List<CustomCollider>();
        collisionsInProgress = new List<CustomCollision>();
        Instance = this;
        StartCoroutine(CustomPhysicsUpdate());
        if(isVerbose)
            Debug.Log("custom collision started");
    }

    public void SubscribeCollider(CustomCollider customCollider)
    {
        activeColliders.Add(customCollider);
        if(isVerbose)
            Debug.Log("collider subscribed");
    }

    public void UnsubscribeCollider(CustomCollider customCollider)
    {
        activeColliders.Remove(customCollider);
        if(isVerbose)
            Debug.Log("collider unsubscribed");
    }

    private IEnumerator CustomPhysicsUpdate()
    {
        while (true)
        {
            for (int i = 0; i < activeColliders.Count; i++)
            {
                CustomCollider a = activeColliders[i];
                for (int j = 0; j < activeColliders.Count; j++)
                {
                    if (i == j)
                        continue;

                    CustomCollider b = activeColliders[j];
                    if (CheckCollision((dynamic)a, (dynamic)b))
                    {
                        if (IsCollisionAlreadyHapenning(a, b))
                        {
  //                          if(isVerbose)
//                                Debug.Log("collision already happening between " + a.gameObject.name + " and " + b.gameObject.name);
                        }
                        else
                        {
                            if(isVerbose)
                                Debug.Log("on collision enter between " + a.gameObject.name + " and " + b.gameObject.name);
                            a.OnCollisionEnter?.Invoke(new CustomCollision(a, b));
                            b.OnCollisionEnter?.Invoke(new CustomCollision(b, a));
                            collisionsInProgress.Add(new CustomCollision(a, b));
                        }
                    }
                    else
                    {
                        if (IsCollisionAlreadyHapenning(a, b))
                        {
                            if(isVerbose)    
                                Debug.Log("on collision exit between " + a.gameObject.name + " and " + b.gameObject.name);
                            a.OnTriggerExit?.Invoke(new CustomCollision(a, b));
                            b.OnTriggerExit?.Invoke(new CustomCollision(b, a));
                            RemoveCollisionByCollider(a, b);
                        }
                    }
                }
            }
            yield return null;
        }
    }
    
    private bool CheckCollision(CustomBoxCollider a, CustomBoxCollider b)
    {
        return (a.Min.x <= b.Max.x && a.Max.x >= b.Min.x) &&
                (a.Min.y <= b.Max.y && a.Max.y >= b.Min.y) &&
                (a.Min.z <= b.Max.z && a.Max.z >= b.Min.z);
    }

    private bool IsCollisionAlreadyHapenning(CustomCollider a, CustomCollider b)
    {
        for (int i = 0; i < collisionsInProgress.Count; i++)
        {
            var t = collisionsInProgress[i];
            if (t.other == a && t.self == b || t.other == b && t.self == a)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveCollisionByCollider(CustomCollider a, CustomCollider b)
    {
        List<CustomCollision> markedToRemove = new List<CustomCollision>();

        for (int i = 0; i < collisionsInProgress.Count; i++)
        {
            var t = collisionsInProgress[i];
            if (t.other == a && t.self == b || t.other == b && t.self == a)
            {
                markedToRemove.Add(t);
            }
        }

        foreach (var item in markedToRemove)
        {
            collisionsInProgress.Remove(item);
        }
    }
}
