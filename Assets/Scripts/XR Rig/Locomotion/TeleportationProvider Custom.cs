using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// Based on TeleportationProvider from XR Toolkit
/// Resync player collider after teleporting
/// </summary>
public class CustomTeleportationProvider : TeleportationProvider
{
    private float _delayTime;
    /// Had to add this because parent needs it
    private float _delayStartTime;
    
    protected override void Update()
    {
        if (!validRequest)
            return;

        if (locomotionState == LocomotionState.Idle)
        {
            if (_delayTime > 0f)
            {
                if (TryPrepareLocomotion())
                    _delayStartTime = Time.time;
            }
            else
            {
                TryStartLocomotionImmediately();
            }
        }

        if (locomotionState == LocomotionState.Moving)
        {
            switch (currentRequest.matchOrientation)
            {
                case MatchOrientation.WorldSpaceUp:
                    upTransformation.targetUp = Vector3.up;
                    TryQueueTransformation(upTransformation);
                    break;
                case MatchOrientation.TargetUp:
                    upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                    TryQueueTransformation(upTransformation);
                    break;
                case MatchOrientation.TargetUpAndForward:
                    upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                    TryQueueTransformation(upTransformation);
                    forwardTransformation.targetDirection = currentRequest.destinationRotation * Vector3.forward;
                    TryQueueTransformation(forwardTransformation);
                    break;
                case MatchOrientation.None:
                    // Change nothing. Maintain current origin rotation.
                    break;
            }

            positionTransformation.targetPosition = currentRequest.destinationPosition;
            TryQueueTransformation(positionTransformation);
            
            FixColliderCenter();

            TryEndLocomotion();
            validRequest = false;
        }
    }
    
    private void FixColliderCenter()
    {
        XROrigin xrOrigin = mediator.xrOrigin;
        if (!xrOrigin || !xrOrigin.Camera || !xrOrigin.Origin)
            return;

        CharacterController cc = xrOrigin.Origin.GetComponent<CharacterController>();
        if (!cc)
            return;

        Vector3 localCamPos = xrOrigin.Origin.transform.InverseTransformPoint(xrOrigin.Camera.transform.position);
        cc.center = new Vector3(localCamPos.x, cc.center.y, localCamPos.z);
    }
}