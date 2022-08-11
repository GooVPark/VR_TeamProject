using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class DynamicRayVisualizer : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;

    [Header("Teleport")]

    public XRRayInteractor teleportInteractor;
    public GameObject teleportPointer;

    private RaycastHit teleportRayHit;
    private bool isTeleportable;

    [Space(5)]

    [Header("UI Interaction")]

    public XRRayInteractor uiInteractor;

    private RaycastHit uiInteractorRayHit;
    private bool isUIDetected = false;

    private void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
        uiInteractor.GetComponent<LineRenderer>().enabled = false;
    }

    private void Update()
    {
        if (InputManager.IsLeftHandStickActivated)
        {
            teleportInteractor.gameObject.SetActive(true);
            isTeleportable = teleportInteractor.TryGetCurrent3DRaycastHit(out teleportRayHit);

            if (isTeleportable)
            {
                teleportPointer.SetActive(true);
                teleportPointer.transform.position = teleportRayHit.point;
            }
            else
            {
                teleportPointer.SetActive(false);
            }
        }
        else
        {
            teleportPointer.SetActive(false);
            teleportInteractor.gameObject.SetActive(false);

            if (isTeleportable)
            {
                var heightAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
                var cameraDestination = teleportPointer.transform.position + heightAdjustment;
                xrOrigin.MoveCameraToWorldLocation(cameraDestination);
            }

            isTeleportable = false;
        }

        isUIDetected = uiInteractor.TryGetCurrent3DRaycastHit(out uiInteractorRayHit);
        
    }

    public void UIRayHoverEnter()
    {


            uiInteractor.GetComponent<LineRenderer>().enabled = true;
            uiInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;

    }

    public void UIRayHoverExit()
    {
        uiInteractor.GetComponent<LineRenderer>().enabled = false;
        uiInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
    }
    
}
