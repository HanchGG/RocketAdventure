using System.Collections;
using Leap;
using Leap.Unity;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionTrustedHands : MonoBehaviour {

        public static LeapMotionTrustedHands Instance;

        public bool IsLeftPalmDown;
        public bool IsRightPalmDown;
        public bool IsMainPalmDown;
        public bool IsLeftPalmUp;
        public bool IsRightPalmUp;
        public bool IsMainPalmUp;
        [Range(0, 180)]
        public float OnAngle = 45f;
        [Range(0, 180)]
        public float OffAngle = 60f;
        public float Period = 0.064f;

        private Controller _controller;
        private Hand _leftHand;
        private Hand _rightHand;
        private Hand _mainHand;
        private bool _isLeftTracked;
        private bool _isRightTracked;
        private bool _isMainTracked;
        private int _lastFrame;
        private int _leftHandId;
        private int _rightHandId;
        private int _mainHandId;
        private Coroutine _palmDirectionCoroutine;

        private void Awake() {
            Instance = this;
        }

        void Start() {
            _controller = new Controller();
        }

        void OnEnable() {
            _palmDirectionCoroutine = StartCoroutine(PalmDirectionCoroutine());
        }

        void OnDisable() {
            if (_palmDirectionCoroutine != null) {
                StopCoroutine(_palmDirectionCoroutine);
            }
        }

        IEnumerator PalmDirectionCoroutine() {
            while (true) {
                Hand hand = GetLeft();
                float angle;
                if (hand != null) {
                    angle = Vector3.Angle(hand.PalmNormal.ToVector3(), Vector3.down);
                    if (IsLeftPalmDown && angle > OffAngle) {
                        IsLeftPalmDown = false;
                    } else if (!IsLeftPalmDown && angle < OnAngle) {
                        IsLeftPalmDown = true;
                    }
                    if (!IsLeftPalmDown) {
                        angle = Vector3.Angle(hand.PalmNormal.ToVector3(), Vector3.up);
                        if (IsLeftPalmUp && angle > OffAngle) {
                            IsLeftPalmUp = false;
                        } else if (!IsLeftPalmUp && angle < OnAngle) {
                            IsLeftPalmUp = true;
                        }
                    }
                } else if (IsLeftPalmDown || IsLeftPalmUp) {
                    IsLeftPalmDown = false;
                    IsLeftPalmUp = false;
                }

                hand = GetRight();
                if (hand != null) {
                    angle = Vector3.Angle(hand.PalmNormal.ToVector3(), Vector3.down);
                    if (IsRightPalmDown && angle > OffAngle) {
                        IsRightPalmDown = false;
                    } else if (!IsRightPalmDown && angle < OnAngle) {
                        IsRightPalmDown = true;
                    }
                    if (!IsRightPalmDown) {
                        angle = Vector3.Angle(hand.PalmNormal.ToVector3(), Vector3.up);
                        if (IsRightPalmUp && angle > OffAngle) {
                            IsRightPalmUp = false;
                        } else if (!IsRightPalmUp && angle < OnAngle) {
                            IsRightPalmUp = true;
                        }
                    }
                } else if (IsRightPalmDown || IsRightPalmUp) {
                    IsRightPalmDown = false;
                    IsRightPalmUp = false;
                }

                if (_mainHand == _leftHand) {
                    IsMainPalmDown = IsLeftPalmDown;
                    IsMainPalmUp = IsLeftPalmUp;
                } else {
                    IsMainPalmDown = IsRightPalmDown;
                    IsMainPalmUp = IsRightPalmUp;
                }
                yield return new WaitForSecondsRealtime(Period);
            }
        }


        void IdentifyHands() {
            if (_lastFrame == Time.frameCount || _controller == null) {
                return;
            }
            _lastFrame = Time.frameCount;
            Hand leftHand = null;
            Hand rightHand = null;
            foreach (var hand in _controller.GetTransformedFrame(transform.GetLeapMatrix()).Hands) {

                if (hand.IsLeft) {
                    if (leftHand == null) {
                        leftHand = hand;
                    } else {
                        if (leftHand.PalmPosition.z < hand.PalmPosition.z) {
                            leftHand = hand;
                        }
                    }
                } else if (hand.IsRight) {
                    if (rightHand == null) {
                        rightHand = hand;
                    } else {
                        if (rightHand.PalmPosition.z < hand.PalmPosition.z) {
                            rightHand = hand;
                        }
                    }
                }
            }

            _isLeftTracked = leftHand != null;
            if (_isLeftTracked) {
                _leftHand = leftHand;
                _leftHandId = leftHand.Id;
            }

            _isRightTracked = rightHand != null;
            if (_isRightTracked) {
                _rightHand = rightHand;
                _rightHandId = rightHand.Id;
            }

            _isMainTracked = _isLeftTracked || _isRightTracked;
            if (_isMainTracked) {
                if (!_isRightTracked) {
                    _mainHand = _leftHand;
                    _mainHandId = _leftHandId;
                } else {
                    _mainHand = _rightHand;
                    _mainHandId = _rightHandId;
                }
            }
        }

        public Hand GetLeft() {
            IdentifyHands();
            return _leftHand;
        }

        public Hand GetRight() {
            IdentifyHands();
            return _rightHand;
        }

        public Hand GetMain() {
            IdentifyHands();
            return _mainHand;
        }

        public int GetLeftId() {
            IdentifyHands();
            return _leftHandId;
        }

        public int GetRightId() {
            IdentifyHands();
            return _rightHandId;
        }

        public int GetMainId() {
            IdentifyHands();
            return _mainHandId;
        }

        public bool IsLeftTracked() {
            IdentifyHands();
            return _isLeftTracked;
        }

        public bool IsRightTracked() {
            IdentifyHands();
            return _isRightTracked;
        }

        public bool IsMainTracked() {
            IdentifyHands();
            return _isMainTracked;
        }

        public bool IsTrustedHand(HandModel handModel) {
            return IsTrustedHand(handModel.GetLeapHand());
        }

        public bool IsTrustedHand(Hand hand) {
            return (hand.IsLeft && hand.Id == _leftHandId) || (hand.IsRight && hand.Id == _rightHandId);
        }

        public bool IsOnlyIndexExtended() {
            return GetMain() != null && GetMain().IsOnlyIndexExtended();
        }

    }
}
