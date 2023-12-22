using Leap.Unity;
using LeapInternal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Nettle.NettleLeap {


    public class LeapMotionStarter : MonoBehaviour {

        public UnityEvent ToggleOnEvent;
        public UnityEvent ToggleOffEvent;

        [SerializeField]
        private bool _canBeEnabled = true;
        public bool CanBeEnabled { get => _canBeEnabled; set => _canBeEnabled = value; }
        [SerializeField]
        private bool _blocksQuit = false;
        public bool EnabledOnStart;
        public LeapServiceProvider LeapServiceProvider;
        public MotionParallax3D2 MotionParallax3D2;
        public NettleBoxTracking NettleBoxTracking;
        public StereoEyes StereoEyes;
        public RAMotionParallaxDisplayHost RAMotionParallaxDisplayHost;
        public LeapMotionGUIManager LeapMotionGUIManager;
        public List<GameObject> SetActiveList;
        public float FOV = 60f;

        private LeapMotionDontDestroyableStates _states;
        private bool _started;

        [DllImport("LeapC", EntryPoint = "LeapSetPause")]
        public static extern eLeapRS LeapSetPause(IntPtr handle, bool pause);

        [DllImport("LeapC", EntryPoint = "LeapOpenConnection")]
        public static extern eLeapRS LeapOpenConnection(IntPtr handle);


        [DllImport("LeapC", EntryPoint = "LeapCreateConnection")]
        public static extern eLeapRS LeapCreateConnection(out IntPtr handle);


        [DllImport("LeapC", EntryPoint = "LeapSetPolicyFlags")]
        public static extern eLeapRS SetPolicyFlags(IntPtr hConnection, UInt64 set, UInt64 clear);


        [DllImport("LeapTrackingController.dll")]
        public static extern void SetPause(bool state);

        [DllImport("LeapTrackingController.dll")]
        public static extern bool GetPause();

        void Reset() {
            OnValidate();
        }

        void OnValidate() {

            if (MotionParallax3D2 == null) {
                MotionParallax3D2 = FindObjectOfType<MotionParallax3D2>();
            }
            if (NettleBoxTracking == null) {
                NettleBoxTracking = FindObjectOfType<NettleBoxTracking>();
            }
            if (StereoEyes == null) {
                StereoEyes = FindObjectOfType<StereoEyes>();
            }
            if (LeapMotionGUIManager == null) {
                LeapMotionGUIManager = FindObjectOfType<LeapMotionGUIManager>();
            }
            if (RAMotionParallaxDisplayHost == null) {
                RAMotionParallaxDisplayHost = FindObjectOfType<RAMotionParallaxDisplayHost>();
            }
        }

        void Awake() {
            _states = FindObjectOfType<LeapMotionDontDestroyableStates>();
        }

        void Start() {
            /*if (LeapMotionGUIManager.GetGUIType() != LeapMotionGUIManager.GUIType.NettleBox) {
                SetActiveList.ForEach(v => v.SetActive(false));
                gameObject.SetActive(false);
                return;
            }*/
            if ((_states && _states.LeapMotionActivated) || EnabledOnStart) {
                TurnOn();
            } else {
                TurnOff();
            }
        }

        private void TurnOn() {
            if (!gameObject.activeSelf || !CanBeEnabled) {
                return;
            }
            Debug.Log("LeapMotion::TurnOn");
            _started = true;
            if (_states) {
                _states.LeapMotionActivated = _started;
            }

            SetActiveList.ForEach(v => v.SetActive(true));

            SetPause(false);
            ToggleOnEvent.Invoke();

        }

        private void TurnOff() {
            if (!gameObject.activeSelf) {
                return;
            }
            Debug.Log("LeapMotion::TurnOff");
            _started = false;

            if (_states) {
                _states.LeapMotionActivated = _started;
            }
            SetActiveList.ForEach(v => v.SetActive(false));

            SetPause(true);
            ToggleOffEvent.Invoke();

        }

        public void Toggle() {
            if (_started) {
                TurnOff();
            } else {
                TurnOn();
            }
        }

        public void SetToggle(bool toggle) {
            if (_started && !toggle) {
                TurnOff();
            } else if (!_started && toggle) {
                TurnOn();
            }
        }

        private void OnDestroy()
        {
            SetPause(true);
        }

        private void OnQuitHotkey()
        {
            if (!GetPause())
            {
                SetPause(true);
            }
        }

        public bool ReadyToQuit()
        {
            return !_blocksQuit || !_canBeEnabled || GetPause();
        }
    }
}
