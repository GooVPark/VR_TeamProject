using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinTrigger : MonoBehaviour
{
    public delegate void OnPinRemoved();
    public OnPinRemoved onPinRemoved;

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
        if(other.CompareTag("PinCollider"))
        {
            onPinRemoved?.Invoke();
        }
    }
}
