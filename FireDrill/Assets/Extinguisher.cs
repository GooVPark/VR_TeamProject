using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Extinguisher : MonoBehaviourPun
{
    public GameObject pinTrigger;
    public GameObject nozzle;
    public Transform extinguisherOrigin;
    public GameObject hose;
    public ParticleSystem hoseWater;

    public GameObject lever;

    private Rigidbody rigidBody;
    private Animator animator;

    [SerializeField] private bool isPinOff = false;
    [SerializeField] private bool isNozzleOff = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        
        Collider pinTriggerCollider = pinTrigger.GetComponent<Collider>();
        pinTriggerCollider.enabled = true;
    }

    public void OnDettach()
    {
        nozzle.GetComponent<Nozzle>().Deactivate();
        pinTrigger.GetComponent<PinTrigger>().Deactivate();

        transform.position = extinguisherOrigin.position;
        transform.rotation = extinguisherOrigin.rotation;
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

    public void Activate()
    {
        animator.SetInteger("Pose", 4);
    }

    public void Deactivate()
    {
        animator.SetInteger("Pose", 1);
    }

}
