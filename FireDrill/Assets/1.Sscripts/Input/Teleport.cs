using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class Teleport : MonoBehaviour
{
    public XROrigin xrOrigin;
    public GameObject teleportDestination;

    public Vector2 test;

    private void Update()
    {
        if(InputManager.IsLeftHandStickActivated)
        {
            teleportDestination.SetActive(true);

            Vector2 stickAxis = InputManager.LeftHandStickContextValue;
            test = stickAxis;
            teleportDestination.transform.position = xrOrigin.transform.position + xrOrigin.transform.rotation * new Vector3(stickAxis.x, 0, stickAxis.y);
        }
        else
        {
            teleportDestination.SetActive(false);
        }
    }
}
