using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Nettle;

public class CustomCollider:MonoBehaviour
{
    public CollisionUtils.CustomCollisionEvent OnTriggerEnter;
    public CollisionUtils.CustomCollisionEvent OnTriggerExit;
    public CollisionUtils.CustomCollisionEvent OnCollisionEnter;
    public CollisionUtils.CustomCollisionEvent OnCollisionExit;
    public bool IsTrigger;

    protected void Awake(){
        CollisionDispatcher.Instance.SubscribeCollider(this);
    }

    void OnDestroy(){
        CollisionDispatcher.Instance.UnsubscribeCollider(this);
    }
}
