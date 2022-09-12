using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_InTraning : RoomState
{
    [Header("Room State")]
    public RoomState_TrainingAnimation roomStateTrainingAnimation;
    [Space(5)]

    [Header("Toasts")]
    public ToastOneButton toastLecture;
    public Toast toastStudent;
    public Toast toastMR;
    private GameObject currentToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        switch (user.userType)
        {
            case UserType.Lecture:
                currentToast = toastLecture.gameObject;
                roomSceneManager.onRoomStateEvent += StartTraining;
                break;
            case UserType.Student:
                if(user.hasExtingisher)
                {
                    currentToast = toastMR.gameObject;
                }
                else
                {
                    currentToast = toastStudent.gameObject;
                }
                break;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {
        currentToast.SetActive(false);
        base.OnStateExit();
    }

    public void StartTraining()
    {
        photonView.RPC(nameof(StartTrainingRPC), RpcTarget.All);
    }

    [PunRPC]
    public void StartTrainingRPC()
    {
        roomSceneManager.RoomState = roomStateTrainingAnimation;
    }
}