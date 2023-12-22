using Leap.Unity;
using Leap.Unity.Swizzle;
using UnityEngine;

namespace Nettle.NettleLeap {
    public class LeapMotionInteractionZone : MonoBehaviour {

        public Renderer Renderer;
        public LeapMotionTrustedHands LeapMotionTrustedHands;
        public LeapMotionPostprocessor LeapMotionPostprocessor;
        public float StartColorDistance = 0.2f;
        public float MiddleCollorDistance = 0.02f;
        public float EndCollorDistance = 0;
        public Color StartColor = new Color(0, 1, 0, 0);
        public Color MiddleColor = new Color(1, 1, 0, 1);
        public Color EndColor = new Color(1, 0, 0, 1);
        public float AppearentTime = 0.4f;

        private float _appearancePercent = 0;
        private Color _lastColor;

        void OnValidate() {
            EditroInit();
        }

        void Reset() {
            EditroInit();
        }

        void EditroInit() {
            if (!Renderer) {
                Renderer = GetComponentInChildren<Renderer>();
            }

            if (!LeapMotionTrustedHands) {
                LeapMotionTrustedHands = FindObjectOfType<LeapMotionTrustedHands>();
            }

            if (!LeapMotionPostprocessor) {
                LeapMotionPostprocessor = FindObjectOfType<LeapMotionPostprocessor>();
            }
        }

        void Awake() {
            Color color = Renderer.material.color;
            color.a = 0;
            _lastColor = color;
            Renderer.material.color = color;
        }


        void Update() {
            Color resultColor = Renderer.material.color;
            if (LeapMotionPostprocessor.ZoomPanControlledByHand()) {
                _appearancePercent = Mathf.Clamp01(_appearancePercent + Time.unscaledDeltaTime / AppearentTime);
                float distanceFromCenter = LeapMotionTrustedHands.GetMain().PalmPosition.ToVector3().xz().magnitude;
                float distanceToBorder = Renderer.transform.localScale.x - distanceFromCenter;
                if (distanceToBorder > MiddleCollorDistance) {
                    float startToMiddlePercent = Mathf.InverseLerp(StartColorDistance, MiddleCollorDistance, distanceToBorder);
                    resultColor = Color.Lerp(StartColor, MiddleColor, startToMiddlePercent);
                } else {
                    float middleToEndPercent = Mathf.InverseLerp(MiddleCollorDistance, EndCollorDistance, distanceToBorder);
                    resultColor = Color.Lerp(MiddleColor, EndColor, middleToEndPercent);
                }
                _lastColor = resultColor;
            } else {
                resultColor = _lastColor;
                _appearancePercent = Mathf.Clamp01(_appearancePercent - Time.unscaledDeltaTime / AppearentTime);
            }

            resultColor.a = resultColor.a * _appearancePercent;
            Renderer.material.color = resultColor;
            
        }
    }
}