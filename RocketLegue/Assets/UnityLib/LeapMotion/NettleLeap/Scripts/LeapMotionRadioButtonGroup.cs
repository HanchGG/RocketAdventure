using System.Collections.Generic;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionRadioButtonGroup : MonoBehaviour {

        [HideInInspector]
        public List<LeapMotionVirtualButton> LMButtons;
        public LeapMotionVirtualButton DefaultLeapMotionVirtualButton;
        private LeapMotionVirtualButton _selectedLeapMotionVirtualButton;

        public void Awake() {
            ResetSelectionToDefault();
        }

        public void ResetIfSelected(LeapMotionVirtualButton virtualButton) {
            if (_selectedLeapMotionVirtualButton == virtualButton) {
                ResetSelection();
            }
        }

        public void ResetSelection() {
            if (_selectedLeapMotionVirtualButton != null) {
                _selectedLeapMotionVirtualButton.PressOffRadioButton();
                _selectedLeapMotionVirtualButton = null;
            }
        }

        public void ResetSelectionToDefault()
        {
            ResetSelection();
            _selectedLeapMotionVirtualButton = DefaultLeapMotionVirtualButton;
            if (_selectedLeapMotionVirtualButton != null)
            {
                _selectedLeapMotionVirtualButton.SetToggle(true);
            }
        }

        public void SelectLMButton(LeapMotionVirtualButton leapMotionVirtualButton) {
            if (_selectedLeapMotionVirtualButton == leapMotionVirtualButton) {
                return;
            }

            ResetSelection();
            _selectedLeapMotionVirtualButton = leapMotionVirtualButton;
            _selectedLeapMotionVirtualButton.SetToggle(true);
        }
    }
}
