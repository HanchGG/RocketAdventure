using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Nettle.NettleLeap {

    public class LeapMotionGUIManager : MonoBehaviour {

        public enum GUIType { NettleBox, NettleDesk, None }
        public enum PressType { Pointer, Finger }

        public float GUIActivationDelay = 0.2f;
        public bool ShowAlways = false;
        
        public UnityEvent OnShowed;
        public UnityEvent OnHided;

        [Header("NettleBox settings")]
        [FormerlySerializedAs("Root3DGUI")]
        public GameObject RootNettleBox;
        [FormerlySerializedAs("GUI3D")]
        public GameObject NettleBoxGUI;
        [FormerlySerializedAs("Joystick3D")]
        public GameObject JoystickNettleBox;

        [Header("NettleDesk settings")]
        public GameObject RootNettleDeskGUI;
        public GameObject GUINettleDesk;
        public GameObject JoystickNettleDesk;

        [Tooltip("In real meters")]
        public float ScreenWidth = 1.4285f;

        [Tooltip("Shift relative BottomCenter of screen. In real meters")]
        public Vector3 ControllerShift = new Vector3(0.0f, 0, 0.015f);

        public StereoEyes StereoEyes;

        [HideInInspector]
        public PressType LeapMotionPressType;


        private GameObject _root;
        private GameObject _gui;
        private GameObject _joystick;
        private bool _isGuiActive;
        private Coroutine _showHideDelayedCoroutine;
        private LeapMotionTrustedHands _trustedHands;
        private LeapMotionPostprocessor _postprocessor;
        private bool _showed = false;
        private MotionParallaxDisplay _display;

        void OnValidate() {
            if (StereoEyes == null) {
                StereoEyes = FindObjectOfType<StereoEyes>();
            }
        }

        void Awake() {
            _trustedHands = FindObjectOfType<LeapMotionTrustedHands>();
            _postprocessor = FindObjectOfType<LeapMotionPostprocessor>();
            _isGuiActive = ShowAlways;
            _display = FindObjectOfType<MotionParallaxDisplay>();
            LeapMotionTwoHandRotation twoHandRotation = FindObjectOfType<LeapMotionTwoHandRotation>();
            Debug.Log("LeapMotionGUIManager::GUIType=" + GetGUIType());
            if (GetGUIType() == GUIType.NettleBox) {
                _root = RootNettleBox;
                _gui = NettleBoxGUI;
                _joystick = JoystickNettleBox;
                LeapMotionPressType = PressType.Finger;
                _postprocessor.EyesRotationEnabled = false;
                if (twoHandRotation != null) {
                    twoHandRotation.enabled = true;
                }
            } else {
                if (RootNettleBox != null) {
                    RootNettleBox.SetActive(false);
                }
                _root = RootNettleDeskGUI;
                _gui = GUINettleDesk;
                _joystick = JoystickNettleDesk;
                LeapMotionPressType = PressType.Finger;
                _postprocessor.EyesRotationEnabled = false;
                if (twoHandRotation != null) {
                    twoHandRotation.enabled = true;
                }
            }

            if (_root != null)
            {
                UpdateRootPosition();
                _gui.SetActive(_isGuiActive);
            }
        }

        public void UpdateRootPosition()
        {

            float screenHeight = (ScreenWidth / 16) * 9;
            float scaleMultiplier = _display.Width / ScreenWidth;
            _root.transform.localPosition = new Vector3(ControllerShift.x, ControllerShift.y, -(screenHeight / 2 + ControllerShift.z)) * scaleMultiplier;
            _root.transform.localScale = transform.localScale * scaleMultiplier;
        }

        void OnEnable() {
            if (_root != null) {
                _root.SetActive(true);
            }
        }

        void OnDisable() {
            if (_root != null) {
                _root.SetActive(false);
            }
            if (_gui != null) {
                _gui.SetActive(false);
            }
            _isGuiActive = false;
        }

        void Update() {
            if (_trustedHands.IsMainTracked() && _trustedHands.IsOnlyIndexExtended() || ShowAlways) {
                if (!_isGuiActive) {
                    _isGuiActive = true;
                    if (_showHideDelayedCoroutine != null) {
                        StopCoroutine(_showHideDelayedCoroutine);
                    }
                    _showHideDelayedCoroutine = StartCoroutine(ShowDelayed());
                }
            } else {
                if (_isGuiActive) {
                    _isGuiActive = false;
                    if (_showHideDelayedCoroutine != null) {
                        StopCoroutine(_showHideDelayedCoroutine);
                    }
                    _showHideDelayedCoroutine = StartCoroutine(HideDelayed());
                }
            }
        }

        public void TogglePressType() {
            if (LeapMotionPressType == PressType.Pointer) {
                LeapMotionPressType = PressType.Finger;
            } else {
                LeapMotionPressType = PressType.Pointer;
            }
        }

        IEnumerator ShowDelayed() {
            yield return new WaitForSecondsRealtime(GUIActivationDelay);
            if (_isGuiActive) {
                _joystick.SetActive(false);
                _gui.SetActive(true);
                if (!_showed) {
                    OnShowed?.Invoke();
                    StartGUIFade(true);
                    _showed = true;
                }
            }
        }

        IEnumerator HideDelayed() {
            yield return new WaitForSecondsRealtime(GUIActivationDelay);
            if (!_isGuiActive) {
                StartGUIFade(false);
                OnHided?.Invoke();
                _showed = false;
                yield return new WaitForSecondsRealtime(LeapMotionUIActiveElement.FadeDuration);
                _gui.SetActive(false);
                _joystick.SetActive(true);
            }
        }

        private void StartGUIFade(bool show) {
            LeapMotionUIActiveElement[] uiElements = _gui.GetComponentsInChildren<LeapMotionUIActiveElement>();
            foreach (LeapMotionUIActiveElement element in uiElements) {
                element.StartFade(show);
            }
        }

        public GUIType GetGUIType() {
            return GUIType.NettleBox;
        }
    }
}
