using System;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionSliderHandler : LeapMotionUIActiveElement {

        public Action<Transform, bool> OnPressed;
        public Vector3 ColliderSizeMultiplier;

        private LeapMotionSlider _slider;
        private BoxCollider _collider;
        private Vector3 _colliderDefaultSize;

        protected override void Awake() {
            base.Awake();
            _slider = transform.parent.GetComponent<LeapMotionSlider>();
            _collider = GetComponent<BoxCollider>();
            _colliderDefaultSize = _collider.size;
        }

        public override void OnTriggerEnter(Collider collider) {
            bool stateBeforeTrigger = _pressed;
            base.OnTriggerEnter(collider);
            if (stateBeforeTrigger == false && _pressed) {
                _slider.StartHandlerFollow(collider.transform);
                _collider.size = new Vector3(_colliderDefaultSize.x * ColliderSizeMultiplier.x, _colliderDefaultSize.y * ColliderSizeMultiplier.y, _colliderDefaultSize.z * ColliderSizeMultiplier.z);
            }            
        }

        public override void OnTriggerExit(Collider collider) {
            bool stateBeforeTrigger = _pressed;
            base.OnTriggerExit(collider);
            if (stateBeforeTrigger && !_pressed) {
                _slider.StopHandlerFollow();
                _collider.size = _colliderDefaultSize;
            }
        }
    }
}
