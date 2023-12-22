using TMPro;
using UnityEngine;
using System;

namespace Nettle.NettleLeap {

    public class LeapMotionSliderLabel : MonoBehaviour {

        public TextMeshPro Text;
        public LeapMotionSliderController Slider;

        void Reset() {
            EditorInit();
        }

        void OnValidate() {
            EditorInit();
        }

        void EditorInit() {
            if (!Text) {
                Text = GetComponent<TextMeshPro>();
            }
        }

        void Start() {
            Slider.VirtualSlider.ValueChanged += OnValueChanged;
            OnValueChanged(Slider.VirtualSlider.GetValue());
        }


        void OnValueChanged(float value) {
                Text.text = Slider.GetLabelText();            
        }
    }
}
