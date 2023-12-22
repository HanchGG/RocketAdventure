using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nettle.NettleLeap {


    public class LeapMotionVirtualSlider : MonoBehaviour {

        [System.Serializable]
        public class UnityIntEvent : UnityEvent<int> {
        }

        public UnityEvent UpdateEvent;
        public UnityIntEvent ValueChangedInt;
        public Action<float> ValueChanged;
        public Action<float> EndSliding;

        public Action<bool> OnChangeActive;


        public float MinValue = 0;
        public float MaxValue = 1;
        [SerializeField]
        private float _value;
        [HideInInspector]
        [SerializeField]
        private int _intValue;



        void OnEnable() {
            if (OnChangeActive != null) {
                OnChangeActive.Invoke(true);
            }
        }

        void OnDisable() {
            if (OnChangeActive != null) {
                OnChangeActive.Invoke(false);
            }
        }

        void OnValidate() {
            _value = GetValidValue(_value);
            _intValue = Mathf.RoundToInt(_value);

        }

        float GetValidValue(float value) {
            if (value < MinValue) {
                return MinValue;
            }
            if (value > MaxValue) {
                return MaxValue;
            }
            return value;
        }

        public void SetValue(int value) {
            if (_intValue  != value) {
                SetValue((float)value);
            }
        }

        public void SetValue(float value) {
            _value = GetValidValue(value);
            if (ValueChanged != null) {
                ValueChanged.Invoke(_value);
            }
            int newInt = Mathf.RoundToInt(_value);
            if (newInt != _intValue) {
                _intValue = newInt;
                if (ValueChangedInt != null) {
                    ValueChangedInt.Invoke(_intValue);
                }
            }
        }

        public void SetLength(int length) {
            MaxValue = MinValue + length - 1;

        }

        public float GetValue() {
            return _value;
        }

        public void OnEndSliding() {
            if (EndSliding != null) {
                EndSliding.Invoke(_value);
            }
        }


    }
}
