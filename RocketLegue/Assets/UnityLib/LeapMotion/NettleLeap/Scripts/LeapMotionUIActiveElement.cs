using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nettle.NettleLeap {

    [SelectionBase]
    public class LeapMotionUIActiveElement : MonoBehaviour {
        public const float FadeDuration = 0.2f;
        private const float _colorLerpThreshold = 0.1f;
        private const float _colorChangeSpeed = 8;
        private const bool IS_LOCK_PRESSED_BUTTON_UNTIL_EXIT = true;

        private const float DELAY_AFTER_PRESS = 0.15f;
        public static float DoublePressDelay = 0.4f;
        public static float LastPressTime;
        protected static LeapMotionUIActiveElement _pressedButton;
        protected static LeapMotionUIActiveElement _lastPressedButton;

        //public bool ControlColor = true;
        [Space(5)]
        public Color DefaultColor = Color.white;
        public Color DefaultIconColor = Color.white;
        [Space(5)]
        public Color PressedColor = Color.green;
        public Color PressedIconColor = Color.green;
        [Space(5)]
        public Color HighlightColor = Color.cyan;
        public Color HighlightIconColor = Color.cyan;
        [Space(15)]
        [SerializeField]
        protected Renderer _meshRenderer;
        [FormerlySerializedAs("TextMeshPro")]
        public TextMeshPro TextMesh;
        public SpriteRenderer IconSprite;
        protected bool _pressed;
        protected bool _highlighted;

        private Color _currentBorderColor;
        private Color _currentIconColor;
        private Color _targetBorderColor;
        private Color _targetIconColor;
        private float _currentAlpha = 1;
        private float _targetAlpha = 1;

        private bool _isChangingColor = false;

        private LeapMotionGUIManager _leapMotionGuiManager;

        protected virtual void Reset() {
            EditorInit();
        }

        protected virtual void OnValidate() {
            EditorInit();
        }

        void EditorInit() {
            _meshRenderer = GetComponent<Renderer>();
            if (_meshRenderer.sharedMaterial.HasProperty("_Color")) {
                DefaultColor = _meshRenderer.sharedMaterial.color;
            }
            if (TextMesh == null) {
                TextMesh = GetComponentInChildren<TextMeshPro>();
            }
            if (IconSprite == null) {
                IconSprite = GetComponentInChildren<SpriteRenderer>();
            }            
        }

        protected virtual void Awake() {
            _leapMotionGuiManager = FindObjectOfType<LeapMotionGUIManager>();            
        }

        protected virtual void OnEnable() {
            _currentAlpha = 1;
            _targetAlpha = 1;
            UpdateColor();
        }

        protected virtual void OnDisable() {
            _pressed = false;
            _highlighted = false;
            if (_pressedButton == this) {
                ExitFromButtonCollider();
            }
        }

        public virtual void SetPressed(bool state) {
            _pressed = state;
            UpdateColor();
        }

        public void SetHighlighted(bool state) {
            _highlighted = state;
            UpdateColor();
        }

        protected virtual void UpdateColor() {
            if (_pressed) {
                StartBorderColorChange(PressedColor);
                StartIconColorChange(PressedIconColor);
            } else if (_highlighted) {
                StartBorderColorChange(HighlightColor);
                StartIconColorChange(HighlightIconColor);
            } else {
                StartBorderColorChange(DefaultColor);
                StartIconColorChange(DefaultIconColor);
            }
        }


        protected void StartIconColorChange(Color color) {
            _targetIconColor = color;
            _isChangingColor = true;
        }

        protected void StartBorderColorChange(Color color) {
            _targetBorderColor = color;
            _isChangingColor = true;
        }

        private void Update() {
            if (_isChangingColor) {
                if (Vector4.Distance(_targetBorderColor, _currentBorderColor) > _colorLerpThreshold || Vector4.Distance(_targetIconColor, _currentIconColor) > _colorLerpThreshold || Mathf.Abs(_targetAlpha - _currentAlpha) > _colorLerpThreshold) {
                    if (Mathf.Abs(_targetAlpha - _currentAlpha) > _colorLerpThreshold) {
                        _currentAlpha += Mathf.Sign(_targetAlpha - _currentAlpha) * Time.unscaledDeltaTime / FadeDuration;
                    }
                    SetBorderColor(Color.Lerp(_currentBorderColor, _targetBorderColor, Time.unscaledDeltaTime * _colorChangeSpeed));
                    SetIconColor(Color.Lerp(_currentIconColor, _targetIconColor, Time.unscaledDeltaTime * _colorChangeSpeed));
                } else {
                    _isChangingColor = false;
                    _currentAlpha = _targetAlpha;
                    SetBorderColor(_targetBorderColor);
                    SetIconColor(_targetIconColor);
                }
            }
        }

        private void SetBorderColor(Color color) {
            _currentBorderColor = color;
            _currentBorderColor.a = _currentAlpha;
            _meshRenderer.material.color = _currentBorderColor;
        }

        private void SetIconColor(Color color) {
            _currentIconColor = color;
            _currentIconColor.a = _currentAlpha;
            if (IconSprite) {
                IconSprite.color = _currentIconColor;
            }
            if (TextMesh) {
                TextMesh.color = _currentIconColor;
            }
        }

        public void StartFade(bool show) {
            if (show) {
                _currentAlpha = 0;
                _targetAlpha = 1;
                UpdateColor();
                SetBorderColor(_targetBorderColor);
                SetIconColor(_targetIconColor);
            } else {
                _targetAlpha = 0;
            }
            _isChangingColor = true;
        }

        public virtual void OnTriggerEnter(Collider collider) {
            //Debug.Log("LeapMotionPressType="+ _leapMotionGuiManager.LeapMotionPressType+ "::_pressedVirtualButton==null = "+(_pressedVirtualButton == null)
            // + "::_lastPressTime + DelayAfterPress < Time.unscaledTime = "+ (_lastPressTime + DelayAfterPress < Time.unscaledTime));
            if (_leapMotionGuiManager.LeapMotionPressType == LeapMotionGUIManager.PressType.Finger
                && (!IsLockPressedButtonUntilExit() || _pressedButton == null)
                && LastPressTime + GetDelayAfterPress() <= Time.unscaledTime
                && collider.GetComponent<LeapMotionPokeFinger>())
            {
                if (_pressedButton != null)
                {
                    _pressedButton.ExitFromButtonCollider();
                }
                //Debug.Log("newButton != null =" + (newButton != null) + "::newButton != _lastPressedVirtualButton = " + (newButton != _lastPressedVirtualButton)
                //+ "::_lastPressTime + DoublePressDelay < Time.unscaledTime = " + (_lastPressTime + DoublePressDelay < Time.unscaledTime));
                if (this != _lastPressedButton || LastPressTime + DoublePressDelay < Time.unscaledTime)
                {
                    _pressedButton = this;
                    SetPressed(true);
                }
            }
        }

        public virtual void OnTriggerExit(Collider collider) {
            //Debug.Log("button != null =" + (button != null) + "::_pressedVirtualButton == button = " + (_pressedVirtualButton == button));
            if (_pressedButton == this && collider.GetComponent<LeapMotionPokeFinger>()) {
                ExitFromButtonCollider();
            }
        }

        public virtual void OnCustomTriggerEnter(CustomCollision collision) {
            //Debug.Log("LeapMotionPressType="+ _leapMotionGuiManager.LeapMotionPressType+ "::_pressedVirtualButton==null = "+(_pressedVirtualButton == null)
            // + "::_lastPressTime + DelayAfterPress < Time.unscaledTime = "+ (_lastPressTime + DelayAfterPress < Time.unscaledTime));
            if (_leapMotionGuiManager.LeapMotionPressType == LeapMotionGUIManager.PressType.Finger
                && (!IsLockPressedButtonUntilExit() || _pressedButton == null)
                && LastPressTime + GetDelayAfterPress() <= Time.unscaledTime
                && collision.other.GetComponent<LeapMotionPokeFinger>()) {
                if (_pressedButton != null) {
                    _pressedButton.ExitFromButtonCollider();
                }
                //Debug.Log("newButton != null =" + (newButton != null) + "::newButton != _lastPressedVirtualButton = " + (newButton != _lastPressedVirtualButton)
                //+ "::_lastPressTime + DoublePressDelay < Time.unscaledTime = " + (_lastPressTime + DoublePressDelay < Time.unscaledTime));
                if (this != _lastPressedButton || LastPressTime + DoublePressDelay < Time.unscaledTime) {
                    _pressedButton = this;
                    SetPressed(true);
                }
            }
        }

        public virtual void OnCustomTriggerExit(CustomCollision collision) {
            //Debug.Log("button != null =" + (button != null) + "::_pressedVirtualButton == button = " + (_pressedVirtualButton == button));
            if (_pressedButton == this && collision.other.GetComponent<LeapMotionPokeFinger>()) {
                ExitFromButtonCollider();
            }
        }

        public void ExitFromButtonCollider() {
            SetPressed(false);
            _lastPressedButton = _pressedButton;
            _pressedButton = null;
            LastPressTime = Time.unscaledTime;
        }

        protected virtual float GetDelayAfterPress() {
            return DELAY_AFTER_PRESS;
        }

        protected virtual bool IsLockPressedButtonUntilExit() {
            return IS_LOCK_PRESSED_BUTTON_UNTIL_EXIT;
        }
    }
}
