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
        if (isUnlinked)
        {
            transform.SetParent(nozzleOrigin);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
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

    [PunRPC]
    private void OnSelectedRPC()
    {
        transform.SetParent(null);
    }
}
