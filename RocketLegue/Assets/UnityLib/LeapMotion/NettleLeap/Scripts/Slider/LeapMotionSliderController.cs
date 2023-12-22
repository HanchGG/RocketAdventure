using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nettle.NettleLeap {

    public abstract class LeapMotionSliderController : MonoBehaviour {

        [FormerlySerializedAs("slider")]
        public LeapMotionVirtualSlider VirtualSlider;

        public virtual string GetLabelText() {
            return "";
        }

        protected virtual void Start() {
            VirtualSlider.ValueChanged += OnValueChanged;
        }

        protected virtual void OnValueChanged(float value) {

        }
    }
}
