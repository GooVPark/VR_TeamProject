using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomState_Quiz : RoomState, IPunObservable
{
    //QZ 부터 QZ-SC까지
    [Header("Room State")]
    public RoomState_QuizWait roomStateQuizWait;
    [Space(5)]

    [Header("Toast")]
    public ToastTwoTextOneButton toast;
    [Space(5)]

    [Header("Timer")]
    public GameObject timerObject;
    public TMP_Text timerText;
    [Space(5)]

    [Header("Score")]
    public GameObject scoreObject;
    public TMP_Text scoreText;
    [Space(5)]

    [Header("Quiz Objects")]
    public QuizObjectManager quizManager;

    private float time = 600f;
    private int scoreCount = 0;
    private int solveCount = 0;
    private int playerCount = 1;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
       
        timerObject.SetActive(true);

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            scoreObject.SetActive(false);
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            foreach (InteractableQuizObject quizObject in quizManager.quizObjects)
            {
                quizObject.onSubmit += ScoreCount;
            }
        }
    }

    public override void OnStateExit()
    {
        timerObject.SetActive(false);
        if (NetworkManager.User.userType == UserType.Student)
        {
            foreach (InteractableQuizObject quizObject in quizManager.quizObjects)
            {
                quizObject.onSubmit -= ScoreCount;
            }
        }
        base.OnStateExit();
    }
    public override void OnUpdate()
    {
        if (NetworkManager.User.userType == UserType.Lecture)
        {
            time -= Time.deltaTime;

            if (time < 0)
            {
                if (NetworkManager.User.userType == UserType.Student)
                {
                    photonView.RPC(nameof(ShowQuizResultRPC), RpcTarget.All);
                }
            }
            if (roomSceneManager.IsReady(playerCount))
            {
                roomSceneManager.RoomState = roomStateQuizWait;
            }
        }

        timerText.text = ((int)time).ToString();
        scoreText.text = $"{scoreCount}/{solveCount}";

        if (solveCount >= quizManager.quizObjects.Length)
        {
            ShowQuizResult();
        }
    }

    public void ScoreCount(int result)
    {
        if (result == 1)
        {
            scoreCount++;
        }
        solveCount++;
    }

    public void ShowQuizResult()
    {

        timerObject.SetActive(false);
        toast.gameObject.SetActive(true);
        toast.text.text = $"{scoreCount * 10} / 100";
    }

    [PunRPC]
    public void ShowQuizResultRPC()
    {
        ShowQuizResult();
    }

    public void ConfirmQuizResult()
    {
        toast.gameObject.SetActive(false);
        photonView.RPC(nameof(AddPlayerCount), RpcTarget.All);
        roomSceneManager.RoomState = roomStateQuizWait;
    }

    [PunRPC]
    public void AddPlayerCount()
    {
        playerCount++;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
        }
        if (stream.IsReading)
        {
            time = (float)stream.ReceiveNext();
        }
    }
}
