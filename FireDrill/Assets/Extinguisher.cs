using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public GameObject pinTrigger;
    public GameObject nozzle;
    public GameObject hose;
    public ParticleSystem hoseWater;

    private Rigidbody rigidBody;

    [SerializeField] private bool isPinOff = false;
    [SerializeField] private bool isNozzleOff = false;

    private void Start()
    {
        pinTrigger.GetComponent<PinTrigger>().onPinRemoved += PinOff;
        rigidBody = GetComponent<Rigidbody>();
    }

    public void OnAttach()
    {
        Collider nozzleCollider = nozzle.GetComponent<Collider>();
        nozzleCollider.enabled = true;
        nozzle.GetComponent<Rigidbody>().isKinematic = false;

        Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
        pinTriggerCollider.enabled = true;
        nozzle.GetComponent<Rigidbody>().isKinematic = false;

        rigidBody.isKinematic = false;
    }

    public void OnDettach()
    {
        if(!isPinOff)
        {
            Collider nozzleCollider = nozzle.GetComponent<Collider>();
            nozzleCollider.enabled = false;
        }
        if (!isNozzleOff)
        {
            Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
            pinTriggerCollider.enabled = false;
        }
    }

    public void PinOff()
    {
        isPinOff = true;
    }

    public void NozzleOff()
    {
        isNozzleOff = true;
    }
}
