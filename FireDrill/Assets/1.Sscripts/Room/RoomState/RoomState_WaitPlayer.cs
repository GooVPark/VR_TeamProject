using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// 룸에 처음 들어온 상태
/// 보이스, 택스트쳇 사용가능
/// 확성기, 채점판 사용 불가능
/// </summary>
public class RoomState_WaitPlayer : RoomState
{
    public RoomState_GoToA roomStateGoToA;

    public ToastOneButton forceStartToast;

    [Header("A Objects")]
    [SerializeField] protected GameObject nextButton;
    [SerializeField] protected GameObject prevButton;

    public GameObject view;
    public GameObject selectLevelUI;
    public ScoreBoard scoreBoardObject;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        view.gameObject.SetActive(true);
        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.textChatDisabled = false;
        roomSceneManager.player.QuizScore = -1;

        voiceChat.UpdateState(ButtonState.Deactivate);
        //voiceChat.button.onClick += roomSceneManager.ToggleVoiceChat;
        voiceChat.button.OnClick.AddListener(() => roomSceneManager.ToggleVoiceChat());

        textChat.UpdateState(ButtonState.Deactivate);
        //textChat.button.onClick += roomSceneManager.ToggleTextChat;
        textChat.button.OnClick.AddListener(() => roomSceneManager.ToggleTextChat());

        string message = $"{EventMessageType.LAMPUPDATE}";
        SendEventMessage(message);

        DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 0);

        

        if (user.userType == UserType.Lecture)
        {
            NetworkManager.Instance.scoreBoardDisabled = false;
            NetworkManager.Instance.onScoreBoard = false;

            scoreBoard.button.OnClick.AddListener(() => ShowScoreBoard());

            NetworkManager.Instance.megaphoneDisabled = false;

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
        view.gameObject.SetActive(false);
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
