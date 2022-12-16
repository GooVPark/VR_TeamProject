using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using MongoDB.Bson;
using MongoDB.Driver;

/// <summary>
/// ������ Ŭ���̾�Ʈ
/// 
/// ������ ����ȭ �� �ߺ� �̺�Ʈ ó��
/// </summary>

public class MasterClient : MonoBehaviour, IChatClientListener
{
    public enum EventType { Send, Recieve }

    public enum VoiceState { ON, OFF, CANCEL, ACCEPT, DEACCEPT, REQUEST }
    [System.Serializable]
    public class MessageWrapper
    {
        public string sender;
        public object message;
        public string channelName;
        public EventType EventType;

        public MessageWrapper(string sender, object message, string channelName, EventType eventType)
        {
            this.sender = sender;
            this.message = message;
            this.channelName = channelName;
            this.EventType = eventType;
        }
    }

    [System.Serializable]
    public class OnlineUser
    {
        public string userName;
        public float userLife;
        public const float MAX_LIFE = 10f;

        public string voiceTarget;
        public string sender;
        public string reciever;

        public bool onRequest = false;
        public bool onVoiceChat = false;

        public VoiceState voiceState = VoiceState.OFF;

        public OnlineUser(string userName)
        {
            this.userName = userName;
            UpdateUserLife();
        }

        public void UpdateUserLife()
        {
            userLife = MAX_LIFE;
        }
    }


    [SerializeField] private string eventServer;

    public List<OnlineUser> alivingUsers;
    public Dictionary<string, OnlineUser> alivingUsersDict;
    public List<MessageWrapper> messageQueue;

    private ChatClient chatClient;
    private bool isOnline = false;

    MongoClient client;

    IMongoDatabase serverSettingDatabase;
    IMongoCollection<ServerSetting> serverSettingCollection;

    IMongoDatabase userAccountDatabase;
    IMongoCollection<User> accountCollection;

    IMongoDatabase roomDatabase;
    IMongoCollection<RoomData> roomCollection;
    IMongoCollection<RoomUser> roomUserCollection;


    IMongoDatabase lobbyDatabase;
    IMongoCollection<LoundgeUser> loundgeUsercollection;

    // Start is called before the first frame update
    void Start()
    {
        messageQueue = new List<MessageWrapper>();
        alivingUsers = new List<OnlineUser>();
        alivingUsersDict = new Dictionary<string, OnlineUser>();



        PhotonNetwork.LocalPlayer.NickName = "_MASTER_";

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();

        client = new MongoClient("mongodb+srv://firedrillMember:member11@cluster0.pt8thqp.mongodb.net/?retryWrites=true&w=majority");

        userAccountDatabase = client.GetDatabase("UserAccount");
        accountCollection = userAccountDatabase.GetCollection<User>("UserAccounts");
        roomDatabase = client.GetDatabase("RoomDatabase");
        roomUserCollection = roomDatabase.GetCollection<RoomUser>("Room1");
        roomCollection = roomDatabase.GetCollection<RoomData>("RoomInfo");
        
        lobbyDatabase = client.GetDatabase("LobbyData");
        loundgeUsercollection = lobbyDatabase.GetCollection<LoundgeUser>("Loundge");

        serverSettingDatabase = client.GetDatabase("ServeSetting");
        serverSettingCollection = serverSettingDatabase.GetCollection<ServerSetting>("Setting");

        Connect();

    }


    private float serviceTimer;

