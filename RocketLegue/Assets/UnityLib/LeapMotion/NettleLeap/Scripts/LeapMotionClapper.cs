using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Leap;
using Leap.Unity;

namespace Nettle.NettleLeap {

public class LeapMotionClapper: MonoBehaviour {


    public float CoroutineInterval = .032f; //seconds
    public float ClapInterval = 1f;
    public float MaxFistStrength = 0.75f;
    public float Proximity = 0.1f; //meters
    public float VelocityThreshold = 0.8f; //meters/s
    public float PalmAngleLimit = 75; //degrees
    public LeapMotionTrustedHands TrustedHands;

    public UnityEvent OnClap;


    private LeapProvider _provider;
    private float _lastClap;
    private int _clapGestureInRow;

    void Start() {
        _provider = GetComponentInParent<LeapServiceProvider>();
    }

    void OnEnable() {
        StartCoroutine(clapWatcher());
    }

    void OnDisable() {
        StopCoroutine(clapWatcher());
    }

    IEnumerator clapWatcher() {
        while (true) {
            if (_provider) {
                Frame frame = _provider.CurrentFrame;
                if (frame != null && frame.Hands.Count >= 2) {
                    Hand thisHand = TrustedHands.GetLeft();
                    Hand thatHand = TrustedHands.GetRight();
                    if (thisHand != null && thatHand != null) {
                        Vector otherhandDirection = (thisHand.PalmPosition - thatHand.PalmPosition).Normalized;
                        /*Debug.Log((thisHand.GetFistStrength() < MaxFistStrength)+"::"+(thatHand.GetFistStrength() < MaxFistStrength) + "::" +
                            (thisHand.PalmVelocity.MagnitudeSquared + thatHand.PalmVelocity.MagnitudeSquared > VelocityThreshold) + "::" +
                            (thisHand.PalmPosition.DistanceTo(thatHand.PalmPosition) < Proximity) + "::"+(thisHand.PalmPosition.DistanceTo(thatHand.PalmPosition)) + "::" +
                            (thisHand.PalmVelocity.Normalized.AngleTo(otherhandDirection) >= (180 - PalmAngleLimit) * Constants.DEG_TO_RAD) + "::" + 
                            (thatHand.PalmVelocity.Normalized.AngleTo(otherhandDirection) <= PalmAngleLimit * Constants.DEG_TO_RAD) + "::" +
                            (_lastClap + ClapInterval < Time.unscaledTime));*/
                        if (thisHand.GetFistStrength() < MaxFistStrength && thatHand.GetFistStrength() < MaxFistStrength &&
                            thisHand.PalmVelocity.MagnitudeSquared + thatHand.PalmVelocity.MagnitudeSquared > VelocityThreshold && //went fast enough
                            thisHand.PalmPosition.DistanceTo(thatHand.PalmPosition) < Proximity && // and got close 
                            thisHand.PalmVelocity.Normalized.AngleTo(otherhandDirection) >= (180 - PalmAngleLimit) * Constants.DEG_TO_RAD &&
                            thatHand.PalmVelocity.Normalized.AngleTo(otherhandDirection) <= PalmAngleLimit * Constants.DEG_TO_RAD &&
                            _lastClap + ClapInterval < Time.unscaledTime) {
                            OnClap.Invoke();
                            _lastClap = Time.unscaledTime;

                        }
                    }
                }
            }
            yield return new WaitForSecondsRealtime(CoroutineInterval);
        }
    }

}
}
