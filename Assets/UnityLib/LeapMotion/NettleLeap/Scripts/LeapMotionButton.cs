using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionButton : LeapMotionUIActiveElement {
        public LeapMotionVirtualButton VirtualButton;
        [Space(5)]
        public Sprite ToggledSprite;
        public Color ToggledColor = new Color(1f, 0.5f, 0f);
        public Color ToggledIconColor = new Color(1f, 0.5f, 0f);

        [Header("3D button config")] public bool ActionAfterAnimation = false;
        public bool Press3DAnimationEnabled = false;
        public bool Toggle3DAnimationEnabled = false;
        public float PressTime = 0.4f;
        public float PressedScalePercent = 0.5f;

        private bool _toggled;
        private float _curPressTime;
        private Coroutine _coroutine;
        private Vector3 _defaultPos;
        private float _defaultScaleY;
        private Sprite _defaultSprite;

        protected override void OnValidate() {
            base.OnValidate();
            EditorInit();
        }

        protected override void Reset() {
            base.Reset();
            EditorInit();
        }

        void EditorInit() {
            if (!VirtualButton) {
                VirtualButton = FindObjectsOfType<LeapMotionVirtualButton>().FirstOrDefault(v => v.name == name);                
            }
        }


        protected override void Awake() {
            base.Awake();
            _defaultPos = transform.localPosition;
            _defaultScaleY = transform.localScale.y;

            VirtualButton.ToggleOnEvent.AddListener(() => SetToggled(true));
            VirtualButton.ToggleOffEvent.AddListener(() => SetToggled(false));
            _toggled = VirtualButton.Toggled;

            UpdateColor();
            if (VirtualButton.gameObject != gameObject)
            {
                VirtualButton.OnChangeActive += OnVirtualButtonChangeActive;
                gameObject.SetActive(VirtualButton.gameObject.activeInHierarchy);
            }
        }

        protected override void OnEnable() {
            _toggled = VirtualButton.Toggled;
            base.OnEnable();
            UpdateSprite();
            UpdateSize();
        }

        void OnDestroy() {
            if (VirtualButton != null) {
                VirtualButton.OnChangeActive -= OnVirtualButtonChangeActive;
            }
        }

        public void SetDefaultSprite(Sprite sprite) {
            _defaultSprite = sprite;
            UpdateSprite();
            UpdateColor();
        }

        public void SetToggledSprite(Sprite sprite) {
            ToggledSprite = sprite;
            UpdateSprite();
            UpdateColor();
        }

        public override void SetPressed(bool state) {
            base.SetPressed(state);
            if (_pressed) {
                if (Press3DAnimationEnabled && gameObject.activeInHierarchy) {
                    if (_coroutine != null) {
                        StopCoroutine(_coroutine);
                    }
                    _coroutine = StartCoroutine(PressCoroutine());
                }
                if (!ActionAfterAnimation) {
                    VirtualButton.Press();
                } else if (Toggle3DAnimationEnabled) {
                    if (VirtualButton.LeapMotionButtonType == LeapMotionVirtualButton.ButtonType.Toggle) {
                        SetToggled(!VirtualButton.Toggled, true);
                    } else if (VirtualButton.LeapMotionButtonType == LeapMotionVirtualButton.ButtonType.RadioButton) {
                        if (!VirtualButton.Toggled) {
                            VirtualButton.LMRadioButtonGroup.ResetSelection();
                            SetToggled(true, true);
                        }
                    }
                }
            }
        }

        public void SetToggled(bool state, bool actionAfterAnimation = false) {
            if (_toggled != state) {
                _toggled = state;
                if (Toggle3DAnimationEnabled && gameObject.activeInHierarchy) {
                    if (_coroutine != null) {
                        StopCoroutine(_coroutine);
                    }
                    _coroutine = StartCoroutine(ToggleCoroutine(actionAfterAnimation));
                }
            }
            UpdateSprite();
            UpdateColor();
        }

        void UpdateSprite() {
            if (!IconSprite) {
                return;
            }

            if (_defaultSprite == null) {
                _defaultSprite = IconSprite.sprite;
            }

            if (IconSprite != null) {
                if (_toggled) {
                    IconSprite.sprite = ToggledSprite != null ? ToggledSprite : _defaultSprite;
                } else {
                    IconSprite.sprite = _defaultSprite;
                }
            }
        }

        protected override void UpdateColor() {
            if (_toggled && !_pressed && !_highlighted) {
                StartBorderColorChange(ToggledColor);
                StartIconColorChange(ToggledIconColor);
            } else {
                base.UpdateColor();
            }
        }


        private void OnVirtualButtonChangeActive(bool state) {            
            gameObject.SetActive(state);
        }

        private void UpdateSize() {
            if (_toggled && Toggle3DAnimationEnabled) {
                SetSize(_defaultScaleY * PressedScalePercent);
            } else {
                ResetTransform();
            }
        }

        private void SetSize(float size)
        {
            float delta = (size - transform.localScale.y) / 2;
            transform.localScale = new Vector3(transform.localScale.x, size, transform.localScale.z);
            //transform.localPosition += delta * transform.parent.InverseTransformVector(transform.up).normalized;
        }

        private void ResetTransform() {
            transform.localScale = new Vector3(transform.localScale.x, _defaultScaleY, transform.localScale.z);
            //transform.localPosition = _defaultPos;
        }

        private void PressAnimationStep(float fromScale, float toScale, float pressTime) {
            float lerpResult = Mathf.Lerp(fromScale, toScale, (1 / pressTime) * _curPressTime);
            _curPressTime += Time.unscaledDeltaTime;
            SetSize(lerpResult);
        }

        IEnumerator PressCoroutine() {
            _curPressTime = 0f;
            float pressTime = PressTime / 2;
            float fromScale = _defaultScaleY;
            float toScale = _defaultScaleY * PressedScalePercent;
            while (_curPressTime < pressTime) {
                PressAnimationStep(fromScale, toScale, pressTime);
                yield return null;
            }

            _curPressTime = 0f;
            fromScale = _defaultScaleY * PressedScalePercent;
            toScale = _defaultScaleY;
            while (_curPressTime < pressTime) {
                PressAnimationStep(fromScale, toScale, pressTime);
                yield return null;
            }
            UpdateSize();
            if (ActionAfterAnimation) {
                VirtualButton.Press();
            }
        }

        IEnumerator ToggleCoroutine(bool actionAfterAnimation = false) {
            _curPressTime = 0f;
            float fromScale;
            float toScale;
            if (_toggled) {
                fromScale = _defaultScaleY;
                toScale = _defaultScaleY * PressedScalePercent;
            } else {
                fromScale = _defaultScaleY * PressedScalePercent;
                toScale = _defaultScaleY;
            }

            if (Math.Abs(toScale - transform.localScale.y) > 0.0001f) {
                while (_curPressTime < PressTime) {
                    PressAnimationStep(fromScale, toScale, PressTime);
                    yield return null;
                }
                UpdateSize();
            }
            if (actionAfterAnimation) {
                VirtualButton.Press();
            }
        }
    }
}