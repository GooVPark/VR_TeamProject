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
    [Space(5)]

    [Header("EventArea")]
    public EventArea eventArea;
    [Space(5)]

    [Header("QuizObject")]
    public GameObject quizObject;
    public GameObject npc;
    public QuizManager quizManager;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        DataManager.Instance.InitializeQuizScore(NetworkManager.User.email);
        roomSceneManager.player.QuizScore = -1;
        if (user.userType == UserType.Lecture)
        {
            eventMessage = null;
            eventMessage += eventSyncronizer.OnSendMessage;

            string message = $"{EventMessageType.QUIZ}";
            eventMessage?.Invoke(message);
        }

        eventArea.gameObject.SetActive(false);
        quizObject.SetActive(true);
        npc.SetActive(true);

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            lectureToast.gameObject.SetActive(true);
            roomSceneManager.onRoomStateEvent += OnQuizStart;
            DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 3);
            quizObject.GetComponent<QuizObjectManager>().SetQuiz();
            string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);
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
