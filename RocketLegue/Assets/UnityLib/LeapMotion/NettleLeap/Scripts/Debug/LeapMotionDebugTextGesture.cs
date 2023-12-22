using UnityEngine;
using UnityEngine.UI;

namespace Nettle.NettleLeap {

    public class LeapMotionDebugTextGesture : MonoBehaviour {
        
        public LeapMotionPostprocessor Postprocessor;
        private Text _text;

        void OnValidate() {
            if (Postprocessor == null) {
                Postprocessor = FindObjectOfType<LeapMotionPostprocessor>();
            }
        }


        void Awake() {
            _text = GetComponent<Text>();
        }

        void Update() {
            _text.text = Postprocessor.MainGesture.ToString();
        }
    }
}
