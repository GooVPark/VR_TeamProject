using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;

public class TextChatManager : MonoBehaviour, IChatClientListener
{
    private string worldChat;
    public ChatClient chatClient;

    private void Start()
    {
        Connect();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }

    public void Connect()
    {
        worldChat = "WORLD_CHAT";

        chatClient = new ChatClient(this);
        bool isConnected = chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "0.0.1", new Photon.Chat.AuthenticationValues(NetworkManager.User.email));
        Debug.Log(isConnected);
    }
    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { worldChat });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);

        chatClient.PublishMessage(worldChat, "test Chat server");
        Debug.Log("Connected to Chat");
    }

    public void OnDisconnected()
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            Debug.Log(messages[i]);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("On Subscribed");
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
}
