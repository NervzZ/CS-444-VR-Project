using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FishingRodCaster : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference castAction; // Reference to trigger action (Press & Release)
    public XRGrabInteractable rodInteractable;
    public Collider rodCollider;

    [Header("Transforms")]
    public Transform controllerTransform;  // The hand/controller transform
    public Transform rodTipTransform;
    public Transform baitTransform;
    public Transform baitInitPosTransform;
    public Transform baitParentTransform;
    
    [Header("Others")]
    public LineRenderer lineRenderer;
    public Rigidbody baitRb;

    [Header("Casting Settings")]
    public float castForceMultiplier = 1.5f;

    private bool _isHeld;
    private bool _isHolding;
    private Vector3 _previousPos;
    private Vector3 _handVelocity;
    
    private System.Action<InputAction.CallbackContext> _onCastStarted;
    private System.Action<InputAction.CallbackContext> _onCastCanceled;

    private void Start()
    {
        baitRb.isKinematic = true;

        _previousPos = controllerTransform.position;
        
        baitTransform.SetParent(transform);
        baitTransform.position = baitInitPosTransform.position;
    }

    private void OnEnable()
    {
        // Store callbacks so we can unsubscribe later
        _onCastStarted = ctx => StartHolding();
        _onCastCanceled = ctx => ReleaseAndCast();
        
        castAction.action.started += _onCastStarted;
        castAction.action.canceled += _onCastCanceled;
        
        rodInteractable.selectEntered.AddListener(OnGrabbed);
        rodInteractable.selectExited.AddListener(OnReleased);

        castAction.action.Enable();
    }

    private void OnDisable()
    {
        castAction.action.started -= _onCastStarted;
        castAction.action.canceled -= _onCastCanceled;
        
        rodInteractable.selectEntered.RemoveListener(OnGrabbed);
        rodInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void Update()
    {
        UpdateHandVelocity();
        UpdateLine();
    }
    
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _isHeld = true;
        // Disables collider which avoids unintentional grab with the other hand
        rodCollider.enabled = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _isHeld = false;
        // Disables collider which avoids unintentional grab with the other hand
        rodCollider.enabled = true;
    }

    private void UpdateHandVelocity()
    {
        Vector3 current = controllerTransform.position;
        _handVelocity = (current - _previousPos) / Time.deltaTime;
        _previousPos = current;
    }

    private void StartHolding()
    {
        if (!_isHeld) return;
        
        _isHolding = true;
        
        baitRb.isKinematic = true;
        baitRb.linearVelocity = Vector3.zero;
        
        baitTransform.SetParent(transform);
        baitTransform.position = baitInitPosTransform.position;
    }

    private void ReleaseAndCast()
    {
        if (!_isHeld) return;
        if (!_isHolding) return;
        
        _isHolding = false;
        
        baitTransform.SetParent(baitParentTransform);
        baitTransform.position = baitInitPosTransform.position;
        
        baitRb.isKinematic = false;
        baitRb.linearVelocity = _handVelocity * castForceMultiplier;
    }

    private void UpdateLine()
    {
        if (!lineRenderer) return;
        
        lineRenderer.SetPosition(0, rodTipTransform.position);
        lineRenderer.SetPosition(1, baitTransform.position);
    }
}
