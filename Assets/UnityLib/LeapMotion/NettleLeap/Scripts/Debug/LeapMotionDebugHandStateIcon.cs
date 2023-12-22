using Leap;
using Leap.Unity;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionDebugHandStateIcon : MonoBehaviour {

        public bool IsLeft;

        private UnityEngine.UI.Image _image;
        private Controller _controller;
        private LeapMotionPostprocessor _postprocessor;

        void Awake() {
            _image = GetComponent<UnityEngine.UI.Image>();
        }

        private void Start() {
            _controller = FindObjectOfType<LeapServiceProvider>().GetLeapController();
            _postprocessor = FindObjectOfType<LeapMotionPostprocessor>();
        }

        void Update() {
            Color color = Color.gray;
            if (IsLeft) {
                if (_postprocessor.TrustedHands.IsLeftTracked()) {
                    if (_postprocessor.TrustedHands.GetMain().IsLeft) {
                        color = Color.green;
                    } else {
                        color = Color.white;
                    }
                }
            } else {
                if (_postprocessor.TrustedHands.IsRightTracked()) {
                    if (!_postprocessor.TrustedHands.GetMain().IsLeft) {
                        color = Color.green;
                    } else {
                        color = Color.white;
                    }                }
            }

            _image.color = color;
        }
    }
}
