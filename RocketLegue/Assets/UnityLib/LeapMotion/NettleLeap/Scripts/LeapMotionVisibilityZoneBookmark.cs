using UnityEngine;

namespace Nettle.NettleLeap {

public class LeapMotionVisibilityZoneBookmark : MonoBehaviour {

    public VisibilityZoneViewer VisibilityZoneViewer;

    private VisibilityZone _visibilityZoneBookmark;

    void Reset() {
        EditorInit();
    }

    void OnValidate() {
        EditorInit();
    }

    void EditorInit() {
        if (!VisibilityZoneViewer) {
            VisibilityZoneViewer = FindObjectOfType<VisibilityZoneViewer>();
        }
    }

    public void RememberCurrentVisibilityZone() {
        _visibilityZoneBookmark = VisibilityZoneViewer.ActiveZone;
    }

    public void ShowVisibilityZoneBookmark() {
        VisibilityZoneViewer.ShowZone(_visibilityZoneBookmark.name);
    }
}
}
