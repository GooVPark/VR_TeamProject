using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// 룸에 처음 들어온 상태
/// 로컬 플레이어들의 버튼 상태와 이벤트 등록 및 데이터 초기화
/// </summary>
public class RoomState_WaitPlayer : RoomState
{
    //현재 상태에서 넘어갈 수 있는 상태들 선언
    public RoomState_GoToA roomStateGoToA;

    public ToastOneButton forceStartToast;

    [Header("A Objects")]
    [SerializeField] protected GameObject nextButton;
    [SerializeField] protected GameObject prevButton;

    public GameObject classObject; //구역 A의 강의에 사용되는 화면 오브젝트
    public GameObject selectLevelUI; //단계를 선택하는 버튼 오브젝트
    public ScoreBoard scoreBoardObject;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        classObject.gameObject.SetActive(true);
        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.textChatDisabled = false;
        roomSceneManager.player.QuizScore = -1;

        voiceChat.UpdateState(ButtonState.Deactivate);
        voiceChat.button.OnClick.AddListener(() => roomSceneManager.ToggleVoiceChat());

        textChat.UpdateState(ButtonState.Deactivate);
        textChat.button.OnClick.AddListener(() => roomSceneManager.ToggleTextChat());

        string message = $"{EventMessageType.LAMPUPDATE}";
        SendEventMessage(message);

        DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 0);

        if (user.userType == UserType.Lecture)
        {
            NetworkManager.Instance.scoreBoardDisabled = false;
            NetworkManager.Instance.onScoreBoard = false;
            NetworkManager.Instance.megaphoneDisabled = false;

            scoreBoard.button.OnClick.AddListener(() => ShowScoreBoard());
            megaphone.button.OnClick.AddListener(() => roomSceneManager.MegaphoneToggle());

            message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);

            forceStartToast.gameObject.SetActive(true);
            roomSceneManager.onRoomStateEvent += ForceStart;
        }
        if(user.userType == UserType.Student)
        {
            nextButton.SetActive(false);
            prevButton.SetActive(false);
            selectLevelUI.SetActive(false);
        }

        DataManager.Instance.UpdateRoomProgress(user.email, 0);

        classObject.gameObject.SetActive(false);
    }

    public override void OnStateExit()
    {
        forceStartToast.gameObject.SetActive(false);
        roomSceneManager.onRoomStateEvent -= ForceStart;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady())
        {
            roomSceneManager.RoomState = roomStateGoToA;
        }
    }

    //강의를 강제로 시작
    public void ForceStart()
    {
        photonView.RPC(nameof(ForceStartRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ForceStartRPC()
    {
        roomSceneManager.requiredPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void ShowScoreBoard()
    {
        if (scoreBoardObject.gameObject.activeSelf)
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

}
