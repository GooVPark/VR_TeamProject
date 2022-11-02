using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public GameObject pinTrigger;
    public GameObject nozzle;
    public GameObject hose;
    public ParticleSystem hoseWater;

    public GameObject lever;

    private Rigidbody rigidBody;

    [SerializeField] private bool isPinOff = false;
    [SerializeField] private bool isNozzleOff = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        pinTrigger.GetComponent<PinTrigger>().onPinRemoved += PinOff;
        nozzle.GetComponent<Nozzle>().onNozzleDettached += NozzleOff;
    }

    public void OnAttach()
    {
        Debug.Log("OnAttach");

        Collider nozzleCollider = nozzle.GetComponent<Collider>();
        nozzleCollider.enabled = true;
        nozzle.GetComponent<Nozzle>().Activate();
        pinTrigger.GetComponent<PinTrigger>().Activate();
        //nozzle.GetComponent<Rigidbody>().isKinematic = false;

        Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
        pinTriggerCollider.enabled = true;
        //nozzle.GetComponent<Rigidbody>().isKinematic = false;

        rigidBody.isKinematic = false;
    }

    public void OnDettach()
    {
        nozzle.GetComponent<Nozzle>().Deactivate();
        pinTrigger.GetComponent<PinTrigger>().Deactivate();
        //Debug.Log("OnDettach");
        //if (!isPinOff)
        //{
        //    Collider nozzleCollider = nozzle.GetComponent<Collider>();
        //    nozzleCollider.enabled = false;
        //}
        //if (!isNozzleOff)
        //{
        //    Debug.Log("IsNozzleOff");
        //    Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
        //    pinTriggerCollider.enabled = false;
        //}
        rigidBody.isKinematic = false;
    }

    public void PinOff()
    {
        isPinOff = true;
    }

    public void NozzleOff()
    {
        Debug.Log("NozzleOff");
        isNozzleOff = true;
    }

    public void OnSpread()
    {

    }
}
