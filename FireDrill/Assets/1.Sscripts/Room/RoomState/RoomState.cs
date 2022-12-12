using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 룸의 상태를 제어하는데 사용함
/// 룸의 상태를 추가하기 위해서는 RoomState를 상속받는 Class를 추가한다.
/// Class를 만들었으면 해당 상태에 필요한 변수들과 함수들을 선언하고 룸 씬의 GameState의 자식 오브젝트에 빈 오브젝트를 만들어서 Class를 추가한다.
/// 
/// RoomState를 상속받는 Class는 룸이 시작될때 초기 상태로 등록된 RoomState가 호출되고 이후 플레이어의 행동 혹은 강의 진행상황에 따라 RoomState가 바뀐다.
/// RoomState가 바뀔때에는 이전 RoomState의 OnStateExit()함수가 호출되고 이후 현재 RoomState의 OnStateEnter()함수가 호출되고 RoomSceneManager의 Update()에서 RoomState의 OnStateUpdate()가 호출된다.
/// </summary>
public abstract class RoomState : MonoBehaviourPun
{
    /// <summary>
    /// 모든 RoomState들이 상속받는 RoomState 추상 클래스
    /// 다른 RoomState들이 공통으로 가져야 하는 변수와 함수들을 선언하고 초기화 한다.
    /// </summary>

    public delegate void EventMessage(string message);
    public EventMessage eventMessage;

    public RoomSceneManager roomSceneManager;
    public EventSyncronizerRoom eventSyncronizer;
    protected User user;

    [Header("UI")]//강의 진행 상황에 따라 버튼의 상태가 변하는 경우가 있기 때문에 미리 버튼들을 가지고 있는다.
    [SerializeField] protected ButtonStateHandler megaphone;
    [SerializeField] protected ButtonStateHandler scoreBoard;
    [SerializeField] protected ButtonStateHandler voiceChat;
    [SerializeField] protected ButtonStateHandler textChat;

    private void Awake()
    {
     
    }

    protected void SendEventMessage(string message)
    {
        roomSceneManager.SendEventMessage(message);
    }

    public virtual void OnStateEnter()
    {
        gameObject.SetActive(true);

        roomSceneManager = FindObjectOfType<RoomSceneManager>();
        eventSyncronizer = FindObjectOfType<EventSyncronizerRoom>();

        megaphone = roomSceneManager.megaphoneButton;
        scoreBoard = roomSceneManager.scoreboardButton;
        voiceChat = roomSceneManager.voiceChatButton;
        textChat = roomSceneManager.textChatButton;

        user = NetworkManager.User;
    }

    public virtual void OnStateExit()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnUpdate()
    {
        
    }
}
