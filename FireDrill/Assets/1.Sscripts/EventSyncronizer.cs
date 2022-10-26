using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public enum EventMessageType { TEXTCHAT, MOVE, VOICECHAT, SPAWN, DISCONNECT, NOTICE, PROGRESS, QUIZ, UPDATEROOMSTATE }
public enum VoiceEventType { REQUEST, CANCEL, ACCEPT, DEACCEPT, DISCONNECT, CONNECT }
public enum NoticeEventType { ONVOICE, JOIN, DISCONNECT }
public enum ProgressEventType { UPDATE, PLAYERCOUNT }

public class EventSyncronizer : MonoBehaviour, IChatClientListener
{
    public delegate void EventMessage();
    public static EventMessage eventMessage;

    [SerializeField] private VoiceManager voiceManager;
    [SerializeField] private LoundgeSceneManager loundgeManager;
    [SerializeField] private TextChatManager textChatManager;

    private ChatClient chatClient;
    [SerializeField] private string eventServer;

    ChatChannel chatChannel;
    private void Start()
    {
        Connect();

        DynamicRayVisualizer.eventSyncronizer = null;
        LoundgeSceneManager.eventMesage = null;
        VoiceManager.eventMessage = null;

        DynamicRayVisualizer.eventSyncronizer += OnSendMessage;
        LoundgeSceneManager.eventMesage += OnSendMessage;
        VoiceManager.eventMessage += OnSendMessage;
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

        Debug.Log("Connected");
    }

    public void DisconnectChat()
    {
        chatClient.Disconnect();
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

            if(type.Equals(EventMessageType.TEXTCHAT.ToString()))
            {
                string sender = command[1];
                string chatMessage = command[2];

                textChatManager.OnGetMessage(sender, chatMessage, NetworkManager.RoomNumber);
            }
            if(type.Equals(EventMessageType.NOTICE.ToString()))
            {
                string noticeEventType = command[1];

                if(noticeEventType.Equals(NoticeEventType.JOIN.ToString()))
                {
                    int roomNumber = int.Parse(command[2]);
                    string targetEmail = command[3];
                    loundgeManager.UpdateRoomPlayerCount(roomNumber);
                    loundgeManager.UpdateLobbyPlayerCount();
                }
                if(noticeEventType.Equals(NoticeEventType.DISCONNECT.ToString()))
                {
                    int roomNumber = int.Parse(command[2]);
                    string targetEmail = command[3];
                    loundgeManager.UpdateRoomPlayerCount(roomNumber);
                    loundgeManager.UpdateLobbyPlayerCount();
                }
            }
            if(type.Equals(EventMessageType.PROGRESS.ToString()))
            {
                string progreesEventType = command[1];
                string roomNumber = command[2];

                if(progreesEventType.Equals(ProgressEventType.UPDATE.ToString()))
                {
                    loundgeManager.UpdateProgressBoard();
                }
            }
            if (type.Equals(EventMessageType.SPAWN.ToString()))
            {
                LoundgeSceneManager.Instance.SpawnNPC();
                loundgeManager.UpdateLobbyPlayerCount();
            }
            if (type.Equals(EventMessageType.DISCONNECT.ToString()))
            {
                string email = command[1];
                loundgeManager.RemoveNPCObject(email);
                loundgeManager.UpdateLobbyPlayerCount();
            }
            if(type.Equals(EventMessageType.UPDATEROOMSTATE.ToString()))
            {
                int roomNumber = int.Parse(command[1]);
                loundgeManager.UpdateRoomEnterence(roomNumber);
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

                    loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                    loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                    loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(true);
                    loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(true);
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

                    loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                    loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                    loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(false);
                    loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(false);
                }
                if (voiceEventType.Equals(VoiceEventType.CONNECT.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        loundgeManager.JoinVoiceChatRoom(senderEmail);
                        voiceManager.OnConnectVoiceManagerEvent(recieverEmail);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        loundgeManager.JoinVoiceChatRoom(senderEmail);
                        voiceManager.OnConnectVoiceManagerEvent(senderEmail);
                    }

                    loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                    loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                    loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(true);
                    loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(true);

                }
                if (voiceEventType.Equals(VoiceEventType.DISCONNECT.ToString()))
                {
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        loundgeManager.LeaveVoiceChatRoom();
                        voiceManager.OnDisconnectVoiceChatEvent(recieverEmail);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        loundgeManager.LeaveVoiceChatRoom();
                        voiceManager.OnDisconnectVoiceChatEvent(senderEmail);
                    }

                    loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                    loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                    loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(false);
                    loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(false);
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
