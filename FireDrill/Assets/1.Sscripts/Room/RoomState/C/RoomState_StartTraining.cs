using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_StartTraining : RoomState
{
    [Header("Room State")]
    public RoomState_TrainingPin roomStateTrainingPin;
    [Space(5)]

    [Header("Toast")]
    public ToastOneButton toast;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        switch (user.userType)
        {
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

    public void EventTrigger()
    {
        photonView.RPC(nameof(EventTriggerRPC), RpcTarget.All);
    }

    [PunRPC]
    public void EventTriggerRPC()
    {
        roomSceneManager.RoomState = roomStateTrainingPin;
    }
}
