using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_InQuiz : RoomState
{
    [Header("Room State")]
    public RoomState_StartQuiz roomStateStartQuiz;
    [Space(5)]

    [Header("Toast")]
    public ToastOneButton lectureToast;
    public Toast studnetToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        roomSceneManager.onRoomStateEvent += OnQuizStart;
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            lectureToast.gameObject.SetActive(true);
        }
        else
        {
            studnetToast.gameObject.SetActive(true);
        }
    }

    public override void OnStateExit()
    {
        roomSceneManager.onRoomStateEvent -= OnQuizStart;
        if (NetworkManager.User.userType == UserType.Lecture)
        {
            lectureToast.gameObject.SetActive(false);
        }
        else
        {
            studnetToast.gameObject.SetActive(false);
        }
        base.OnStateExit();
    }


    public void OnQuizStart()
    {
        photonView.RPC(nameof(OnQuizStartRPC), RpcTarget.All);
    }

    [PunRPC]
    public void OnQuizStartRPC()
    {
        roomSceneManager.RoomState = roomStateStartQuiz;
    }
}
