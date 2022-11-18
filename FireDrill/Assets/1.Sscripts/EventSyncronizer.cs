using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public enum EventMessageType { TEXTCHAT, MOVE, VOICECHAT, SPAWN, DISCONNECT, NOTICE, PROGRESS, QUIZ, UPDATEROOMSTATE, UPDATEUSERCOUNT, LAMPUPDATE, FORCEEXIT, OUT }
public enum VoiceEventType { REQUEST, CANCEL, ACCEPT, DEACCEPT, DISCONNECT, CONNECT }
public enum NoticeEventType { ONVOICE, JOIN, DISCONNECT }
public enum ProgressEventType { UPDATE, PLAYERCOUNT }

public enum MasterClientProtocol { RESPONSE }

public class EventSyncronizer : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string masterChannel = "_MASTER_";

    public delegate void EventMessage();
    public static EventMessage eventMessage;

    [SerializeField] private VoiceManager voiceManager;
    [SerializeField] private LoundgeSceneManager loundgeManager;
    [SerializeField] private TextChatManager textChatManager;

    private ChatClient chatClient;
    [SerializeField] private string eventServer;

    private Queue<string> eventQueue = new Queue<string>();
    [SerializeField] private List<string> eventList = new List<string>();

    [Header("To Master")]
    [SerializeField] private float responseIntervalToMaster = 3f;

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



    private void FixedUpdate()
    {
        chatClient.Service();

        //if (eventQueue.TryDequeue(out string message))
        //{
        //    ExcuteEvent(message);
        //}

        if(eventList.Count > 0)
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

    public void DisconnectChat()
    {
        chatClient.Disconnect();
    }
    public void OnDisconnected()
    {
     
    }

    public void OnSendMessage(string message)
    {
        //if(string.isnullorempty(message))
        //{
        //    return;
        //}

        //chatClient.PublishMessage(eventServer, message);
        //DataManager.Instance.WriteLog(message);

        SendToMasterClient(message);
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

            Debug.Log("Get Public Message: " + message);

            string[] command = message.Split('_');

            string type = command[0];
            if (type.Equals(EventMessageType.OUT))
            {
                string target = command[1];
                loundgeManager.RemoveTargetNPC(target);
            }
            if (type.Equals(EventMessageType.LAMPUPDATE.ToString()))
            {
                loundgeManager.UpdateRoomStateLamp();
            }
            if(type.Equals(EventMessageType.TEXTCHAT.ToString()))
            {
                string senderEmail = command[1];
                string senderName = command[2];
                string chatMessage = command[3];
                int roomNumber = int.Parse(command[4]);

                textChatManager.OnGetMessage(senderName, chatMessage, roomNumber);
                if (!NetworkManager.User.email.Equals(senderEmail))
                {
                    if (loundgeManager.spawnedNPCObject.ContainsKey(senderEmail))
                    {
                        loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().ShowBubble(chatMessage.Split('=')[0]);
                    }
                }
            }
            if(type.Equals(EventMessageType.UPDATEUSERCOUNT.ToString()))
            {
                int loundgeUserCount = int.Parse(command[1]);
                loundgeManager.UpdateLobbyPlayerCount(loundgeUserCount);
                int roomUserCount = int.Parse(command[2]);
                loundgeManager.UpdateRoomPlayerCount(roomUserCount);

                Debug.Log($"Loundge User Count: {loundgeUserCount}\nRoom User Count: {roomUserCount}");
            }
            //if(type.Equals(EventMessageType.NOTICE.ToString()))
            //{
            //    string noticeEventType = command[1];

            //    if(noticeEventType.Equals(NoticeEventType.JOIN.ToString()))
            //    {
            //        int roomNumber = int.Parse(command[2]);
            //        string targetEmail = command[3];
            //        loundgeManager.UpdateRoomPlayerCount(roomNumber);
            //        loundgeManager.UpdateLobbyPlayerCount();
            //    }
            //    if(noticeEventType.Equals(NoticeEventType.DISCONNECT.ToString()))
            //    {
            //        int roomNumber = int.Parse(command[2]);
            //        string targetEmail = command[3];
            //        loundgeManager.UpdateRoomPlayerCount(roomNumber);
            //        loundgeManager.UpdateLobbyPlayerCount();
            //    }
            //}
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
            }
            if (type.Equals(EventMessageType.DISCONNECT.ToString()))
            {
                string email = command[1];
                loundgeManager.RemoveNPCObject(email);
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

                    //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                    //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                    //loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(false);
                    //loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(false);
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

                    //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                    //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                    //loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(true);
                    //loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(true);
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

                    //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                    //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                }
                if (voiceEventType.Equals(VoiceEventType.CONNECT.ToString()))
                {
                    Debug.Log("Connect Voice Chat");
                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        Debug.Log("ConnectEvent Reciever: " + recieverEmail);
                        voiceManager.OnConnectVoiceManagerEvent(recieverEmail);
                        loundgeManager.JoinVoiceChatRoom(senderEmail);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        Debug.Log("ConnectEvent Sender: " + senderEmail);
                        voiceManager.OnConnectVoiceManagerEvent(senderEmail);
                        loundgeManager.JoinVoiceChatRoom(senderEmail);
                    }

                    //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                    //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                    GameObject spawnedNPC = null;
                    if(loundgeManager.spawnedNPCObject.TryGetValue(senderEmail, out spawnedNPC))
                    {
                        spawnedNPC.GetComponent<NPCController>().OnVoiceChat(true);
                    }
                    
                    if(loundgeManager.spawnedNPCObject.TryGetValue(recieverEmail, out spawnedNPC))
                    {
                        spawnedNPC.GetComponent<NPCController>().OnVoiceChat(true);
                    }
                }
                if (voiceEventType.Equals(VoiceEventType.DISCONNECT.ToString()))
                {

                    if (recieverEmail.Equals(NetworkManager.User.email))
                    {
                        Debug.Log("DisconnectEvent RecieverEmail:" + recieverEmail);
                        loundgeManager.LeaveVoiceChatRoom();
                        voiceManager.OnDisconnectVoiceChatEvent(recieverEmail);
                    }
                    if (senderEmail.Equals(NetworkManager.User.email))
                    {
                        Debug.Log("DisconnectEvent SenderEmail: " + senderEmail);
                        loundgeManager.LeaveVoiceChatRoom();
                        voiceManager.OnDisconnectVoiceChatEvent(senderEmail);

                    }

                    //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                    //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                    GameObject spawnedNPC = null;
                    if (loundgeManager.spawnedNPCObject.TryGetValue(senderEmail, out spawnedNPC))
                    {
                        spawnedNPC.GetComponent<NPCController>().OnVoiceChat(false);
                    }

                    if (loundgeManager.spawnedNPCObject.TryGetValue(recieverEmail, out spawnedNPC))
                    {
                        spawnedNPC.GetComponent<NPCController>().OnVoiceChat(false);
                    }
                }
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if(sender.Equals(masterChannel))
        {
            Debug.Log("Get Private Message: " + message.ToString());
            //eventQueue.Enqueue(message.ToString());
            eventList.Add(message.ToString());
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
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

    private void ExcuteEvent(string eventMessage)
    {
        Debug.Log("Excute Event: " + eventMessage);

        string[] command = eventMessage.Split('_');

        string type = command[0];

        if (type.Equals(EventMessageType.LAMPUPDATE.ToString()))
        {
            loundgeManager.UpdateRoomStateLamp();
        }
        if (type.Equals(EventMessageType.TEXTCHAT.ToString()))
        {
            string senderEmail = command[1];
            string senderName = command[2];
            string chatMessage = command[3];
            int roomNumber = int.Parse(command[4]);

            textChatManager.OnGetMessage(senderName, chatMessage, roomNumber);
            if (!NetworkManager.User.email.Equals(senderEmail))
            {
                if (loundgeManager.spawnedNPCObject.ContainsKey(senderEmail))
                {
                    loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().ShowBubble(chatMessage.Split('=')[0]);
                }
            }
        }
        //마스터로 부터 OUT메세지를 받음
        if(type.Equals(EventMessageType.OUT))
        {
            string target = command[1];
            loundgeManager.RemoveTargetNPC(target);
        }
        //if (type.Equals(EventMessageType.NOTICE.ToString()))
        //{
        //    string noticeEventType = command[1];

        //    if (noticeEventType.Equals(NoticeEventType.JOIN.ToString()))
        //    {
        //        int roomNumber = int.Parse(command[2]);
        //        string targetEmail = command[3];
        //        loundgeManager.UpdateRoomPlayerCount(roomNumber);
        //        loundgeManager.UpdateLobbyPlayerCount();
        //    }
        //    if (noticeEventType.Equals(NoticeEventType.DISCONNECT.ToString()))
        //    {
        //        int roomNumber = int.Parse(command[2]);
        //        string targetEmail = command[3];
        //        loundgeManager.UpdateRoomPlayerCount(roomNumber);
        //        loundgeManager.UpdateLobbyPlayerCount();
        //    }
        //}
        if (type.Equals(EventMessageType.PROGRESS.ToString()))
        {
            string progreesEventType = command[1];
            string roomNumber = command[2];

            if (progreesEventType.Equals(ProgressEventType.UPDATE.ToString()))
            {
                loundgeManager.UpdateProgressBoard();
            }
        }
        if (type.Equals(EventMessageType.SPAWN.ToString()))
        {
            LoundgeSceneManager.Instance.SpawnNPC();
        }
        if (type.Equals(EventMessageType.DISCONNECT.ToString()))
        {
            string email = command[1];
            loundgeManager.RemoveNPCObject(email);
        }
        if (type.Equals(EventMessageType.UPDATEROOMSTATE.ToString()))
        {
            int roomNumber = int.Parse(command[1]);
            loundgeManager.UpdateRoomEnterence(roomNumber);
        }
        if (type.Equals(EventMessageType.VOICECHAT.ToString()))
        {
            string voiceEventType = command[1];
            string senderEmail = command[2];
            string recieverEmail = command[3];

            LoundgeUser sender = loundgeManager.GetLoundgeUser(senderEmail);
            LoundgeUser reciever = loundgeManager.GetLoundgeUser(recieverEmail);

            if (voiceEventType.Equals(VoiceEventType.CANCEL.ToString()))
            {
                if (recieverEmail.Equals(NetworkManager.User.email))
                {
                    voiceManager.OnVoiceChatCancelEvent(sender);
                }
                if (senderEmail.Equals(NetworkManager.User.email))
                {
                    voiceManager.OnVoiceChatCancelEvent(reciever);
                }

                //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                //loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(false);
                //loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(false);
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

                //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                //loundgeManager.spawnedNPCObject[senderEmail].GetComponent<NPCController>().OnVoiceChat(true);
                //loundgeManager.spawnedNPCObject[recieverEmail].GetComponent<NPCController>().OnVoiceChat(true);
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

                //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

            }
            if (voiceEventType.Equals(VoiceEventType.CONNECT.ToString()))
            {
                Debug.Log("Connect Voice Chat");
                if (recieverEmail.Equals(NetworkManager.User.email))
                {
                    Debug.Log("ConnectEvent Reciever: " + recieverEmail);
                    voiceManager.OnConnectVoiceManagerEvent(recieverEmail);
                    loundgeManager.JoinVoiceChatRoom(senderEmail);
                }
                if (senderEmail.Equals(NetworkManager.User.email))
                {
                    Debug.Log("ConnectEvent Sender: " + senderEmail);
                    voiceManager.OnConnectVoiceManagerEvent(senderEmail);
                    loundgeManager.JoinVoiceChatRoom(senderEmail);
                }

                //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = true;
                //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = true;

                GameObject spawnedNPC = null;
                if (loundgeManager.spawnedNPCObject.TryGetValue(senderEmail, out spawnedNPC))
                {
                    spawnedNPC.GetComponent<NPCController>().OnVoiceChat(true);
                }

                if (loundgeManager.spawnedNPCObject.TryGetValue(recieverEmail, out spawnedNPC))
                {
                    spawnedNPC.GetComponent<NPCController>().OnVoiceChat(true);
                }
            }
            if (voiceEventType.Equals(VoiceEventType.DISCONNECT.ToString()))
            {

                if (recieverEmail.Equals(NetworkManager.User.email))
                {
                    Debug.Log("DisconnectEvent RecieverEmail:" + recieverEmail);
                    loundgeManager.LeaveVoiceChatRoom();
                    voiceManager.OnDisconnectVoiceChatEvent(recieverEmail);
                }
                if (senderEmail.Equals(NetworkManager.User.email))
                {
                    Debug.Log("DisconnectEvent SenderEmail: " + senderEmail);
                    loundgeManager.LeaveVoiceChatRoom();
                    voiceManager.OnDisconnectVoiceChatEvent(senderEmail);

                }

                //loundgeManager.spawnedNPC[senderEmail].onVoiceChat = false;
                //loundgeManager.spawnedNPC[recieverEmail].onVoiceChat = false;

                GameObject spawnedNPC = null;
                if (loundgeManager.spawnedNPCObject.TryGetValue(senderEmail, out spawnedNPC))
                {
                    spawnedNPC.GetComponent<NPCController>().OnVoiceChat(false);
                }

                if (loundgeManager.spawnedNPCObject.TryGetValue(recieverEmail, out spawnedNPC))
                {
                    spawnedNPC.GetComponent<NPCController>().OnVoiceChat(false);
                }
            }
        }
    }

    #region To MasterClient

    int sendCount = 0;
    /// <summary>
    /// 마스터 클라언트에게 주기적으로 메세지를 보냄
    /// </summary>
    /// <param name="sender"></param>
    public void ResponseToMasterClient(string sender)
    {
        chatClient.SendPrivateMessage(masterChannel, "!") ;
    }
    private IEnumerator ResponseLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(responseIntervalToMaster);

        while(true)
        {
            yield return wait;
            Debug.Log("Responed to Master");
            ResponseToMasterClient(NetworkManager.User.email);
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
