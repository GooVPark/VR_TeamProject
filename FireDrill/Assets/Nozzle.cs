using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : MonoBehaviour
{
    public delegate void OnNozzleDettached();
    public OnNozzleDettached onNozzleDettached;

    [SerializeField] private bool isUnlinked = false;

    public void OnDeselected()
    {
        if (isUnlinked)
        {
            GetComponent<Rigidbody>().isKinematic = false;
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
