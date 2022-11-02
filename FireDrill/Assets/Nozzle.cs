using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Nozzle : MonoBehaviourPun
{
    public delegate void OnNozzleDettached();
    public OnNozzleDettached onNozzleDettached;

    [SerializeField] private bool isUnlinked = false;

    public void OnDeselected()
    {
        if (isUnlinked)
        {
            SetKinematic(false);
        }
    }
    public void Activate()
    {
        GetComponent<XROffsetGrabInteractable>().enabled = true;
    }

    public void Deactivate()
    {
        GetComponent<XROffsetGrabInteractable>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NozzleCollider"))
        {
            isUnlinked = true;
            onNozzleDettached?.Invoke();
        }
    }

    public void SetKinematic(bool value)
    {
        photonView.RPC(nameof(SetKinemaicRPC), RpcTarget.All, value);
    }

    [PunRPC]
    private void SetKinemaicRPC(bool value)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
