using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class Teleport : MonoBehaviour
{
    public XROrigin xrOrigin;
    public GameObject teleportDestination;

    public XRRayInteractor interactor;

    public Vector2 test;
    private RaycastHit rayHit;

    private bool isTeletortable = false;

    private void Start()
    {
        interactor.gameObject.SetActive(false);
    }

    private void Update()
    {

        if(InputManager.IsLeftHandStickActivated)
        {
            interactor.gameObject.SetActive(true);
            isTeletortable = interactor.TryGetCurrent3DRaycastHit(out rayHit);

            if (isTeletortable)
            {
                teleportDestination.SetActive(true);
                teleportDestination.transform.position = rayHit.point;
            }
            else
            {
                teleportDestination.SetActive(false);
            }
        }
        else
        {
            teleportDestination.SetActive(false);
            interactor.gameObject.SetActive(false);

            if (isTeletortable)
            {
                var heightAdjustment = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
                var cameraDestination = teleportDestination.transform.position + heightAdjustment;
                xrOrigin.MoveCameraToWorldLocation(cameraDestination);
            }

            isTeletortable = false;
        }
    }
}
