using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Leap;
using Leap.Unity;

namespace Nettle.NettleLeap {

public class LeapMotionTwoHandRotation: MonoBehaviour {
    public float CoroutineInterval = .032f; //seconds
    public float MaxFistStrength = 0.5f;
    public float MinAngle = 45; //degrees
    public float Cooldown = 0.5f;

    public UnityEvent RotateLeftEvent;
    public UnityEvent RotateRightEvent;

    private Vector3 _startVector = Vector2.zero;
    private float _cooldownStartTime;

    [SerializeField]
    [HideInInspector]
    private LeapMotionTrustedHands _trustedHands;

    void OnValidate() {
        _trustedHands = FindObjectOfType<LeapMotionTrustedHands>();
    }

    void OnEnable() {
        StartCoroutine(rotateWatcher());
    }

    void OnDisable() {
        StopCoroutine(rotateWatcher());
    }

    IEnumerator rotateWatcher() {
        while (true) {
            Hand left;
            Hand right;
            if (Time.unscaledTime - _cooldownStartTime > Cooldown &&
                (right = _trustedHands.GetRight()) != null && (left = _trustedHands.GetLeft()) != null &&
                _trustedHands.IsRightTracked() && _trustedHands.IsLeftTracked() &&
                _trustedHands.IsLeftPalmDown && _trustedHands.IsRightPalmDown &&
                left.GetFistStrength() < MaxFistStrength && right.GetFistStrength() < MaxFistStrength &&
                left.AllFingersExtended() && right.AllFingersExtended()) {

                Vector3 currentVector = right.PalmPosition.ToVector3() - left.PalmPosition.ToVector3();
                currentVector.y = 0;
                if (_startVector == Vector3.zero) {
                    _startVector = currentVector;
                } else {
                    float angle = Vector3.Angle(_startVector, currentVector);
                    if (angle > MinAngle) {
                        float direction = Vector3.Cross(_startVector.normalized, currentVector.normalized).y;
                        if (direction < 0) {
                            if (RotateRightEvent != null) {
                                RotateRightEvent.Invoke();
                            }
                        } else {
                            if (RotateLeftEvent != null) {
                                RotateLeftEvent.Invoke();
                            }
                        }
                        _startVector = Vector3.zero;
                        _cooldownStartTime = Time.unscaledTime;
                    }
                }
            } else if (_startVector != Vector3.zero) {
                _startVector = Vector3.zero;
            }
            yield return new WaitForSecondsRealtime(CoroutineInterval);
        }
    }

}
}
