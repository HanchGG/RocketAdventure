using System.Collections;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionSlider : MonoBehaviour {

        public LeapMotionVirtualSlider VirtualSlider;

        public float Width;
        public LeapMotionSliderHandler Handler;
        public float LerpSpeed = 10f;

        private Coroutine _followCoroutine;
        private Vector3 _followToLocalPos = Vector3.zero;
        private Vector3 _prevFollowTargetPos = Vector3.zero;

        void OnDrawGizmos() {
            if (VirtualSlider != null) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(GetGlobalHandlerPosition(VirtualSlider.MinValue), GetGlobalHandlerPosition(VirtualSlider.MaxValue));
            }
        }

        void Awake() {
            VirtualSlider.OnChangeActive += OnVirtualSliderChangeActive;
        }

        void Start() {
            VirtualSlider.ValueChanged += OnValueChanged;
            UpdateHandlerPosition();

            if (!VirtualSlider.gameObject.activeInHierarchy) {
                gameObject.SetActive(false);
            }
        }

        void OnValidate() {
            UpdateHandlerPosition();
        }

        void OnDestroy() {
            if (VirtualSlider != null) {
                VirtualSlider.OnChangeActive -= OnVirtualSliderChangeActive;
            }
        }
        private void OnVirtualSliderChangeActive(bool state) {
            gameObject.SetActive(state);
        }

        public void OnValueChanged(float value) {
            UpdateHandlerPosition();
        }

        public void StartHandlerFollow(Transform followTo) {
            _followCoroutine = StartCoroutine(Follow(followTo));
        }

        public void StopHandlerFollow() {
            if (_followCoroutine != null) {
                StopCoroutine(_followCoroutine);
                VirtualSlider.OnEndSliding();
            }
        }

        private void UpdateHandlerPosition() {
            if (VirtualSlider != null) {
                Handler.transform.position = GetGlobalHandlerPosition(VirtualSlider.GetValue());
            }
        }

        private Vector3 GetGlobalHandlerPosition(float value) {
            float normalizedValue = Mathf.InverseLerp(VirtualSlider.MinValue, VirtualSlider.MaxValue, value);
            float x = Mathf.Lerp(0, Width * transform.lossyScale.x, normalizedValue);
            return transform.position + transform.right * x;
        }

        IEnumerator Follow(Transform followTo) {
            _followToLocalPos = transform.InverseTransformPoint(followTo.position);
            while (true) {
                Vector3 newPosition = transform.InverseTransformPoint(followTo.position);
                float sensitive = 1.5f;
                if (Mathf.Abs(newPosition.x - _prevFollowTargetPos.x) * sensitive > Mathf.Abs(newPosition.y - _prevFollowTargetPos.y) + Mathf.Abs(newPosition.z - _prevFollowTargetPos.z)) {
                    _followToLocalPos = transform.InverseTransformPoint(followTo.position);
                }
                _prevFollowTargetPos = newPosition;
                
                float normalizedX;
                if (_followToLocalPos.x < 0) {
                    normalizedX = 0;
                } else if (_followToLocalPos.x > Width) {
                    normalizedX = 1;
                } else {
                    normalizedX = _followToLocalPos.x / Width;
                }
                float newValue = Mathf.Lerp(VirtualSlider.MinValue, VirtualSlider.MaxValue, normalizedX);
                float curValue = VirtualSlider.GetValue();
                float value = Mathf.Lerp(curValue, newValue, Time.unscaledDeltaTime * LerpSpeed);
                VirtualSlider.SetValue(value);
                yield return null;
            }
        }
    }
}
