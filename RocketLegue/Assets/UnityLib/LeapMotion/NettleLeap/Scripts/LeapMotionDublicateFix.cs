using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionDublicateFix : MonoBehaviour {


        public Vector3 StartLocalPosition = Vector3.zero;
        public Quaternion StartLocalRotation = Quaternion.identity;
        public Vector3 StartLocalScale = Vector3.one;

        void Start() {
            transform.localPosition = StartLocalPosition;
            transform.localRotation = StartLocalRotation;
            transform.localScale = StartLocalScale;

        }
    }
}
