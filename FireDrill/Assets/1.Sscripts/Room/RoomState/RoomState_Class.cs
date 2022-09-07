using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_Class : RoomState
{
    [Header("Room State")]
    public RoomState_ClassEnd roomStateClassEnd;

    [Header("Toast")]
    public Toast lectureToast;
    public Toast studentToast;
    private GameObject currentToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            currentToast = lectureToast.gameObject;
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {

        currentToast.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        
    }
}
