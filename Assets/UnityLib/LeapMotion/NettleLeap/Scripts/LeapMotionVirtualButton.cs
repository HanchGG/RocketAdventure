using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nettle.NettleLeap
{


    public class LeapMotionVirtualButton : MonoBehaviour
    {
        public enum ButtonType { Button, RadioButton, Toggle }


        public ButtonType LeapMotionButtonType = ButtonType.Button;
        public LeapMotionRadioButtonGroup LMRadioButtonGroup;
        public bool Toggled;

        public UnityEvent PressEvent;
        public UnityEvent PressOffEvent;
        public UnityEvent ToggleOnEvent;
        public UnityEvent ToggleOffEvent;

        public Action<bool> OnChangeActive;
        private bool _settingToggle = false;


        void OnEnable()
        {
            if (OnChangeActive != null)
            {
                OnChangeActive.Invoke(true);
            }      
        }

        void OnDisable()
        {
            if (OnChangeActive != null)
            {
                OnChangeActive.Invoke(false);
            }
        }

        void Awake()
        {
            if (LeapMotionButtonType == ButtonType.RadioButton)
            {
                LMRadioButtonGroup.LMButtons.Add(this);
            }
        }

        public void Press()
        {
            Debug.Log(name + "::Press");
            if (LeapMotionButtonType == ButtonType.Button)
            {
                PressEvent.Invoke();
            }
            else if (LeapMotionButtonType == ButtonType.RadioButton)
            {
                if (!Toggled)
                {
                    LMRadioButtonGroup.SelectLMButton(this);
                    PressEvent.Invoke();
                }
            }
            else if (LeapMotionButtonType == ButtonType.Toggle)
            {
                SetToggle(!Toggled);
                PressEvent.Invoke(); 
                if (!Toggled){
                    PressOffEvent.Invoke();
                }
            }
        }

        public void PressOffRadioButton()
        {
            if (Toggled)
            {
                SetToggle(false);
                PressOffEvent.Invoke();
            }
        }

        public void Toggle()
        {
            SetToggle(!Toggled);
        }

        public void SetToggle(bool toggled)
        {
            if (_settingToggle)
            {
                //Prevent infinite recursion
                return;
            }
            _settingToggle = true;
            //Debug.Log(name + "::SetToggle::"+ toggled);
            if (Toggled != toggled)
            {
                Toggled = toggled;

                if (Toggled)
                {
                    ToggleOnEvent.Invoke();
                }
                else
                {
                    ToggleOffEvent.Invoke();
                }
            }
            _settingToggle = false;
        }

        public void SetOn()
        {
            SetToggle(true);
        }

        public void SetOff()
        {
            SetToggle(false);
        }
    }
}