using System;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nettle.NettleLeap {

    public class LeapMotionPostprocessor : MonoBehaviour {
        public enum Gesture { None, Pinch, OnlyIndexExtended, Grap, Joystick }
        public enum ControlTargets { ZoomPan, ObjectRotation }

        public Gesture MainGesture;

        public ControlTargets ControlTarget;

        public bool EyesRotationEnabled = true;
        public bool GrabDragEnabled = true;
        public bool JoystickScrollingEnabled = false;
        public bool RotateByCheckpoints = true;
        public bool IgnoreDragOrientationCheck = false;
        public Vector3 MoveOrientation = Vector3.one;

        public Camera Camera;
        public Transform Eyes;
        public ZoomPan ZoomPan;
        public HandPool HandPool;
        public LeapMotionTrustedHands TrustedHands;
        public float GrabFistStrength = 0.8f;
        [FormerlySerializedAs("DragMultiplier")]
        public float ScrollSpeed = 1f;
        [FormerlySerializedAs("DragAdditiveMultiplier")]
        public float AddScrollSpeedDependOnZoom = 1f;
        public Vector3 RotateMultiplier = Vector3.one;
        [FormerlySerializedAs("ZoomMultiplier")]
        public float ZoomSpeed = 1f;
        [FormerlySerializedAs("ZoomAdditiveMultiplier")]
        public float AddZoomSpeedDependOnZoom = 1f;

        public float MovePercentPerSec = 10f;
        public AnimationCurve InertionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float InertionTime = 2f;
        public float AntiShakeSpeed = 0.004f;
        public float MovingLockAfterPressBtn = 3f;
        public Vector2 JoystickScrollSpeed = new Vector2(0.03f, 0.3f);
        public Vector2 JoystickZoomSpeed = new Vector2(0.04f, 0.4f);
        public float JoystickXZMinRange = 0.04f;
        public float JoystickXMaxRange = 0.2f;
        public float JoystickZMaxRange = 0.12f;
        public float JoystickYMinRange = 0.03f;
        public float JoystickYMaxRange = 0.22f;
        public float DelayBeforeJoystickDisable = 0.2f;
        public Action<Vector3, float> UpdateJoystick;

        public float ObjectRotationSpeed = 500;
        public float RotationLerpAccelerationPerSec = 4f;
        public float RotationLerpSpeedPerSec = 10f;
        public Vector3 MinRotation = new Vector3(60, -180, 0);
        public Vector3 MaxRotation = new Vector3(90, 180, 0);
        public float MinRotationCheckpointStep = 1f;
        public float RotateByCheckpointTime = 1f;
        public bool RotateByCheckpointCycledX = true;
        public bool RotateByCheckpointCycledY = false;
        public AnimationCurve SpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float[] XRotationCheckpoints;
        public float[] YRotationCheckpoints;

        public int CurrentRotationCheckpointX;
        public int CurrentRotationCheckpointY;
        private Vector3 _startDragPosition;
        private Vector3 _moveVector;
        private Vector3 _rotation;
        private Vector3 _endCheckpointRotation;
        private float _currentRotateByCheckpointTime;
        private Vector3 _oldRotateByCheckpoint;
        private float _timeLeftToJoystickDisable;
        private float _lastPostProcessTime;

        private float _curMoveLerp;
        private float _curRotationLerp;
        private int _mainHandId;
        private Vector3 _curRotation;
        private Vector3 _startInertion = Vector3.zero;
        private Vector3 _lastMoveVector;
        private float _startInertionTime;

        void OnValidate() {
            if (ZoomPan == null) {
                ZoomPan = SceneUtils.FindObjectIfSingle<ZoomPan>();
            }
            if (Camera == null) {
                Camera = FindObjectOfType<Camera>();
            }
        }

        void Awake() {
            _currentRotateByCheckpointTime = RotateByCheckpointTime;
            _lastPostProcessTime = Time.unscaledTime;
            if (ZoomPan == null) {
                ZoomPan = SceneUtils.FindObjectIfSingle<ZoomPan>();
            }
        }


        void OnEnable() {
            if (Eyes != null) {
                _curRotation = Eyes.localEulerAngles;
                Rotate(new Vector3(YRotationCheckpoints[CurrentRotationCheckpointY], XRotationCheckpoints[CurrentRotationCheckpointX], _curRotation.z) - _curRotation);
            }
            MainGesture = Gesture.None;
            _moveVector = Vector3.zero;
        }

        public void RotationMode()
        {
            ControlTarget = ControlTargets.ObjectRotation;
        }

        public void PanMode()
        {
            ControlTarget = ControlTargets.ZoomPan;
        }



        void Update() {
            Vector3 move;
            float moveSpeed = _moveVector.magnitude;
            if (Time.unscaledTime > MovingLockAfterPressBtn + LeapMotionUIActiveElement.LastPressTime
                && (moveSpeed > 0f || _startInertion != Vector3.zero)) {

                bool controlledByHand = _moveVector != _lastMoveVector;

                if (controlledByHand) {
                    _startInertionTime = 0f;
                    _startInertion = _moveVector;
                }

                if (moveSpeed > 0f) {
                    move = _startInertion * MovePercentPerSec * Time.unscaledDeltaTime;
                    if (_moveVector.sqrMagnitude > move.sqrMagnitude) {
                        _moveVector -= move;
                    } else {
                        _moveVector = Vector3.zero;
                    }
                    _lastMoveVector = _moveVector;
                } else {
                    if (_startInertionTime == 0f) {
                        _startInertionTime = Time.unscaledTime;
                    }
                    float kLepr = Mathf.Clamp01(InertionCurve.Evaluate((Time.unscaledTime - _startInertionTime) / InertionTime));
                    if (kLepr < 1) {
                        move = Vector3.Lerp(_startInertion, Vector3.zero, kLepr) * MovePercentPerSec * Time.unscaledDeltaTime;
                    } else {
                        _startInertion = Vector3.zero;
                        move = Vector3.zero;
                    }
                }

                float antiShakeMultiplier = Mathf.InverseLerp(-AntiShakeSpeed / 2, AntiShakeSpeed, move.magnitude);
                move *= antiShakeMultiplier;


                /*if (TrustedHands.GetLeft() != null && IsGrapGesture(TrustedHands.GetLeft())
                    && TrustedHands.GetRight() != null && IsGrapGesture(TrustedHands.GetRight())) {
                    ControlTarget = ControlTargets.ObjectRotation;
                } else {
                    ControlTarget = ControlTargets.ZoomPan;
                }*/

                switch (ControlTarget) {
                    case ControlTargets.ZoomPan:
                        MoveZoomPan(move);
                        ZoomZoomPan(move.y);
                        break;
                    case ControlTargets.ObjectRotation:
                        RotateObject(move);
                        ZoomZoomPan(move.y);
                        break;
                }
            } else {
                if (_curMoveLerp != 0f) {
                    _curMoveLerp = 0f;
                }
                _moveVector = Vector3.zero;
                _lastMoveVector = Vector3.zero;
            }

            if (RotateByCheckpoints) {
                if (_currentRotateByCheckpointTime < RotateByCheckpointTime) {
                    float kLepr = Mathf.Clamp01(SpeedCurve.Evaluate(_currentRotateByCheckpointTime / RotateByCheckpointTime));
                    _currentRotateByCheckpointTime += Time.unscaledDeltaTime;
                    if (_currentRotateByCheckpointTime >= RotateByCheckpointTime) {
                        kLepr = 1;
                    }
                    Vector3 rotate = Vector3.Lerp(Vector3.zero, _endCheckpointRotation, kLepr);
                    Rotate(rotate - _oldRotateByCheckpoint);
                    _oldRotateByCheckpoint = rotate;
                    _rotation = Vector3.zero;
                } else {
                    if (Mathf.Abs(_rotation.x) > MinRotationCheckpointStep) {
                        int newCheckpointId = PrepareRotation(XRotationCheckpoints, CurrentRotationCheckpointX, _rotation.x > 0, RotateByCheckpointCycledX);
                        if (newCheckpointId != -1) {
                            CurrentRotationCheckpointX = newCheckpointId;
                            BeginRotateByCheckpoint(new Vector3(_curRotation.x, XRotationCheckpoints[CurrentRotationCheckpointX], _curRotation.z));
                        }
                        _rotation = Vector3.zero;
                    } else if (Mathf.Abs(_rotation.y) > MinRotationCheckpointStep) {
                        int newCheckpointId = PrepareRotation(YRotationCheckpoints, CurrentRotationCheckpointY, _rotation.y > 0, RotateByCheckpointCycledY);
                        if (newCheckpointId != -1) {
                            CurrentRotationCheckpointY = newCheckpointId;
                            BeginRotateByCheckpoint(new Vector3(YRotationCheckpoints[CurrentRotationCheckpointY], _curRotation.y, _curRotation.z));
                        }
                        _rotation = Vector3.zero;
                    }
                }
            } else {
                if (_rotation.sqrMagnitude > 0f) {
                    if (_curRotationLerp < 1) {
                        _curRotationLerp = Mathf.Clamp01(_curRotationLerp + RotationLerpAccelerationPerSec * Time.unscaledDeltaTime);
                    }
                    Vector3 rotate = Vector3.Lerp(Vector3.zero, _rotation, RotationLerpSpeedPerSec * _curRotationLerp * Time.unscaledDeltaTime);

                    if (_rotation.sqrMagnitude > rotate.sqrMagnitude) {
                        _rotation -= rotate;
                    } else {
                        rotate = _rotation;
                        _rotation = Vector3.zero;
                    }
                    rotate = new Vector3(rotate.x * RotateMultiplier.x, rotate.y * RotateMultiplier.y, rotate.z * RotateMultiplier.z);
                    float t = rotate.x;
                    rotate.x = rotate.y;
                    rotate.y = t;
                    Rotate(rotate);
                } else {
                    if (_curRotationLerp != 0f) {
                        _curRotationLerp = 0f;
                    }
                }
            }
        }

        private void MoveZoomPan(Vector3 move) {
            float zoomInversedLerp = Mathf.InverseLerp(ZoomPan.MaxZoom, ZoomPan.MinZoom, ZoomPan.CurrentZoom);
            float dragMultiplier = ScrollSpeed + zoomInversedLerp * AddScrollSpeedDependOnZoom;
            Vector2 moveV2 = new Vector2(move.x * dragMultiplier * MoveOrientation.x, move.z * dragMultiplier * MoveOrientation.z);
            if (EyesRotationEnabled) {
                moveV2 = Quaternion.Euler(0, 0, -XRotationCheckpoints[CurrentRotationCheckpointX]) * moveV2;
            }
            ZoomPan.Move(moveV2.x, moveV2.y);
        }

        private void ZoomZoomPan(float value) {
            float zoomInversedLerp = Mathf.InverseLerp(ZoomPan.MaxZoom, ZoomPan.MinZoom, ZoomPan.CurrentZoom);
            float zoom = value * (ZoomSpeed + zoomInversedLerp * AddZoomSpeedDependOnZoom) * MoveOrientation.y;
            ZoomPan.DoZoom(zoom);
        }

        private void RotateObject(Vector3 move) {
            //placeholder
        }

        private int PrepareRotation(float[] checkpoints, int currenntCheckpoint, bool rotateLeft, bool cycled) {
            int result = -1;
            if (rotateLeft && (cycled || currenntCheckpoint < checkpoints.Length - 1)) {
                currenntCheckpoint++;
                result = currenntCheckpoint % checkpoints.Length;
            } else if (!rotateLeft && (cycled || currenntCheckpoint > 0)) {
                currenntCheckpoint--;
                result = currenntCheckpoint % checkpoints.Length;
                if (currenntCheckpoint < 0) {
                    result = checkpoints.Length + currenntCheckpoint;
                }
            }
            return result;
        }

        private void BeginRotateByCheckpoint(Vector3 rotate) {
            _oldRotateByCheckpoint = Vector3.zero;
            _endCheckpointRotation = rotate - _curRotation;
            _endCheckpointRotation = CycledDegree(_endCheckpointRotation);
            _currentRotateByCheckpointTime = 0;
        }



        private void Rotate(Vector3 rotate) {
            _curRotation += rotate;
            _curRotation = CycledDegree(_curRotation);

            _curRotation.x = Mathf.Clamp(_curRotation.x, MinRotation.x, MaxRotation.x);
            _curRotation.y = Mathf.Clamp(_curRotation.y, MinRotation.y, MaxRotation.y);
            _curRotation.z = Mathf.Clamp(_curRotation.z, MinRotation.z, MaxRotation.z);
            Eyes.localEulerAngles = _curRotation;
            float a = 90 - Camera.fieldOfView / 2;
            float b = 90 - _curRotation.x;
            float zShift = Mathf.Sin(b * Mathf.Deg2Rad) / (Mathf.Sin(a * Mathf.Deg2Rad) * Mathf.Sin((b + a) * Mathf.Deg2Rad));
            Eyes.localPosition = new Vector3(-Mathf.Sin(_curRotation.y * Mathf.Deg2Rad) * zShift, Eyes.localPosition.y,
                -Mathf.Cos(_curRotation.y * Mathf.Deg2Rad) * zShift);
        }

        private Vector3 CycledDegree(Vector3 rotation) {
            if (rotation.x > 180) {
                rotation.x = rotation.x - 360;
            } else if (rotation.x < -180) {
                rotation.x = rotation.x + 360;
            }

            if (rotation.y > 180) {
                rotation.y = rotation.y - 360;
            } else if (rotation.y < -180) {
                rotation.y = rotation.y + 360;
            }

            if (rotation.z > 180) {
                rotation.z = rotation.z - 360;
            } else if (rotation.x < -180) {
                rotation.z = rotation.z + 360;
            }
            return rotation;
        }


        public void OnPostprocess(Hand hand) {

            if (hand.Id == TrustedHands.GetMainId()) {
                if (_mainHandId != hand.Id) {
                    _mainHandId = hand.Id;
                    MainGesture = Gesture.None;
                } else if ((MainGesture == Gesture.None || MainGesture == Gesture.OnlyIndexExtended)
                    && (TrustedHands.IsMainPalmDown || TrustedHands.IsMainPalmUp) && TrustedHands.GetMain().IsOnlyIndexExtended()) {
                    MainGesture = Gesture.OnlyIndexExtended;
                } else if (GrabDragEnabled && (MainGesture == Gesture.None || MainGesture == Gesture.Grap) && IsGrapGesture(hand)) {
                    if (MainGesture != Gesture.Grap) {
                        MainGesture = Gesture.Grap;
                        _startDragPosition = hand.PalmPosition.ToVector3();
                    } else {
                        Vector3 curPos = hand.PalmPosition.ToVector3();
                        Vector3 delta = _startDragPosition - curPos;
                        _startDragPosition = curPos;
                        float someMultiplier = 1;
                        if (new Vector2(delta.x, delta.z).sqrMagnitude * someMultiplier * someMultiplier > delta.y * delta.y) {
                            _moveVector += new Vector3(delta.x, 0f, delta.z);
                        } else {
                            _moveVector += new Vector3(0f, delta.y, 0f);
                        }
                    }
                } else if (JoystickScrollingEnabled && (MainGesture == Gesture.None || MainGesture == Gesture.Joystick)
                    && (CheckJoystickRequirements(hand) || _timeLeftToJoystickDisable > 0)) {
                    float deltaTime = Time.unscaledTime - _lastPostProcessTime;

                    if (CheckJoystickRequirements(hand)) {
                        _timeLeftToJoystickDisable = DelayBeforeJoystickDisable;
                    } else {
                        _timeLeftToJoystickDisable -= deltaTime;
                    }
                    if (MainGesture != Gesture.Joystick) {
                        MainGesture = Gesture.Joystick;
                        _startDragPosition = hand.PalmPosition.ToVector3();
                    } else {
                        Vector3 curPos = hand.PalmPosition.ToVector3();
                        Vector3 delta = curPos - _startDragPosition;
                        float xzSqrMagnitude = new Vector2(delta.x, delta.z).sqrMagnitude;
                        float ySqrMagnitude = delta.y * delta.y;
                        if (xzSqrMagnitude > JoystickXZMinRange * JoystickXZMinRange ||
                            ySqrMagnitude > JoystickYMinRange * JoystickYMinRange) {
                            float directionMagnitude;
                            float power;
                            float moveDistance;
                            Vector3 normalizedDirection;
                            if (xzSqrMagnitude > ySqrMagnitude) {
                                float powerX = Mathf.Sign(delta.x) * Mathf.InverseLerp(JoystickXZMinRange, JoystickXMaxRange, Mathf.Abs(delta.x));
                                float powerZ = Mathf.Sign(delta.z) * Mathf.InverseLerp(JoystickXZMinRange, JoystickZMaxRange, Mathf.Abs(delta.z));
                                Vector3 direction = new Vector3(powerX, 0f, powerZ);
                                power = direction.magnitude;
                                moveDistance = deltaTime * Mathf.Lerp(JoystickScrollSpeed.x, JoystickScrollSpeed.y, power);
                                if (power != 0) {
                                    normalizedDirection = direction / power;
                                } else {
                                    normalizedDirection = Vector3.zero;
                                }
                            } else {
                                Vector3 direction = new Vector3(0f, delta.y, 0f);
                                directionMagnitude = direction.magnitude;
                                power = Mathf.InverseLerp(JoystickYMinRange, JoystickYMaxRange, directionMagnitude);
                                moveDistance = deltaTime * Mathf.Lerp(JoystickZoomSpeed.x, JoystickZoomSpeed.y, power);
                                if (directionMagnitude != 0) {
                                    normalizedDirection = direction / directionMagnitude;
                                } else {
                                    normalizedDirection = Vector3.zero;
                                }
                            }
                            _moveVector += normalizedDirection * moveDistance;
                            if (UpdateJoystick != null) {
                                UpdateJoystick.Invoke(normalizedDirection, power);
                            }
                        } else {
                            if (UpdateJoystick != null) {
                                UpdateJoystick.Invoke(Vector3.zero, 0f);
                            }
                        }
                    }
                } else {
                    MainGesture = Gesture.None;
                }

                _lastPostProcessTime = Time.unscaledTime;
            }
        }

        public bool IsGrapGesture(Hand hand) {
            if (!IgnoreDragOrientationCheck) { // palmOrientationCheck;
                if (hand.IsLeft) {
                    if (!TrustedHands.IsLeftPalmDown && !TrustedHands.IsLeftPalmUp) {
                        return false;
                    }
                } else {
                    if (!TrustedHands.IsRightPalmDown && !TrustedHands.IsRightPalmUp) {
                        return false;
                    }
                }
            }

            return hand.GrabStrength > 0.99f && hand.GetFistStrength() > GrabFistStrength;
        }

        public bool CheckJoystickRequirements(Hand hand) {
            return hand.GrabStrength > 0.99f && hand.GetFistStrength() > GrabFistStrength;
        }

        public void RotateLeft() {
            _rotation = new Vector3(-MinRotationCheckpointStep - 1f, 0, 0);
        }

        public void RotateRight() {
            _rotation = new Vector3(MinRotationCheckpointStep + 1f, 0, 0);
        }

        public void SetControlType(int typeId) {
            if (typeId == 0) {
                ScrollSpeed = 1f;
                AddScrollSpeedDependOnZoom = 0.8f;
                GrabDragEnabled = true;
                JoystickScrollingEnabled = false;
            } else if (typeId == 1) {
                ScrollSpeed = 1;
                AddScrollSpeedDependOnZoom = 0.8f;
                GrabDragEnabled = false;
                JoystickScrollingEnabled = true;
            }
        }

        public bool ZoomPanControlledByHand() {
            return MainGesture == Gesture.Grap || MainGesture == Gesture.Joystick;
        }

        private bool _switchSpeedState = true;
        public void SwitchSpeed() {
            _switchSpeedState = !_switchSpeedState;
            if (_switchSpeedState) {
                ScrollSpeed = 1;
                AddScrollSpeedDependOnZoom = 0.8f;
            } else {
                ScrollSpeed = 1f;
                AddScrollSpeedDependOnZoom = 0.8f;
            }
        }
    }
}
