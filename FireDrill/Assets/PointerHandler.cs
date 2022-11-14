using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

public class PointerHandler : MonoBehaviourPun
{
    public GameObject reticle;
    private LineRenderer line;

    public GameObject pointStart;

    public XRRayInteractor interactor;
    private XRInteractorLineVisual lineVisual;

    private UserType userType;
    private Vector3[] points = new Vector3[2];

    private bool isHovered;
    // Start is called before the first frame update
    void Start()
    {
        
        userType = NetworkManager.User.userType;
        line = reticle.GetComponent<LineRenderer>();
        lineVisual = interactor.GetComponent<XRInteractorLineVisual>();
        if (userType == UserType.Student)
        {
            lineVisual.reticle = null;
        }
    }

    private void Update()
    {
        if(userType == UserType.Lecture && isHovered)
        {
            Vector3 start = pointStart.transform.position;
            Vector3 end = lineVisual.reticle.transform.position;
            photonView.RPC(nameof(DrawLineRPC), RpcTarget.All, start, end);
        }
    }

    [PunRPC]
    public void DrawLineRPC(Vector3 pointStart, Vector3 pointEnd)
    {
        reticle.transform.position = pointEnd;

        points[0] = pointStart;
        points[1] = reticle.transform.position;

        line.SetPositions(points);
    }

    public void OnHoverEntered()
    {
        photonView.RPC(nameof(OnHoverEnteredRPC), RpcTarget.All);
    }

    [PunRPC]
    public void OnHoverEnteredRPC()
    {
        isHovered = true;
        reticle.SetActive(true);
        line.enabled = true;
    }

    public void OnHoverExited()
    {
        photonView.RPC(nameof(OnHoverExitedRPC), RpcTarget.All);
    }

    [PunRPC]
    public void OnHoverExitedRPC()
    {
        isHovered = false;
        reticle.SetActive(false);
        line.enabled = false;
    }
}
