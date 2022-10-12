using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public enum EventMessageType { MOVE, VOICECHAT, SPAWN }
public enum VoiceEventType { REQUEST, CANCEL, ACCEPT, DEACCEPT, DISCONNECT }

public class EventSyncronizer : MonoBehaviour, IChatClientListener
{
    public delegate void EventMessage();
    public static EventMessage eventMessage;

    [SerializeField] private VoiceManager voiceManager;
    [SerializeField] private LoundgeSceneManager loundgeManager;
    private ChatClient chatClient;
    [SerializeField] private string eventServer;
    private void Start()
    {
        Connect();
        DynamicRayVisualizer.eventSyncronizer += OnSendMessage;
        LoundgeSceneManager.eventMesage += OnSendMessage;
        VoiceManager.eventMessage += OnSendMessage;
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


    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("EventSyncronizer Debug Return: " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { eventServer });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        LoundgeSceneManager.Instance.isEventServerConnected = true;
    }

    public void OnDisconnected()
    {
     
    }

    public void OnSendMessage(string message)
    {
        if(string.IsNullOrEmpty(message))
        {
            return;
        }

        chatClient.PublishMessage(eventServer, message);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if(channelName.Equals(eventServer))
        {
            ChatChannel channel = null;
            bool isFound = chatClient.TryGetChannel(eventServer, out channel);
            if (!isFound)
            {
                Debug.Log("Channel not found");
                return;
            }

            string message = messages[^1].ToString();
            string[] command = message.Split('_');

            string type = command[0];

            Debug.Log(message);

            if (type.Equals(EventMessageType.SPAWN.ToString()))
            {
                LoundgeSceneManager.Instance.SpawnNPC();
            }
            if(type.Equals(EventMessageType.VOICECHAT.ToString()))
            {
                string voiceEventType = command[1];
                string senderEmail = command[2];
                string recieverEmail = command[3];

                LoundgeUser sender = loundgeManager.GetLoundgeUser(senderEmail);
                LoundgeUser reciever = loundgeManager.GetLoundgeUser(recieverEmail);

                if(voiceEventType.Equals(VoiceEventType.CANCEL.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnVoiceChatCancelEvent(sender);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnVoiceChatCancelEvent(reciever);
                    }
                }
                if (voiceEventType.Equals(VoiceEventType.REQUEST.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnVoiceChatRecieveEvent(sender, reciever);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnVoiceChatSendEvent(sender, reciever);
                    }
                }
                if (voiceEventType.Equals(VoiceEventType.ACCEPT.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnAcceptVoiceChatEventReciever(sender, reciever);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnAcceptVoiceChatEventSender(sender, reciever);
                    }
                }
                if (voiceEventType.Equals(VoiceEventType.DEACCEPT.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnDeacceptVoiceChatEventReciever(sender, reciever);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        voiceManager.OnDeacceptVoiceChatEventSender(sender, reciever);
                    }
                }

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
