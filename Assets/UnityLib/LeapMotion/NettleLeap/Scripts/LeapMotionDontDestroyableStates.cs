using System.Collections;
using Leap;
using UnityEngine;

namespace Nettle.NettleLeap {

    public class LeapMotionDontDestroyableStates : MonoBehaviour {

        public bool LeapMotionActivated = false;
        public bool DisableInEditor = true; //Bug workaround. Freezing when Play, if execute new Controller() in previous Play

        private Controller _controller;
        private bool _needReconnect;

        public void Start() {
#if UNITY_EDITOR
            if (DisableInEditor) {
                return;
            }
#endif
            _controller = new Controller();
            StartCoroutine(StartConnection());
            _controller.Device += OnDeviceConnected;
            _controller.Connect += (sender, args) => Debug.Log("LeapMotion::Connect");
            _controller.DeviceFailure +=
                (sender, args) => Debug.Log("LeapMotion::DeviceFailure::ErrorCode=" + args.ErrorCode + "::ErrorMessage=" + args.ErrorMessage);
            _controller.DeviceLost += (sender, args) => Debug.Log("LeapMotion::DeviceLost");
            _controller.Disconnect += (sender, args) => Debug.Log("LeapMotion::Disconnect");
            _controller.LogMessage += (sender, args) => Debug.Log("LeapMotion::LogMessage::message=" + args.message);

        }

        void OnDestroy() {
            if (_controller != null) {
                _controller.Device += OnDeviceConnected;
                _controller.Dispose();
            }
        }


        void OnDeviceConnected(object obj, DeviceEventArgs args) {
            Debug.Log("LeapMotio::DeviceConnected");
            _needReconnect = true;
        }



        IEnumerator StartConnection() {
            while (true) {
                if (_needReconnect) {
                    _controller.StartConnection();
                    _needReconnect = false;
                }
                yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }
}
