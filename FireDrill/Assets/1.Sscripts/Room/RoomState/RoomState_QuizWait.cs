using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_QuizWait : RoomState
{
    [Header("RoomState")]
    public RoomState_EndQuiz roomStateQuizEnd;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;
    [Space(5)]

    private int playerCount = 0;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            photonView.RPC(nameof(SetState), RpcTarget.All);
        }
        else
        {
            toast.gameObject.SetActive(true);
        }
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    [PunRPC]
    public void SetState()
    {
        roomSceneManager.RoomState = roomStateQuizEnd;
    }
}
