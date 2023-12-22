using UnityEngine;

namespace Nettle.NettleLeap {

public class LeapMotionCursorSwitcher: MonoBehaviour {

    public MeshRenderer MeshRenderer;
    private Object[] _cursors;
    private int _currentCursor;

    void OnValidate() {
        if (MeshRenderer == null) {
            MeshRenderer = FindObjectOfType<LeapMotionCursor>().GetComponent<MeshRenderer>();
        }
    }

    void Awake() {
        _cursors = Resources.LoadAll("Cursors/");
    }

    void Update () {
        if (Input.GetKeyUp(KeyCode.KeypadPlus)) {
            _currentCursor = Mathf.Clamp(++_currentCursor, 0, _cursors.Length - 1);
            MeshRenderer.materials[0].mainTexture = _cursors[_currentCursor] as Texture2D;
        } else if (Input.GetKeyUp(KeyCode.KeypadMinus)) {
            _currentCursor = Mathf.Clamp(--_currentCursor, 0, _cursors.Length);
            MeshRenderer.materials[0].mainTexture = _cursors[_currentCursor] as Texture2D;
        }
    }
}
}
