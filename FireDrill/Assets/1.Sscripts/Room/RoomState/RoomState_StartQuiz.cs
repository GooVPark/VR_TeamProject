using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class RoomState_StartQuiz : RoomState
{
    [Header("Room State")]
    public RoomState_Quiz roomStateQuiz;
    [Space(5)]

    [Header("Toast")]
    public ToastTwoText toast;
    [Space(5)]

    [Header("Quiz Objects")]
    public InteractableQuizObject[] quizObjects;

    private float time = 3f;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        toast.gameObject.SetActive(true);
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        toast.text.text = $"{(int)time + 1}";

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            time -= Time.deltaTime;

            photonView.RPC(nameof(Timer), RpcTarget.All, time);

            if(time < 0)
            {
                photonView.RPC(nameof(StartQuizRPC), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void StartQuizRPC()
    {
        roomSceneManager.RoomState = roomStateQuiz;
    }

    [PunRPC]
    public void Timer(float time)
    {
        this.time = time;
    }
}
