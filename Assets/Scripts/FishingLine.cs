using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform rodTip;
    public Transform bait;
    
    private LineRenderer _lineRenderer;
    
    void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }
    
    void Update() {
        _lineRenderer.SetPosition(0, rodTip.position);
        _lineRenderer.SetPosition(1, bait.position);
    }
}
