using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractorUI : XRRayInteractor
{
    private XRInteractorLineVisual lineVisual;

    protected override void Start()
    {
        base.Start();
        lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        lineVisual.gameObject.SetActive(true);
        Debug.Log("hover enter");
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        lineVisual.gameObject.SetActive(false);
        Debug.Log("hover exit");
        base.OnHoverExited(args);
    }
}
