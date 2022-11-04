using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Nozzle : MonoBehaviourPun
{
    public delegate void OnNozzleDettached();
    public OnNozzleDettached onNozzleDettached;
    public Transform nozzleOrigin;

    [SerializeField] private bool isUnlinked = false;
    public void OnSelected()
    {
        photonView.RPC(nameof(OnSelectedRPC), RpcTarget.All);
    }

    public void OnDeselected()
    {
        photonView.RPC(nameof(OnDeselectedRPC), RpcTarget.All);
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

    [PunRPC]
    private void OnSelectedRPC()
    {
        transform.SetParent(null);
    }

    [PunRPC]
    private void OnDeselectedRPC()
    {
        transform.SetParent(nozzleOrigin);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
