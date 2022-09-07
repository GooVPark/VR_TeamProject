using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            roomSceneManager.onRoomStateEvent += OnClassEnd;
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
        roomSceneManager.onRoomStateEvent -= OnClassEnd;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
     //On DisMic Toast;

    }

    public void OnClassEnd()
    {
        
    }
}
