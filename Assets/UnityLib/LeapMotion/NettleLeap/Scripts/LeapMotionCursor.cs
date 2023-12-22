using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Nettle.NettleLeap {

public class LeapMotionCursor: MonoBehaviour {

    public float HoldTime;

    public Transform BaseTransform;
    public Transform CircleTransform;

    public float MoveScale = 3.5f;
    public float CoordinateSystemYOffset = 0.3f;

    public float MoveLerpT = 40f;

    public UnityEvent ClickEvent;

    public Vector2 MinPos = new Vector2(-1f, -0.5625f);
    public Vector2 MaxPos = new Vector2(1f, 0.5625f);

    private bool _showed;
    private LeapMotionButton _leapMotionButton;
    private LeapMotionGUIManager _leapMotionGuiManager;
    private float _currectHoldTime;
    private Coroutine _holdCoroutine;
    private MeshRenderer _circleMeshRenderer;
    private LeapMotionTrustedHands _trustedHands;

    void Awake() {
        _trustedHands = FindObjectOfType<LeapMotionTrustedHands>();
        _leapMotionGuiManager = FindObjectOfType<LeapMotionGUIManager>();
        _circleMeshRenderer = CircleTransform.GetComponent<MeshRenderer>();
    }


    void LateUpdate() {
        if (IsPointerActive()) {
            if (!_showed) {
                Show();
            }

            Vector3 currentPos = new Vector3(LeapMotionPokeFinger.TrustedFinger.localPosition.x * MoveScale, 0, (LeapMotionPokeFinger.TrustedFinger.localPosition.y - CoordinateSystemYOffset) * MoveScale);
            Vector3 newPos = Vector3.Lerp(BaseTransform.localPosition, currentPos, MoveLerpT * Time.unscaledDeltaTime);
            if (newPos.x > MaxPos.x) {
                newPos.x = MaxPos.x;
            } else if (newPos.x < MinPos.x) {
                newPos.x = MinPos.x;
            }
            if (newPos.z > MaxPos.y) {
                newPos.z = MaxPos.y;
            } else if (newPos.z < MinPos.y) {
                newPos.z = MinPos.y;
            }
            BaseTransform.localPosition = newPos;


        } else if (_showed) {
            Hide();
        }
    }

    void Hide() {
        _showed = false;
        BaseTransform.GetComponent<BoxCollider>().enabled = false;
        BaseTransform.GetComponent<MeshRenderer>().enabled = false;
        _circleMeshRenderer.enabled = false;
    }

    void Show() {
        _showed = true;
        BaseTransform.GetComponent<BoxCollider>().enabled = true;
        BaseTransform.GetComponent<MeshRenderer>().enabled = true;
        _circleMeshRenderer.enabled = true;
    }


    bool IsPointerActive() {
        return _leapMotionGuiManager.LeapMotionPressType == LeapMotionGUIManager.PressType.Pointer && _trustedHands.IsOnlyIndexExtended();
    }

    void OnTriggerEnter(Collider collider) {
        if (IsPointerActive()) {
            LeapMotionButton link = collider.GetComponent<LeapMotionButton>();
            if (link != null && !(link.VirtualButton.LeapMotionButtonType == LeapMotionVirtualButton.ButtonType.RadioButton && link.VirtualButton.Toggled)) {
                if (_leapMotionButton != link && _leapMotionButton != null) {
                    ExitFromButton();
                }
                _leapMotionButton = link;
                _leapMotionButton.SetHighlighted(true);
                _holdCoroutine = StartCoroutine(HoldCoroutine());
            }
        }
    }

    void OnTriggerExit(Collider collider) {
        LeapMotionButton link = collider.GetComponent<LeapMotionButton>();
        if (link != null && link == _leapMotionButton) {
            ExitFromButton();
        }
    }

    void OnDisable() {
        if (_leapMotionButton != null) {
            ExitFromButton();
        }
    }

    void ExitFromButton() {
        _currectHoldTime = 0f;
        _circleMeshRenderer.material.SetFloat("_ClipValue", 1);
        _leapMotionButton.SetHighlighted(false);
        _leapMotionButton = null;
        if (_holdCoroutine != null) {
            StopCoroutine(_holdCoroutine);
        }
    }

    IEnumerator HoldCoroutine() {
        while (_currectHoldTime < HoldTime) {
            _currectHoldTime += Time.unscaledDeltaTime;

            float v = 1 - GetPressPercent();
            _circleMeshRenderer.material.SetFloat("_ClipValue", v);
            yield return null;
        }
        _circleMeshRenderer.material.SetFloat("_ClipValue", 1);
        _leapMotionButton.SetHighlighted(false);
        _leapMotionButton.VirtualButton.Press();
        ClickEvent.Invoke();
    }

    public float GetPressPercent() {
        float result = _currectHoldTime / HoldTime;
        return result < 1 ? result : 1;
    }


}
}
