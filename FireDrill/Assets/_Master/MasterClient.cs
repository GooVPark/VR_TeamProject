using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class MasterClient : MonoBehaviour, IChatClientListener
{
    [System.Serializable]
    public class MessageWrapper
    {
        public string sender;
        public object message;
        public string channelName;

        public MessageWrapper(string sender, object message, string channelName)
        {
            this.sender = sender;
            this.message = message;
            this.channelName = channelName;
        }
    }

    [System.Serializable]
    public class User
    {
        public string userName;
        public float userLife;
        public const float MAX_LIFE = 10f;

        public User(string userName)
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

    public List<User> alivingUsers;
    public Dictionary<string, User> alivingUsersDict;
    public List<MessageWrapper> messageQueue;

    private ChatClient chatClient;

    // Start is called before the first frame update
    void Start()
    {
        messageQueue = new List<MessageWrapper>();
        alivingUsers = new List<User>();
        alivingUsersDict = new Dictionary<string, User>();



        PhotonNetwork.LocalPlayer.NickName = "_MASTER_";

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();



        Connect();
    }


    private float serviceTimer;

    void Update()
    {
        if (messageQueue.Count > 0)
        {
            MessageWrapper m = messageQueue[0];
            messageQueue.RemoveAt(0);
            HandlePrivateMessage(m.sender, m.message, m.channelName);
        }

        CheckAliving();


        // 0.05초에 한 번씩 서비스하도록. 초당20회.
        serviceTimer -= Time.unscaledDeltaTime;
        if (serviceTimer < 0)
        {
            if (chatClient != null)
                chatClient.Service();
            serviceTimer = 0.05f;
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (sender.Equals("_MASTER_"))
            return;
        messageQueue.Add(new MessageWrapper(sender, message, channelName));
    }

    public void HandlePrivateMessage(string sender, object messageOb, string channelName)
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

        if (message[0] == '!')
        {
            // MasterLogger.Log("느낌표다! : " + sender);
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
            for (int i = 0; i < alivingUsers.Count; i++)
                chatClient.SendPrivateMessage(alivingUsers[i].userName, message);
        }
    }





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


    public void OnUserTimeout(User user)
    {
        MasterLogger.Good("User Timeout: " + user.userName);

        alivingUsers.Remove(user);
        alivingUsersDict.Remove(user.userName);

        string msg = $"{EventMessageType.OUT}_{user.userName}";
        for (int i = 0; i < alivingUsers.Count; i++)
            chatClient.SendPrivateMessage(alivingUsers[i].userName, msg);
    }

    public void AddUser(string userName)
    {
        if (alivingUsersDict.ContainsKey(userName))
        {
            MasterLogger.Error("User <" + userName + "> is aleady exists!!!");
            return;
        }
        MasterLogger.Good("새로운 유저: <" + userName + ">");
        User user = new User(userName);
        alivingUsers.Add(user);
        alivingUsersDict.Add(userName, user);
    }


    







    public void Connect()
    {
        Application.runInBackground = true;

        chatClient = new ChatClient(this)
        {
            UseBackgroundWorkerForSending = true
        };

        MasterLogger.Log("AppIdChat: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat + ", AppVersion: " + PhotonNetwork.AppVersion);
        MasterLogger.Log("GameVersion: " + PhotonNetwork.GameVersion);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues("_MASTER_"));

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

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        MasterLogger.Log("OnGetMessages: " + senders[^1] + ", message: " + messages[^1]);
    }

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
}
