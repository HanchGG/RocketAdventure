using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionUIJoystick : MonoBehaviour {

        [Serializable]
        public class JoystickPlane {
            public Renderer Center;
            public List<Arrow> Arrows = new List<Arrow>();
            public bool RenderersEnabled = false;
        }

        [Serializable]
        public class Arrow {
            public Vector3 Direction = Vector3.zero;
            public List<Renderer> Elements = new List<Renderer>();
        }


        public LeapMotionTrustedHands TrustedHands;
        public LeapMotionPostprocessor Postprocessor;

        public Vector3 Forward = Vector3.left;
        public Color DefaultColor = new Color(0.8f, 0.8f, 0.8f, 0.25f);
        public Color ActiveMinColor = new Color(0f, 1f, 0f, 0.25f);
        public Color ActiveMaxColor = new Color(0f, 1f, 0f, 1f);

        public float FadeTime = 0.2f;

        public JoystickPlane HorizontalPlane;
        public JoystickPlane VerticalPlane;

        [HideInInspector]
        public bool ForceHide;

        private bool _isJoystickActive = false;
        private float _fadeAlphaMultiplier;

        private Color _defaultColor = new Color(0.8f, 0.8f, 0.8f, 0.25f);
        private Color _activeMinColor = new Color(0f, 1f, 0f, 0.25f);
        private Color _activeMaxColor = new Color(0f, 1f, 0f, 1f);
        private float _fadeStartTime;
        private Coroutine _fadeCoroutine;

        private Vector3 _lastDirection;
        private float _lastPower;

        void Awake() {
            SetEnableRenderers(HorizontalPlane, false, true);
            SetEnableRenderers(VerticalPlane, false, true);
        }

        void Start() {
            Postprocessor.UpdateJoystick += OnUpdateJoystick;
        }

        void OnEnable() {
            _fadeAlphaMultiplier = 0f;
            OnToggleJoystick(false);
        }

        void Update() {
            bool isGestureJoystick = TrustedHands.IsMainTracked() && Postprocessor.MainGesture == LeapMotionPostprocessor.Gesture.Joystick;
            if (!_isJoystickActive && isGestureJoystick) {
                OnToggleJoystick(true);
            } else if (_isJoystickActive && !isGestureJoystick) {
                OnToggleJoystick(false);
            }
        }

        IEnumerator ShowHideCoroutine() {
            float startFadeAlphaMultiplier;
            float targetFadeAlphaMultiplier;
            if (_isJoystickActive) {
                startFadeAlphaMultiplier = _fadeAlphaMultiplier;
                targetFadeAlphaMultiplier = 1;
            } else {
                startFadeAlphaMultiplier = _fadeAlphaMultiplier;
                targetFadeAlphaMultiplier = 0;
            }
            _fadeStartTime = Time.unscaledTime;
            do {
                _fadeAlphaMultiplier = Mathf.Lerp(startFadeAlphaMultiplier, targetFadeAlphaMultiplier, (Time.unscaledTime - _fadeStartTime) / FadeTime);
                _defaultColor.a = DefaultColor.a * _fadeAlphaMultiplier;
                _activeMinColor.a = ActiveMinColor.a * _fadeAlphaMultiplier;
                _activeMaxColor.a = ActiveMaxColor.a * _fadeAlphaMultiplier;
                if (!_isJoystickActive) {
                    //Force update joystick
                    OnUpdateJoystick(_lastDirection, _lastPower);
                }
                yield return null;
            } while (Math.Abs(_fadeAlphaMultiplier - targetFadeAlphaMultiplier) > 0.001f);
            if (!_isJoystickActive) {
                SetEnableRenderers(HorizontalPlane, false, true);
                SetEnableRenderers(VerticalPlane, false, true);
            }
            _fadeCoroutine = null;
        }

        void OnUpdateJoystick(Vector3 direction, float power) {
            _lastDirection = direction;
            _lastPower = power;
            JoystickPlane targetPlane;
            if (direction.y == 0f && (!VerticalPlane.RenderersEnabled || direction.x != 0 || direction.z != 0)) {
                targetPlane = HorizontalPlane;
                SetEnableRenderers(VerticalPlane, false);
            } else {
                targetPlane = VerticalPlane;
                SetEnableRenderers(HorizontalPlane, false);
            }
            SetEnableRenderers(targetPlane, true);
            targetPlane.Center.material.color = power > 0.999f ? _defaultColor : Color.Lerp(_activeMaxColor, _activeMinColor, power);
            foreach (var arrow in targetPlane.Arrows) {
                float angle = Vector3.Angle(direction, arrow.Direction);
                if (angle < 90f) {
                    for (int i = 0; i < arrow.Elements.Count; i++) {
                        float t = Mathf.InverseLerp(i / (float)arrow.Elements.Count, (i + 1) / (float)arrow.Elements.Count,
                            power * ((90f - angle) / 90f));
                        Color color = t < 0.001f ? _defaultColor : Color.Lerp(_activeMinColor, _activeMaxColor, t);
                        arrow.Elements[i].material.color = color;
                    }
                } else {
                    for (int i = 0; i < arrow.Elements.Count; i++) {
                        arrow.Elements[i].material.color = _defaultColor;
                    }
                }
            }
        }

        void OnToggleJoystick(bool toggled) {
            _isJoystickActive = toggled;
            if (toggled) {
                SetPlaneColor(HorizontalPlane, _defaultColor);
                SetEnableRenderers(VerticalPlane, false, true);
            }
            if (gameObject.activeInHierarchy) {
                if (_fadeCoroutine != null) {
                    StopCoroutine(_fadeCoroutine);
                }
                _fadeCoroutine = StartCoroutine(ShowHideCoroutine());
            }
        }

        void SetPlaneColor(JoystickPlane plane, Color color) {
            SetEnableRenderers(plane, true);
            plane.Center.material.color = color;
            plane.Arrows.ForEach(arrow => arrow.Elements.ForEach(element => {
                element.material.color = color;
            }));
        }

        void SetEnableRenderers(JoystickPlane plane, bool state, bool force = false) {
            if (plane.RenderersEnabled != state || force) {
                plane.Arrows.ForEach(arrow => arrow.Elements.ForEach(element => element.enabled = state));
                plane.Center.enabled = state;
                plane.RenderersEnabled = state;
            }
        }
    }
}
