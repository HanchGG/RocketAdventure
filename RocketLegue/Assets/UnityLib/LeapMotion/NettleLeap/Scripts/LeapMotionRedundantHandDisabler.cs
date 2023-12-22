using Leap.Unity;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionRedundantHandDisabler : MonoBehaviour {

        public Transform HandsRoot;
        private HandModel[] _childs;
        public LeapMotionTrustedHands TrustedHands;

        void Awake() {
            _childs = new HandModel[0];
        }

        void Update() {
            if (_childs.Length < HandsRoot.childCount) {
                _childs = HandsRoot.GetComponentsInChildren<HandModel>(true);
            }
            foreach (var hand in _childs) {
                if (hand.IsTracked) {
                    bool isTrused = TrustedHands.IsTrustedHand(hand);
                    if (!isTrused && hand.gameObject.activeSelf) {
                        hand.gameObject.SetActive(false);
                    } else if (isTrused && !hand.gameObject.activeSelf) {
                        hand.gameObject.SetActive(true);
                    }
                }
            }

        }
    }
}
