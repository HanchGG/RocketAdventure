#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Nettle.NettleLeap {

    [RequireComponent(typeof(LeapMotionVirtualButton))]
    public class LeapMotionButtonZoneSync : MonoBehaviour {


        [Tooltip("Check active zone name start with Zone or fully coincides")]
        public bool CheckZoneStartWith = false;


        public bool UseAutofill = true;
        public string Zone;
        public LeapMotionVirtualButton Button;

        void OnValidate() {
            EditorInit();
        }

        void Reset() {
            EditorInit();
        }

        void EditorInit() {
            if (!Button) {
                Button = GetComponent<LeapMotionVirtualButton>();
            }
        }

        void OnEnable() {
            EditorInit();
            Sync();
            VisibilityZoneViewer.Instance.ActiveZoneChanged += Sync;
            Button.PressEvent.AddListener(Sync);
        }

        void OnDisable() {
            VisibilityZoneViewer.Instance.ActiveZoneChanged -= Sync;
            Button.PressEvent.RemoveListener(Sync);
        }

        public void Sync() {
            VisibilityZone activeZone = VisibilityZoneViewer.Instance.ActiveZone;
            bool pressed = activeZone != null && (CheckZoneStartWith ? activeZone.name.StartsWith(Zone) : activeZone.name == Zone);
            if (Button.LMRadioButtonGroup == null) {
                Button.SetToggle(pressed);
            } else {
                if (pressed) {
                    Button.LMRadioButtonGroup.SelectLMButton(Button);
                } else {
                    Button.LMRadioButtonGroup.ResetIfSelected(Button);
                }
            }
        }

        public void ShowZone()
        {
            VisibilityZoneViewer.Instance.ShowZone(Zone);
        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LeapMotionButtonZoneSync))]
    public class LeapMotionButtonZoneSyncEditor : Editor {
        public override void OnInspectorGUI() {
            base.DrawDefaultInspector();
            serializedObject.Update();

            LeapMotionButtonZoneSync buttonZoneSync = (LeapMotionButtonZoneSync)target;
            if (buttonZoneSync.UseAutofill) {
                string zoneName = GetZoneName(buttonZoneSync);
                if (zoneName != null) {
                    buttonZoneSync.Zone = zoneName;
                    buttonZoneSync.name = "Btn_" + buttonZoneSync.Zone;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        string GetZoneName(LeapMotionButtonZoneSync buttonZoneSync) {
            LeapMotionVirtualButton button = buttonZoneSync.Button;
            for (int i = 0; i < button.PressEvent.GetPersistentEventCount(); i++) {
                var target = button.PressEvent.GetPersistentTarget(i);
                if (target is VisibilityZone) {
                    return target.name;
                }
                if (target is VisibilityManager) {
                    SerializedObject so = new SerializedObject(button);
                    SerializedProperty pressEventProperty = so.FindProperty("PressEvent.m_PersistentCalls.m_Calls");
                    if (pressEventProperty.arraySize > 0) {
                        return pressEventProperty.GetArrayElementAtIndex(0).FindPropertyRelative("m_Arguments.m_StringArgument").stringValue;
                    }
                }
            }
            return null;
        }
    }
#endif
}
