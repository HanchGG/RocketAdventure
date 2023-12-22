using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Nettle.NettleLeap {

    public class LeapMotionDebugHandsCount : MonoBehaviour {

        public bool IsLeft = true;
        public LeapMotionPostprocessor Postprocessor;
        private Text _text;
        private Controller _controller;

        void OnValidate() {
            if (Postprocessor == null) {
                Postprocessor = FindObjectOfType<LeapMotionPostprocessor>();
            }
        }


        void Awake() {
            _text = GetComponent<Text>();
            _controller = FindObjectOfType<LeapServiceProvider>().GetLeapController();
        }

        void Update() {
            int total = 0;
            foreach (var hand in _controller.Frame().Hands) {
                if (hand.IsLeft == IsLeft) {
                    total++;
                }
            }

            if (IsLeft) {
                _text.text = total.ToString();
            } else {
                _text.text = total.ToString();
            }
        }
    }
}
