using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class EventSyncronizerLogin : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    public string eventServer;

    private bool isConnected = false;

    public void Connect()
    {

    }


    public void Disconnect()
    {
       
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        ;
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        Debug.Log("Connect");
        isConnected = true;
        chatClient.Subscribe(new string[] { eventServer });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        isConnected = false;
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
        string fpsInfo = NetworkManager.Instance.fpsInfo;
        string message = $"{EventMessageType.LOGIN}_{NetworkManager.User.email}_{NetworkManager.User.name}_{fpsInfo}";

        //Debug.Log(message);

        chatClient.Service();
        chatClient.PublishMessage(eventServer, message);
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
