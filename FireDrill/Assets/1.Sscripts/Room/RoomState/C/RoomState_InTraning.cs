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
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventArea;
    public EventAreaMR eventAreaMR;
    [Space(5)]

    [Header("NPC")]
    public GameObject npcObject;
    public Animator npcAnimator;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        eventArea.gameObject.SetActive(false);
        eventAreaMR.gameObject.SetActive(false);

        npcAnimator.SetInteger("AnimationState", 2);

        switch (user.userType)
        {
            case UserType.Lecture:
                currentToast = toastLecture.gameObject;
                roomSceneManager.onRoomStateEvent += StartTraining;
                DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 5);

                string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
                SendEventMessage(message);
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
        if(user.userType == UserType.Lecture)
        {
            roomSceneManager.onRoomStateEvent -= StartTraining;
        }
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