using System;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionPokeFinger : MonoBehaviour {

        private class Snapshot {
            public float Time;
            public Vector3 Position;
        }

        private const float SMOOTH_TIME = 0.2f;

        public HandModel IHandModel;

        [NonSerialized]
        public static Transform TrustedFinger;
        [NonSerialized]
        public static HandModel CurrentHandModel;

        private CapsuleCollider _collider;

        private Collider[] _lastColliders = new Collider[0];

        private LinkedList<Snapshot> _snapshots = new LinkedList<Snapshot>();
        private Vector3 _smoothedPosition;
        private int _lastGetSmoothedPositionFrame = -1;

        void Awake() {
            TrustedFinger = transform;
            CurrentHandModel = IHandModel;
            _collider = GetComponent<CapsuleCollider>();
        }


        void Update() {
            _snapshots.AddLast(
                new Snapshot() {
                    Time = Time.unscaledTime,
                    Position = transform.position
                });
            EvaluateSmooth();

            if (TrustedFinger != transform) { //TODO: Need to optimize. Use event from TrustedHands.
                TrustedFinger = transform;
                CurrentHandModel = IHandModel;
            }
            /*Vector3 fromColliderCenterToEnd = transform.forward * (_collider.height / 2) * transform.lossyScale.x;
            Collider[] colliders = Physics.OverlapCapsule(
                transform.position + fromColliderCenterToEnd, transform.position - fromColliderCenterToEnd, _collider.radius * transform.lossyScale.x
                );
            foreach (var collider in colliders) {
                if (!_lastColliders.Contains(collider)) {
                    LeapMotionUIActiveElement uiElement = collider.GetComponent<LeapMotionUIActiveElement>();
                    if (uiElement) {
                        uiElement.OnTriggerEnter(_collider);
                        Debug.Break();
                    }
                }
            }
            foreach (var collider in _lastColliders) {
                if (!colliders.Contains(collider)) {
                    LeapMotionUIActiveElement uiElement = collider.GetComponent<LeapMotionUIActiveElement>();
                    if (uiElement) {
                        uiElement.OnTriggerExit(_collider);
                    }
                }
            }
            _lastColliders = colliders;*/
        }

        private void EvaluateSmooth() {
            while (_snapshots.First.Value.Time + SMOOTH_TIME < Time.unscaledTime) {
                _snapshots.RemoveFirst();
            }
        }

        public Vector3 SmoothedPosition() {
            if(_lastGetSmoothedPositionFrame != Time.frameCount) {
                _smoothedPosition = Vector3.zero;
                foreach (var snapshot in _snapshots) {

                }
                _lastGetSmoothedPositionFrame = Time.frameCount;
            }
            return _smoothedPosition;
        }


    }
}
