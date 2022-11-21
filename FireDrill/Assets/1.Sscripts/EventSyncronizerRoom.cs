using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class EventSyncronizerRoom : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string masterChannel = "_MASTER_";
    public static EventSyncronizerRoom Instance;

    public delegate void EventMessage();
    public EventMessage eventMessage;

    [SerializeField] private RoomSceneManager roomSceneManager;
    private ChatClient chatClient;
    [SerializeField] private string eventServer;
    [SerializeField] private TextChatManager textChatManager;
    [SerializeField] private ScoreBoard scoreBoard;

    public bool onConnected = false;

    private List<string> eventList = new List<string>();

    [Header("To Master")]
    [SerializeField] private float responseIntervalToMaster = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }
    }

    private void Start()
    { 
        Connect();

        RoomSceneManager.eventMessage = null;
        RoomSceneManager.eventMessage += OnSendMessage;
        textChatManager.eventMessage += OnSendMessage;
    }

    private void FixedUpdate()
    {
        chatClient.Service();

        if (eventList.Count > 0)
        {
            string message = eventList[0];
            eventList.RemoveAt(0);
            Debug.Log("Event List Count: " + eventList.Count);
            ExcuteEvent(message);
        }
    }

    public void Connect()
    {
        Application.runInBackground = true;

        chatClient = new ChatClient(this)
        {
            UseBackgroundWorkerForSending = true
        };

        string email = NetworkManager.User.email;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(email));
        
    }

    public void Disconnect()
    {
        chatClient.Disconnect();
    }

    public void OnSendMessage(string message)
    {
        //if (string.IsNullOrEmpty(message))
        //{
        //    return;
        //}

        //chatClient.PublishMessage(eventServer, message);
        //DataManager.Instance.WriteLog(message);
        SendToMasterClient(message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
     
    }

    public void OnChatStateChange(ChatState state)
    {
     
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { eventServer });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);

        roomSceneManager.isEventServerConnected = true;
    }

    public void OnDisconnected()
    {
     
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(eventServer))
        {
            ChatChannel channel = null;
            bool isFound = chatClient.TryGetChannel(eventServer, out channel);
            if (!isFound)
            {
                Debug.Log("Channel not found");
                return;
            }

            string message = messages[^1].ToString();

            Debug.Log(message);

            string[] command = message.Split('_');

            string type = command[0];

            if (type.Equals(EventMessageType.TEXTCHAT.ToString()))
            {
                string senderEmail = command[1];
                string senderName = command[2];
                string chatMessage = command[3];
                int roomNumber = int.Parse(command[4]);

                textChatManager.OnGetMessage(senderName, chatMessage, roomNumber);
            }
            if (type.Equals(EventMessageType.QUIZ.ToString()))
            {
                scoreBoard.UpdateScoreBoard();
            }
            if(type.Equals(EventMessageType.FORCEEXIT.ToString()))
            {
                Debug.Log("Leave Room Event Message");
                roomSceneManager.LeaveRoom();
            }
        }
    }

    private void ExcuteEvent(string eventMessage)
    {
        string message = eventMessage;

        Debug.Log(message);

        string[] command = message.Split('_');

        string type = command[0];

        if (type.Equals(EventMessageType.QUIZ.ToString()))
        {
            scoreBoard.UpdateScoreBoard();
        }
        if (type.Equals(EventMessageType.FORCEEXIT.ToString()))
        {
            Debug.Log("Leave Room Event Message");
            roomSceneManager.LeaveRoom();
        }

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //if (sender.Equals(masterChannel))
        //{
        //    Debug.Log("Get Private Message: " + message.ToString());
        //    //eventQueue.Enqueue(message.ToString());
        //    eventList.Add(message.ToString());
        //}
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
     
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed on Room");
        StartCoroutine(ResponseLoop());
    }

    public void OnUnsubscribed(string[] channels)
    {
     
    }

    public void OnUserSubscribed(string channel, string user)
    {
     
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
     
    }

    #region To MasterClient

    int sendCount = 0;
    /// <summary>
    /// 마스터 클라언트에게 주기적으로 메세지를 보냄
    /// </summary>
    /// <param name="sender"></param>
    public void ResponToMasterClient(string sender)
    {
        chatClient.SendPrivateMessage(masterChannel, "!");
    }
    private IEnumerator ResponseLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(responseIntervalToMaster);

        while (true)
        {
            yield return wait;
            //Debug.Log("Responed to Master");
            ResponToMasterClient(NetworkManager.User.email);
        }
    }

    /// <summary>
    /// 마스터에 이벤트 메세지 전송
    /// </summary>
    /// <param name="message"></param>
    public void SendToMasterClient(string message)
    {
        Debug.Log("Send Message: " + message);
        chatClient.SendPrivateMessage(masterChannel, message);
    }

    #endregion
}
