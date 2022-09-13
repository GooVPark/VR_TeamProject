using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_TrainingPin : RoomState
{
    [Header("Room State")]
    public RoomState_TraningControler roomStateTrainingController;
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
                    currentToast = toast.gameObject;
                    roomSceneManager.onRoomStateEvent += EventTrigger;
                    pinTrigger = roomSceneManager.player.pinTrigger;
                    pinTrigger.onPinRemoved += OnPinRemove;
                }
                break;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {
        roomSceneManager.onRoomStateEvent -= EventTrigger;
        currentToast.SetActive(false);
        base.OnStateExit();
    }

    public void EventTrigger()
    {
        if (isPinRemoved)
        {
            photonView.RPC(nameof(EventTriggerRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    public void EventTriggerRPC()
    {
        roomSceneManager.RoomState = roomStateTrainingController;
    }

    public void OnPinRemove()
    {
        isPinRemoved = true;
    }
}
