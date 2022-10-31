using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public GameObject pinTrigger;
    public GameObject nozzle;
    public GameObject hose;
    public ParticleSystem hoseWater;

    [SerializeField] private bool isPinOff = false;
    [SerializeField] private bool isNozzleOff = false;

    private void Start()
    {
        pinTrigger.GetComponent<PinTrigger>().onPinRemoved += PinOff;
    }

    public void OnAttach()
    {
        Collider nozzleCollider = nozzle.GetComponent<Collider>();
        nozzleCollider.enabled = true;

        Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
        pinTriggerCollider.enabled = true;
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
