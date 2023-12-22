using System;
using System.Collections.Generic;
using UnityEngine;


public class LeapMotionPhysicTimeFix : MonoBehaviour {

    [Tooltip("Wait Time.fixedDeltaTime*this before manual update")]
    public float FixedDeltaTimeMultiplier = 2;
    private float _lastManualUpdate;


    protected static LeapMotionPhysicTimeFix Instance;

    protected static List<MonoBehaviour> FixedUpdateListeners = new List<MonoBehaviour>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("Multiple LeapMotionPhysicTimeFix instance");
        }
    }

    public static void AddFixedUpdateListener(MonoBehaviour monoBehaviour) {
        FixedUpdateListeners.Add(monoBehaviour);
    }

    public static void RemoveFixedUpdateListener(MonoBehaviour monoBehaviour) {
        FixedUpdateListeners.Remove(monoBehaviour);
    }

    void Update() {
        /* if (Time.timeScale < 1 / FixedDeltaTimeMultiplier &&
             Time.fixedUnscaledDeltaTime + Time.fixedDeltaTime * FixedDeltaTimeMultiplier < Time.unscaledTime &&
             _lastManualUpdate + Time.fixedDeltaTime * FixedDeltaTimeMultiplier < Time.unscaledTime) {
             Physics.SyncTransforms();
             _lastManualUpdate = Time.unscaledTime; 
         }   */
        if (Time.timeScale == 0 &&
            Time.fixedUnscaledTime + Time.fixedUnscaledDeltaTime < Time.unscaledTime &&
            _lastManualUpdate + Time.fixedUnscaledDeltaTime < Time.unscaledTime) {
            Physics.autoSimulation = false;
            Physics.Simulate(0.00000000001f);
            _lastManualUpdate = Time.unscaledTime;
            for (int i = 0; i < FixedUpdateListeners.Count;) {
                if (FixedUpdateListeners[i] == null) {
                    FixedUpdateListeners.RemoveAt(i);
                    continue;
                }

                FixedUpdateListeners[i].SendMessage("FixedUpdate");
                i++;
            }
        } else {
            if (Time.timeScale > 0) {
                Physics.autoSimulation = true;
            }
        }
    }
}
