using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableObject : XROffsetGrabInteractable
{
    [SerializeField] private bool hasPose;
    [SerializeField] private int poseIndex;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (hasPose && args.interactorObject is XRDirectInteractor)
        {
            args.interactorObject.transform.GetComponent<HandAnimationController>().SetHandPose(poseIndex);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (hasPose)
        {
            args.interactorObject.transform.GetComponent<HandAnimationController>().SetHandPose(0);
        }
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
    }
}
