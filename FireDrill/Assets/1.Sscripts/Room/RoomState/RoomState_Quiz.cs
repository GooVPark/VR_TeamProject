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

    public GameObject timerObjectLecture;
    public TMP_Text timerTextLecture;
    [Space(5)]

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text progressText;
    public ScoreBoard scoreBoardObject;
    [Space(5)]

    [Header("Quiz Objects")]
    public QuizObjectManager quizManager;
    public InteractableQuizObject[] quizObjects;
    [Space(5)]

    [SerializeField] private float originTime;
    [SerializeField] private float time = 60f;
    private int scoreCount = 0;
    private int solveCount = 0;
    [SerializeField] private int playerCount = 1;
    private bool endQuiz = false;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        playerCount = 1;
        time = originTime;

        scoreCount = 0;
        solveCount = 0;

        endQuiz = false;

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            timerObjectLecture.SetActive(true);
            //scoreBoard.UpdateState(ButtonState.Deactivate);
        }
        if (NetworkManager.User.userType == UserType.Student)
        {
            timerObject.SetActive(true);

            foreach (var quizObject in quizObjects)
            {
                quizObject.Activate();
            }

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
        timerObjectLecture.SetActive(false);

        if (NetworkManager.User.userType == UserType.Student)
        {
            foreach (var quizObject in quizObjects)
            {
                quizObject.Activate();
            }

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
        if(time <= 0f)
        {
            time = 0;
        }
        timerText.text = $"{(int)time / 60} : {(int)time % 60}";
        timerTextLecture.text = $"{(int)time / 60} : {(int)time % 60}";
        progressText.text = $"{solveCount}/10";
        scoreText.text = $"{scoreCount}";

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            time -= Time.deltaTime;

            photonView.RPC(nameof(Timer), RpcTarget.All, time);

            if (time <= 0 && !endQuiz)
            {
                endQuiz = true;
                photonView.RPC(nameof(ShowQuizResultRPC), RpcTarget.Others);

            }
            if (roomSceneManager.IsReady(playerCount))
            {
                roomSceneManager.RoomState = roomStateQuizWait;
            }
        }

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

        roomSceneManager.player.QuizScore = scoreCount * 10;
        //photonView.RPC(nameof(SetScoreRPC), RpcTarget.All, scoreCount);
    }

    [PunRPC]
    private void SetScoreRPC(int scoreCount)
    {
        roomSceneManager.player.scoreUI.text = $"{scoreCount * 10}";
    }
    public void ShowQuizResult()
    {
        if (NetworkManager.User.userType == UserType.Student || roomSceneManager.RoomState != roomStateQuizWait)
        {
            quizManager.gameObject.SetActive(false);
            timerObjectLecture.SetActive(false);
            timerObject.SetActive(false);
            toast.gameObject.SetActive(true);
            toast.text.text = $"{scoreCount * 10}";
        }
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

    }

    [PunRPC]
    public void Timer(float time)
    {
        this.time = time;
    }
}
