using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class EventSyncronizerRoom : MonoBehaviour, IChatClientListener
{
    public delegate void EventMessage();
    public EventMessage eventMessage;

    [SerializeField] private RoomSceneManager roomSceneManager;
    private ChatClient chatClient;
    [SerializeField] private string eventServer;

    private void Start()
    { 
        Connect();

        RoomSceneManager.eventMessage = null;
        RoomSceneManager.eventMessage += SendMessage;
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

    public void OnSendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        chatClient.PublishMessage(eventServer, message);
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
    }

    public void OnDisconnected()
    {
     
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
     
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
