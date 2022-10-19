using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_TraningControler : RoomState
{
    [Header("Room State")]
    public RoomState_TrainingPin roomstateTrainingPin;
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
                    toast.gameObject.SetActive(true);
                    roomSceneManager.onRoomStateEvent += EventTrigger;
                }
                break;
        }
    }

    public override void OnStateExit()
    {
        switch (user.userType)
        {
            case UserType.Lecture:
                break;
            case UserType.Student:
                toast.gameObject.SetActive(false);
                roomSceneManager.onRoomStateEvent -= EventTrigger;

                if(user.hasExtingisher)
                {
                    roomSceneManager.player.SetExtinguisher();
                }

                break;
        }
        base.OnStateExit();
    }

    public void EventTrigger()
    {
        photonView.RPC(nameof(EventTriggerRPC), RpcTarget.All);
    }

    [PunRPC]
    public void EventTriggerRPC()
    {
        roomSceneManager.RoomState = roomstateTrainingPin;
    }
}
