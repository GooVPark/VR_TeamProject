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

    public void OnDeselected()
    {
        if (isUnlinked)
        {
            transform.position = nozzleOrigin.position;
            transform.rotation = nozzleOrigin.rotation;
            transform.SetParent(nozzleOrigin);
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
}
