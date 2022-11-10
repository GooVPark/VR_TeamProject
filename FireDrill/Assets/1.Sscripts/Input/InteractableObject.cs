using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableObject : XROffsetGrabInteractable
{
    [SerializeField] private bool hasPose;
    [SerializeField] private int poseIndex;

    private HandAnimationController handAnimationController;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (hasPose && args.interactorObject is XRDirectInteractor)
        {
            handAnimationController = args.interactorObject.transform.GetComponent<HandAnimationController>();
            handAnimationController.SetHandPose(poseIndex);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (hasPose)
        {
            //args.interactorObject.transform.GetComponent<HandAnimationController>().SetHandPose(0);
            if(handAnimationController != null)
            {
                Debug.Log("Initialize Hand Pose");
                handAnimationController.SetHandPose(0);
            }
            handAnimationController = null;
        }
        base.OnSelectExited(args);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Debug.Log("Extinguisher Activate");
        args.interactorObject.transform.GetComponent<HandAnimationController>().SetHandPose(poseIndex + 1);
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        args.interactorObject.transform.GetComponent<HandAnimationController>().SetHandPose(1);
        base.OnDeactivated(args);
        Debug.Log("Extinguisher Deactivate");
    }
}
