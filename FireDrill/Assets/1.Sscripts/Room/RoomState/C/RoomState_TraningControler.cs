using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_TraningControler : RoomState
{
    [Header("Room State")]
    public RoomState_TrainingExtinguish roomStateTrainingExtinguish;
    [Space(5)]

    [Header("Toast")]
    public ToastOneButton toast;
    public GameObject currentToast;

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
        photonView.RPC(nameof(EventTriggerRPC), RpcTarget.All);
    }

    [PunRPC]
    public void EventTriggerRPC()
    {
        roomSceneManager.RoomState = roomStateTrainingExtinguish;
    }
}
