using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nettle.NettleLeap {

    public class LeapMotionScrollViewController : MonoBehaviour {
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private Canvas _canvas;

        public float LerpSpeed;

        private LeapMotionPokeFinger Finger;
        private Vector3 _lastFingerPosition;

        private bool _needReleaseFinger = false;
        private float _desireDelta = 0;

        // Update is called once per frame
        void Update() {
            if (Finger) {
                Vector3 currentPosition = GetFingerPosition(Finger);    
                float delta = _lastFingerPosition.z - currentPosition.z;                
                float multiplier = _canvas.GetComponent<RectTransform>().rect.height * transform.localScale.z * _scrollRect.verticalScrollbar.size * 0.02f; 
                _desireDelta += delta * multiplier;
                float resultDelta = Mathf.Lerp(0, _desireDelta, Time.deltaTime * LerpSpeed);
                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition + resultDelta);
                _desireDelta -= resultDelta;
                _lastFingerPosition = currentPosition;
            } else {
            }
        }

        private void FixedUpdate() {
            if (_needReleaseFinger) {
                Release();
            } else {
                _needReleaseFinger = true;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (Finger) {
                return;
            }
            var finger = other.GetComponent<LeapMotionPokeFinger>();
            if (finger) {
                Finger = finger;
                _lastFingerPosition = GetFingerPosition(finger);
            }
        }

        private void OnTriggerStay(Collider other) {
            if (Finger == null) {
                OnTriggerEnter(other);
            }
            var finger = other.GetComponent<LeapMotionPokeFinger>();
            if (finger == Finger) {
                _needReleaseFinger = false;
            }

        }

        private void OnTriggerExit(Collider other) {
            if (other.GetComponent<LeapMotionPokeFinger>() == Finger) {
                Release();
            }
        }

        private void Release() {
            Finger = null;
            _desireDelta = 0;
        }

        private Vector3 GetFingerPosition(LeapMotionPokeFinger finger) {
            return transform.InverseTransformPoint(finger.transform.position);
        }
    }

}