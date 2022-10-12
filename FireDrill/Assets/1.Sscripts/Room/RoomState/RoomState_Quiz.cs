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
    [Space(5)]

    private float time = 600f;
    private int scoreCount = 0;
    private int solveCount = 0;
    [SerializeField] private int playerCount = 1;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
       
        timerObject.SetActive(true);

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            scoreObject.SetActive(false);
            scoreBoard.UpdateState(ButtonState.Deactivate);
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            foreach (InteractableQuizObject quizObject in quizManager.quizObjects)
            {
                quizObject.onSubmit += ScoreCount;
            }

            roomSceneManager.onRoomStateEvent += ConfirmQuizResult;
        }
    }

    public override void OnStateExit()
    {
        timerObject.SetActive(false);
        if (NetworkManager.User.userType == UserType.Student)
        {
            roomSceneManager.onRoomStateEvent -= ConfirmQuizResult;
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

                photonView.RPC(nameof(ShowQuizResultRPC), RpcTarget.Others);

            }
            if (roomSceneManager.IsReady(playerCount))
            {
                roomSceneManager.RoomState = roomStateQuizWait;
            }
        }

        timerText.text = $"{(int)time / 60} : {(int)time % 60}";
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
        toast.text.text = $"{scoreCount * 10} / 30";
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
            timerText.text = ((int)time).ToString();
        }
    }
}
