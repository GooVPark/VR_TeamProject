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
    public GameObject scoreObject;
    public TMP_Text scoreText;
    public ScoreBoard scoreBoardObject;
    [Space(5)]

    [Header("Quiz Objects")]
    public QuizObjectManager quizManager;
    public InteractableQuizObject[] quizObjects;
    [Space(5)]

    private float time = 600f;
    private int scoreCount = 0;
    private int solveCount = 0;
    [SerializeField] private int playerCount = 1;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
       
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            timerObjectLecture.SetActive(true);
            //scoreBoard.UpdateState(ButtonState.Deactivate);
            NetworkManager.Instance.scoreBoardDisabled = false;
            NetworkManager.Instance.onScoreBoard = false;

            scoreBoard.button.OnClick.AddListener(() => ShowScoreBoard());
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

        timerText.text = $"{(int)time / 60} : {(int)time % 60}";
        timerTextLecture.text = $"{(int)time / 60} : {(int)time % 60}";
        scoreText.text = $"{scoreCount}/{solveCount}";

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            time -= Time.deltaTime;

            photonView.RPC(nameof(Timer), RpcTarget.All, time);

            if (time < 0)
            {

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
    }

    public void ShowQuizResult()
    {
        timerObjectLecture.SetActive(false);
        timerObject.SetActive(false);
        toast.gameObject.SetActive(true);
        toast.text.text = $"{scoreCount * 10}";
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

    public void ShowScoreBoard()
    {
        if(scoreBoardObject.gameObject.activeSelf)
        {
            scoreBoardObject.gameObject.SetActive(false);
            NetworkManager.Instance.onScoreBoard = false;
        }
        else
        {
            scoreBoardObject.gameObject.SetActive(true);
            NetworkManager.Instance.onScoreBoard = true;
        }
    }

    [PunRPC]
    public void Timer(float time)
    {
        this.time = time;
    }
}
