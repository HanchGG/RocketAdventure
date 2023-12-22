using System.Collections.Generic;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionIcon : MonoBehaviour {

        public List<GameObject> LeapMotionGUIs;
        public Renderer Renderer;

        void Update() {
            foreach (var gui in LeapMotionGUIs) {
                if (gui.activeSelf) {
                    Renderer.enabled = true;
                    return;
                }
            }
            Renderer.enabled = false;
        }
    }
}
