using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ���� ���¸� �����ϴµ� �����
/// ���� ���¸� �߰��ϱ� ���ؼ��� RoomState�� ��ӹ޴� Class�� �߰��Ѵ�.
/// Class�� ��������� �ش� ���¿� �ʿ��� ������� �Լ����� �����ϰ� �� ���� GameState�� �ڽ� ������Ʈ�� �� ������Ʈ�� ���� Class�� �߰��Ѵ�.
/// 
/// RoomState�� ��ӹ޴� Class�� ���� ���۵ɶ� �ʱ� ���·� ��ϵ� RoomState�� ȣ��ǰ� ���� �÷��̾��� �ൿ Ȥ�� ���� �����Ȳ�� ���� RoomState�� �ٲ��.
/// RoomState�� �ٲ𶧿��� ���� RoomState�� OnStateExit()�Լ��� ȣ��ǰ� ���� ���� RoomState�� OnStateEnter()�Լ��� ȣ��ǰ� RoomSceneManager�� Update()���� RoomState�� OnStateUpdate()�� ȣ��ȴ�.
/// </summary>
public abstract class RoomState : MonoBehaviourPun
{
    /// <summary>
    /// ��� RoomState���� ��ӹ޴� RoomState �߻� Ŭ����
    /// �ٸ� RoomState���� �������� ������ �ϴ� ������ �Լ����� �����ϰ� �ʱ�ȭ �Ѵ�.
    /// </summary>

    public delegate void EventMessage(string message);
    public EventMessage eventMessage;

    public RoomSceneManager roomSceneManager;
    public EventSyncronizerRoom eventSyncronizer;
    protected User user;

    [Header("UI")]//���� ���� ��Ȳ�� ���� ��ư�� ���°� ���ϴ� ��찡 �ֱ� ������ �̸� ��ư���� ������ �ִ´�.
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
