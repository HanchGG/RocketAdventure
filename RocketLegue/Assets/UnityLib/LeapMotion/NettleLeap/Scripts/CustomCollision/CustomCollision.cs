using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomCollision 
{
    public CustomCollider other;
    public CustomCollider self;
    public CustomCollision(CustomCollider self, CustomCollider other){
        this.other = other;
        this.self = self;
    }
}
