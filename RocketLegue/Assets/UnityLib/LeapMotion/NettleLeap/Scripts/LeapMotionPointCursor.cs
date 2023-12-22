using UnityEngine;

namespace Nettle.NettleLeap {

public class LeapMotionPointCursor: MonoBehaviour {
    public float RayLength = 1f;
    public string LayerName = "InfoBorders";
    private Renderer _lastRenderer;
    private int _layer;
    RaycastHit[] hits = new RaycastHit[4];

    void Start() {
        _layer =LayerMask.GetMask(LayerName);
    }

    void Update() {
        Ray ray = new Ray(transform.position - transform.up * 0.5f, transform.up);
        int count = Physics.RaycastNonAlloc(ray, hits, RayLength, _layer);
        if (count > 0) {
            RaycastHit hit = hits[0];
            float minSqrDistance = float.MaxValue;
            for (int i = 0; i < count; i++) {
                float sqrDistance = Vector3.SqrMagnitude(hits[i].transform.position - transform.position);
                if (minSqrDistance > sqrDistance) {
                    hit = hits[i];
                    minSqrDistance = sqrDistance;
                }
            }


            Renderer renderer = hit.transform.GetComponent<Renderer>();
            if (_lastRenderer == null) {
                _lastRenderer = renderer;
                renderer.enabled = true;
            } else if (_lastRenderer != renderer) {
                _lastRenderer.enabled = false;
                _lastRenderer = renderer;
                renderer.enabled = true;
            }

        } else {
            if (_lastRenderer != null) {
                _lastRenderer.enabled = false;
                _lastRenderer = null;
            }
        }


    }
}
}
