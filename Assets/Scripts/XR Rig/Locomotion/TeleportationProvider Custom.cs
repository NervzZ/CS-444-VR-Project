using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class CustomTeleportationProvider : TeleportationProvider
{
    float _DelayTime;
    float _DelayStartTime;
    protected virtual void Update()
    {
        if (!validRequest)
            return;

        if (locomotionState == LocomotionState.Idle)
        {
            if (_DelayTime > 0f)
            {
                if (TryPrepareLocomotion())
                    _DelayStartTime = Time.time;
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
                default:
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
        var xrOrigin = mediator.xrOrigin;
        if (xrOrigin == null || xrOrigin.Camera == null || xrOrigin.Origin == null)
            return;

        var cc = xrOrigin.Origin.GetComponent<CharacterController>();
        if (cc == null)
            return;

        var localCamPos = xrOrigin.Origin.transform.InverseTransformPoint(xrOrigin.Camera.transform.position);
        cc.center = new Vector3(localCamPos.x, cc.center.y, localCamPos.z);
    }
}