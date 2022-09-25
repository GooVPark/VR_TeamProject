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
    [SerializeField] private string worldChat;
    
    public ChatClient chatClient;

    public TMP_Text[] chatList;

    private void Start()
    {
        Connect();
    }

    private void Update()
    {
        chatClient.Service();
    }
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void Connect()
    {
        Application.runInBackground = true;

        chatClient = new ChatClient(this)
        {
            UseBackgroundWorkerForSending = true
        };

        string email = NetworkManager.User.email;

        Debug.Log(email);

        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(email));
    }
    public void OnConnected()
    {
        Debug.Log("Connected to Chat");
        chatClient.Subscribe(new string[] { worldChat });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void SendChatMessage(InputField inputField)
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        chatClient.PublishMessage(worldChat, inputField.text);
    }

    public void DisconnectChat()
    {
        //chatClient.Disconnect();
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected to Chat server");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(worldChat))
        {
            ChatChannel channel = null;
            bool isFound = chatClient.TryGetChannel(worldChat, out channel);
            if (!isFound)
            {
                Debug.Log("Channel not found");
                return;
            }

            chatList[0].text = channel.ToStringMessages();
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
