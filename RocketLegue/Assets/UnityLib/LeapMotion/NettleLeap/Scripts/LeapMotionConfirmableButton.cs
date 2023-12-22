using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nettle.NettleLeap {
    public class LeapMotionConfirmableButton : LeapMotionButton {

        private const float DELAY_AFTER_PRESS = 0f;
        private const bool IS_LOCK_PRESSED_BUTTON_UNTIL_EXIT = false;

        protected override float GetDelayAfterPress() {
            return DELAY_AFTER_PRESS;
        }

        public virtual void OnTriggerStay(Collider collider) {
            if (_pressedButton == null) {
                OnTriggerEnter(collider);
            }
        }

        protected override bool IsLockPressedButtonUntilExit() {
            return IS_LOCK_PRESSED_BUTTON_UNTIL_EXIT;
        }
    }
}
