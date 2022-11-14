using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_TrainingPin : RoomState
{
    [Header("Room State")]
    public RoomState_TrainingExtinguish roomStateExtinguish;
    [Space(5)]

    [Header("Toast")]
    public ToastOneButton toast;
    private GameObject currentToast;

    [Header("Extinguisher")]
    public PinTrigger pinTrigger;
    
    private bool isPinRemoved = false;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        switch (user.userType)
        {
            case UserType.Lecture:
                break;
            case UserType.Student:
                if(user.hasExtingisher)
                {
                    toast.gameObject.SetActive(true);
                    roomSceneManager.onRoomStateEvent += EventTrigger;

                    pinTrigger = roomSceneManager.player.pinTrigger;
                    pinTrigger.GetComponent<XROffsetGrabInteractable>().enabled = true;
                    pinTrigger.onPinRemoved += OnPinRemove;
                }
                break;
        }

        //currentToast?.SetActive(true);
    }

    public override void OnStateExit()
    {
        switch (user.userType)
        {
            case UserType.Lecture:
                break;
            case UserType.Student:
                if (user.hasExtingisher)
                {
                    toast.gameObject.SetActive(false);
                    roomSceneManager.onRoomStateEvent -= EventTrigger;
                }
                break;
        }
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if(user.hasExtingisher)
        {
            EventTrigger();
        }
    }

    public void EventTrigger()
    {
        if (isPinRemoved)
        {
            photonView.RPC(nameof(EventTriggerRPC), RpcTarget.All);
            isPinRemoved = false;
        }
    }

    [PunRPC]
    public void EventTriggerRPC()
    {
        roomSceneManager.RoomState = roomStateExtinguish;
    }

    public void OnPinRemove()
    {
        Debug.Log("Pin Removed");
        isPinRemoved = true;
    }
}