    void FixedUpdate()
    {
        //������ Online�̸� �� �����Ӹ��� �÷��̾�鿡�� �̺�Ʈ ����
        if (isOnline == true)
        {
            if (messageQueue.Count > 0)
            {
                MessageWrapper m = messageQueue[0];
                messageQueue.RemoveAt(0);
                HandlePrivateMessage(m.sender, m.message, m.channelName, m.EventType);
            }

            CheckAliving();

            chatClient.Service();
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (sender.Equals("_MASTER_"))
        {
            return;
        }
        messageQueue.Add(new MessageWrapper(sender, message, channelName, EventType.Recieve));
    }

    //������ �̺�Ʈ �޼����� ó���ϰ� �ٽ� �۽���
    public void HandlePrivateMessage(string sender, object messageOb, string channelName, EventType eventType)
    {
        /*if (!channelName.Equals(eventServer))
            return;

        ChatChannel channel = null;
        bool isFound = chatClient.TryGetChannel(eventServer, out channel);
        if (!isFound)
        {
            MasterLogger.Error("Channel not found : " + eventServer);
            return;
        }*/

        string message = messageOb.ToString().Trim();

        if (eventType == EventType.Recieve)
        {
            if (message[0] == '!')
            {
                // MasterLogger.Log("����ǥ��! : " + sender);
                if (!alivingUsersDict.ContainsKey(sender))
                {
                    AddUser(sender);
                }
                alivingUsersDict[sender].UpdateUserLife();
            }
            else
            {
                MasterLogger.Log($"({sender}) HANDLE: {message}");
                // to all user
                //for (int i = 0; i < alivingUsers.Count; i++)
                //    chatClient.SendPrivateMessage(alivingUsers[i].userName, message);
                string[] command = message.Split('_');
                //sender�� reciever�� reciever�� sender�� ������ return
                if (command[0].Equals(EventMessageType.VOICECHAT.ToString()))
                {
                    if (command[1].Equals(VoiceEventType.REQUEST.ToString()))
                    {
                        string senderEmail = command[2];
                        string recieverEmail = command[3];

                        VoiceChatRequestOverlap(senderEmail, recieverEmail, message);
                    }
                    else if (command[1].Equals(VoiceEventType.CANCEL.ToString()))
                    {
                        string senderEmail = command[2];
                        string recieverEmail = command[3];

                        VoiceChatCancelOverlap(senderEmail, recieverEmail, message);
                    }
                    else if (command[1].Equals(VoiceEventType.ACCEPT.ToString()))
                    {
                        string senderEmail = command[2];
                        string recieverEmail = command[3];

                        VoiceChatAccept(senderEmail, recieverEmail, message);
                    }
                    else if (command[1].Equals(VoiceEventType.DEACCEPT.ToString()))
                    {
                        string senderEmail = command[2];
                        string recieverEmail = command[3];

                        VoiceChatDeaccept(senderEmail, recieverEmail, message);
                    }
                    else if (command[1].Equals(VoiceEventType.DISCONNECT.ToString()))
                    {
                        string senderEmail = command[2];
                        string recieverEmail = command[3];

                        VoiceChatDisconnect(senderEmail, recieverEmail, message);
                    }
                    else
                    {
                        chatClient.PublishMessage(eventServer, message);
                    }
                }
                else
                {
                    chatClient.PublishMessage(eventServer, message);
                }
            }
        }
        else
        {
            chatClient.PublishMessage(eventServer, message);
        }
    }

    //�÷��̾ �����Է� ����
    #region Voice Chat Event Overlap

    private void VoiceChatRequestOverlap(string senderEmail, string recieverEmail, string message)
    {
        if(string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(recieverEmail))
        {
            return;
        }

        OnlineUser sender = alivingUsersDict[senderEmail];
        OnlineUser reciever = alivingUsersDict[recieverEmail];

        if(reciever.voiceState != VoiceState.OFF)
        {
            return;
        }
        else
        {
            sender.voiceState = VoiceState.REQUEST;
            sender.voiceTarget = recieverEmail;
            reciever.voiceState = VoiceState.REQUEST;
            reciever.voiceTarget = senderEmail;

            //chatClient.PublishMessage(eventServer, message);
            messageQueue.Add(new MessageWrapper(senderEmail, message, string.Empty, EventType.Send));
        }
    }

    private void VoiceChatCancelOverlap(string senderEmail, string recieverEmail, string message)
    {
        OnlineUser sender = alivingUsersDict[senderEmail];
        OnlineUser reciever = alivingUsersDict[recieverEmail];
        
        if(reciever.voiceState != VoiceState.REQUEST)
        {
            return;
        }
        else
        {
            reciever.voiceState = VoiceState.OFF;
            sender.voiceState = VoiceState.OFF;

            reciever.voiceTarget = string.Empty;
            sender.voiceTarget = string.Empty;

            //chatClient.PublishMessage(eventServer, message);
            messageQueue.Add(new MessageWrapper(senderEmail, message, string.Empty, EventType.Send));
        }
    }

    private void VoiceChatAccept(string senderEmail, string recieverEmail, string message)
    {
        OnlineUser sender = alivingUsersDict[senderEmail];
        OnlineUser reciever = alivingUsersDict[recieverEmail];

        if (sender.voiceState != VoiceState.REQUEST)
        {
            return;
        }
        else
        {
            sender.voiceState = VoiceState.ON;
            reciever.voiceState = VoiceState.ON;

            //chatClient.PublishMessage(eventServer, message);
            messageQueue.Add(new MessageWrapper(senderEmail, message, string.Empty, EventType.Send));
        }
    }

    private void VoiceChatDeaccept(string senderEmail, string recieverEmail, string message)
    {
        OnlineUser sender = alivingUsersDict[senderEmail];
        OnlineUser reciever = alivingUsersDict[recieverEmail];

        if (sender.voiceState != VoiceState.REQUEST)
        {
            return;
        }
        else
        {
            sender.voiceState = VoiceState.OFF;
            reciever.voiceState = VoiceState.OFF;

            sender.voiceTarget = string.Empty;
            reciever.voiceTarget = string.Empty;

            //chatClient.PublishMessage(eventServer, message);       
            messageQueue.Add(new MessageWrapper(senderEmail, message, string.Empty, EventType.Send));
        }
    }

    private void VoiceChatDisconnect(string senderEmail, string recieverEmail, string message)
    {
        OnlineUser sender = alivingUsersDict[senderEmail];
        OnlineUser reciever = alivingUsersDict[recieverEmail];

        sender.voiceState = VoiceState.OFF;
        reciever.voiceState = VoiceState.OFF;

        sender.voiceTarget = string.Empty;
        reciever.voiceTarget = string.Empty;

        //chatClient.PublishMessage(eventServer, message);      
        messageQueue.Add(new MessageWrapper(senderEmail, message, string.Empty, EventType.Send));
    }

    #endregion

    //������ ������ �÷��̾ �˻�
    #region Player TimeOut Control

    private void CheckAliving()
    {
        float deltaTime = Time.unscaledDeltaTime;
        for (int i = 0; i < alivingUsers.Count; i++)
        {
            alivingUsers[i].userLife -= deltaTime;
            if (alivingUsers[i].userLife < 0)
                OnUserTimeout(alivingUsers[i]);
        }
    }

    //�����ð� �̻� ������ ���� �÷��̾�� ������ �����ٰ� �Ǵ��ϰ� �ٸ� �÷��̾�鿡�Լ� �Ⱥ��̰� ó����
    public void OnUserTimeout(OnlineUser user)
    {
        MasterLogger.Good("User Timeout: " + user.userName);

        alivingUsers.Remove(user);
        alivingUsersDict.Remove(user.userName);

        var filterLoundge = Builders<LoundgeUser>.Filter.Eq("email", user.userName);
        var filterRoom = Builders<RoomUser>.Filter.Eq("email", user.userName);
        var filterUser = Builders<User>.Filter.Eq("email", user.userName);

        User targetUser = accountCollection.Find(filterUser).ToList()[0];

        if(targetUser.currentRoom == 999)
        {
            loundgeUsercollection.DeleteOne(filterLoundge);
        }
        else
        {
            roomUserCollection.DeleteOne(filterRoom);
        }

        var update = Builders<User>.Update.Set("isOnline" , false);
        accountCollection.UpdateOne(filterUser, update);

        string msg = $"{EventMessageType.OUT}_{user.userName}";
        for (int i = 0; i < alivingUsers.Count; i++)
            chatClient.SendPrivateMessage(alivingUsers[i].userName, msg);
    }

    #endregion

    public void AddUser(string userName)
    {
        if (alivingUsersDict.ContainsKey(userName))
        {
            MasterLogger.Error("User <" + userName + "> is aleady exists!!!");
            return;
        }
        MasterLogger.Good("���ο� ����: <" + userName + ">");
        OnlineUser user = new OnlineUser(userName);
        alivingUsers.Add(user);
        alivingUsersDict.Add(userName, user);
    }

    public void Connect()
    {
        if (IsOnline())
        {
            MasterLogger.Log("Server is already Online");
        }
        else
        {
            SetOnline(true);
            isOnline = true;
            Application.runInBackground = true;

            chatClient = new ChatClient(this)
            {
                UseBackgroundWorkerForSending = true
            };


            MasterLogger.Log("AppIdChat: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat + ", AppVersion: " + PhotonNetwork.AppVersion);
            MasterLogger.Log("GameVersion: " + PhotonNetwork.GameVersion);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues("_MASTER_"));

            StartCoroutine(UpdateUserCount());
        }
    }

    public void Disconnect()
    {
        chatClient.Disconnect();
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnDisconnected()
    {

    }

    public void OnConnected()
    {
        MasterLogger.Log("Connected");
        chatClient.Subscribe(new string[] { eventServer });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        //LoundgeSceneManager.Instance.isEventServerConnected = true;
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    //���� �̺�Ʈ �޼����� �α�
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (senders[^1].Equals("_MASTER_"))
        {
            return;
        }
        MasterLogger.Log("OnGetMessages: " + senders[^1] + ", message: " + messages[^1]);
    }

    //ä�ü��� ����
    public void OnSubscribed(string[] channels, bool[] results)
    {
        MasterLogger.Log("OnSubscribed" + channels + ", " + results);
        for (int i = 0; i < channels.Length; i++)
            MasterLogger.Log(channels[i]);
        for (int i = 0; i < results.Length; i++)
            MasterLogger.Log(results[i].ToString());



        MasterLogger.Good("--------------------------------------------");
        MasterLogger.Good(" Master Started");
        MasterLogger.Good("--------------------------------------------");
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }

    //���� �ð����� ��� ������� ���� �� ����
    private IEnumerator UpdateUserCount()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while(true)
        {
            string message = $"{EventMessageType.UPDATEUSERCOUNT}_{GetLoundgeUserCount()}_{GetRoomUserCount(0)}";
            chatClient.PublishMessage(eventServer, message);

            yield return wait;
        }
    }

    //������� ���� �� ��ȯ
    public int GetLoundgeUserCount()
    {
        var filter = Builders<LoundgeUser>.Filter.Empty;
        return (int)loundgeUsercollection.CountDocuments(FilterDefinition<LoundgeUser>.Empty);
    }

    //���� ���� �� ��ȯ
    public int GetRoomUserCount(int roomNumber)
    {
        int userCounts = 0;
        switch (roomNumber)
        {
            case 0:
                userCounts = (int)roomUserCollection.CountDocuments(FilterDefinition<RoomUser>.Empty);
                break;
        }

        //Debug.Log($"Room Number: {roomNumber}\nUser Counts: {userCounts}");
        return userCounts;
    }

    #region ServerSetting

    //�ٸ� ������ Ŭ���̾�Ʈ�� �����ִ����� �˻���
    public bool IsOnline()
    {
        var filter = Builders<ServerSetting>.Filter.Empty;
        var setting = serverSettingCollection.Find(filter);
        if (setting == null)
        {
            return false;
        }
        //Debug.Log(setting.ToList().Count);
        return setting.ToList()[0].isOnline;
    }

    //�ٸ� ���� ������ Ŭ���̾�Ʈ�� ���ٸ� ������ Online���·� ����
    public void SetOnline(bool value)
    {
        var filter = Builders<ServerSetting>.Filter.Empty;
        var update = Builders<ServerSetting>.Update.Set("isOnline", value);

        serverSettingCollection.UpdateOne(filter, update);
    }

    [ContextMenu("�������� ����")]
    public void InsertServerSetting()
    {
        serverSettingCollection.InsertOne(new ServerSetting());
    }

    #endregion

    //������ Ŭ���̾�Ʈ ����� ȣ�� - ��, ������� �����ִ� ���� ó�� �� ����� ���� �ʱ�ȭ
    private void CloseServer()
    {
        string message = EventMessageType.SERVERDOWN.ToString() + "_FROMMASTER";
        chatClient.PublishMessage(eventServer, message);

        var filter = Builders<User>.Filter.Empty;
        var updateOnline = Builders<User>.Update.Set("isOnline", false);
        var updateRoom = Builders<User>.Update.Set("currentRoom", 999);

        accountCollection.UpdateMany(filter, updateOnline);
        accountCollection.UpdateMany(filter, updateRoom);

        var roomFilter = Builders<RoomData>.Filter.Empty;
        var updateRoomProgress = Builders<RoomData>.Update.Set("progress", 0);
        var updateRoomState = Builders<RoomData>.Update.Set("isStarted", false);

        roomCollection.UpdateMany(roomFilter, updateRoomProgress);
        roomCollection.UpdateMany(roomFilter, updateRoomState);

        var loundgeUserFilter = Builders<LoundgeUser>.Filter.Empty;

        loundgeUsercollection.DeleteMany(loundgeUserFilter);

        var roomUserFilter = Builders<RoomUser>.Filter.Empty;

        roomUserCollection.DeleteMany(roomUserFilter);
    }

    private void OnApplicationQuit()
    {
        if (isOnline)
        {
            CloseServer();
        }
        isOnline = false;
        SetOnline(false);

        foreach(var user in alivingUsers)
        {
            OnUserTimeout(user);
        }
    }
}
