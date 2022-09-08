using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_InClass : RoomState
{
    [Header("Room State")]
    public RoomState_Class roomStateClass;
    [Space(5)]

    [Header("Toast")]
    public Toast lectureToast;
    public Toast studentToast;
    public Toast disMicToast;
    private GameObject currentToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if (NetworkManager.User.userType == UserType.Lecture)
        {
            lectureToast.gameObject.SetActive(true);
            currentToast = lectureToast.gameObject;

            roomSceneManager.onRoomStateEvent += OnClassStart;
        }
        if (NetworkManager.User.userType == UserType.Student)
        {
            studentToast.gameObject.SetActive(true);
            currentToast = studentToast.gameObject;
        }
    }

    public override void OnStateExit()
    {
        currentToast.SetActive(false);
        roomSceneManager.onRoomStateEvent -= OnClassStart;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
     //On DisMic Toast;

    }

    public void OnClassStart()
    {
        photonView.RPC(nameof(ClassStartRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ClassStartRPC()
    {
        roomSceneManager.RoomState = roomStateClass;
    }
}
