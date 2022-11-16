using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class EventSyncronizerRoom : MonoBehaviour, IChatClientListener
{
    public static EventSyncronizerRoom Instance;

    public delegate void EventMessage();
    public EventMessage eventMessage;

    [SerializeField] private RoomSceneManager roomSceneManager;
    private ChatClient chatClient;
    [SerializeField] private string eventServer;
    [SerializeField] private TextChatManager textChatManager;
    [SerializeField] private ScoreBoard scoreBoard;

    public bool onConnected = false;

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

    private void Update()
    {
        chatClient.Service();
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
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        chatClient.PublishMessage(eventServer, message);
        DataManager.Instance.WriteLog(message);
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
                string sender = command[1];
                string chatMessage = command[2];
                int roomNumber = int.Parse(command[3]);

                textChatManager.OnGetMessage(sender, chatMessage, roomNumber);
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

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
     
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
     
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed on Room");
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
}
