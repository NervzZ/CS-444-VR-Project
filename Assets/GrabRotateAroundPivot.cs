using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabRotateAroundPivot : MonoBehaviour
{
    [Tooltip("The pivot point around which this object should rotate when grabbed.")]
    public Transform pivotPoint;

    [Tooltip("The local normal of the plane to constrain motion to (e.g., Vector3.right for X-axis).")]
    public Vector3 localPlaneNormal = Vector3.right;

    private Transform _interactorAttachTransform;
    private XRGrabInteractable _grabInteractable;

    private float _grabbedRadius;
    private Vector3 _initialDirectionOnPlane;
    
    private float _planeOffset = 0.05f / 2;

    private void OnEnable()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        _interactorAttachTransform = args.interactorObject.GetAttachTransform(_grabInteractable);

        Vector3 planeNormal = pivotPoint.TransformDirection(localPlaneNormal).normalized;
        Vector3 toStick = transform.position - pivotPoint.position;
        Vector3 projected = Vector3.ProjectOnPlane(toStick, planeNormal);

        _grabbedRadius = projected.magnitude;
        _initialDirectionOnPlane = projected.normalized;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        _interactorAttachTransform = null;
    }

    private void Update()
    {
        if (_interactorAttachTransform == null || pivotPoint == null)
            return;

        Vector3 planeNormal = pivotPoint.TransformDirection(localPlaneNormal).normalized;

        // Vector from pivot to current hand position, projected on plane
        Vector3 toHand = _interactorAttachTransform.position - pivotPoint.position;
        Vector3 projectedHand = Vector3.ProjectOnPlane(toHand, planeNormal).normalized;

        // Calculate angle between initial grab direction and current direction
        Quaternion rotationDelta = Quaternion.FromToRotation(_initialDirectionOnPlane, projectedHand);
        Vector3 finalDirection = rotationDelta * _initialDirectionOnPlane;

        // Set final position using original radius and plane
        Vector3 targetPosition = pivotPoint.position + finalDirection * _grabbedRadius +
                                 planeNormal * (transform.localScale.x / 2);
        transform.position = targetPosition;

        // Optional: stick faces outward from pivot
        transform.rotation = Quaternion.LookRotation(finalDirection, planeNormal);
    }
}
