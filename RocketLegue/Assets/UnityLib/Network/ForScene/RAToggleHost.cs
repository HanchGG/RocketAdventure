using System;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;

namespace Nettle {

    public class RAToggleHost : RemoteActionsUser {

        [SerializeField]
        [Tooltip("Set this in editor to match the initial state of the toggle.")]
        private bool state = false;
        public bool CurrentState {
            get {
                return state;
            }
        }
        public UnityEvent OnToggleOn;
        public UnityEvent OnToggleOff;
        public UnityEvent NeedUpdateEvent;
        private bool needSend = true;
        private NetworkConnection currentSenderConnection = null;


        protected override void OnAddLink(RALink _raLink) {
            if (netBehaviour == NetBehaviour.SERVER) {
                (_raLink as RABytesLink).CmdSetBytesEvent += ToggleCmd;
            } else if (netBehaviour == NetBehaviour.CLIENT) {
                (_raLink as RABytesLink).RpcSetBytesEvent += ToggleRpc;
            }
        }

        public bool GetCurrentState() {
            return state;
        }

        public void UpdateToggleState(bool on) {
            ToggleLocal(on);
        }

        public void SwitchState() {
            ToggleLocal(!state);
        }

        protected override void Synchronize() {
            if (netBehaviour == NetBehaviour.SERVER) {
                if (NeedUpdateEvent != null) {
                    NeedUpdateEvent.Invoke();
                }
                SendState(state, null);
            }
        }

        private void ToggleCmd(byte[] bytes, NetworkConnection senderConnection) {
            state = BitConverter.ToInt32(bytes, 0) != -1;
            currentSenderConnection = senderConnection;
            InvokeEvents(state);
            SendState(state, senderConnection);
        }

        private void ToggleRpc(byte[] bytes) {
            if (bytes.Length == 0)
            {
                return;
            }            
            state = BitConverter.ToInt32(bytes, 0) != -1;
            InvokeEvents(state);
        }

        private void ToggleLocal(bool on) {
            state = on;
            if (needSend) {
                SendState(state, currentSenderConnection);
            }
        }

        private void InvokeEvents(bool on) {
            needSend = false;
            if (on && OnToggleOn != null) {
                OnToggleOn.Invoke();
            } else if (!on && OnToggleOff != null) {
                OnToggleOff.Invoke();
            }
            needSend = true;
        }

        private void SendState(bool on, NetworkConnection exceptConnection) {
            SendBytes(BitConverter.GetBytes(on ? 1 : -1), exceptConnection);
            currentSenderConnection = null;
        }
    }
}
