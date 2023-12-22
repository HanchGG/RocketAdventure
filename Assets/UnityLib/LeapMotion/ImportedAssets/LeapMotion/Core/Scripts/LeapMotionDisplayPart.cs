using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nettle.NettleLeap {
    public class LeapMotionDisplayPart : MonoBehaviour {


        void Awake() {
            Transform display = FindObjectOfType<MotionParallaxDisplay>().transform;
            transform.SetParent(display,false);
            Destroy(this);
        }

    }

}