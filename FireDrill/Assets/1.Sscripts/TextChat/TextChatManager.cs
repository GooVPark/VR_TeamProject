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

    private int colorNumber = 0;
    [SerializeField] private List<Color> colorList = new List<Color>();
    private void Start()
    {
        colorList.Add(Color.red);
        colorList.Add(Color.green);
        colorList.Add(Color.blue);
        //colorList.Add(Color.white);
        colorList.Add(Color.black);
        colorList.Add(Color.yellow);
        colorList.Add(Color.cyan);
        colorList.Add(Color.magenta);
        colorList.Add(Color.gray);
        colorList.Add(Color.grey);

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
        colorNumber = NetworkManager.User.characterNumber % 9;

        chatClient.Subscribe(new string[] { worldChat });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }
    public delegate void SendChatMessageEvent(string message);
    public static SendChatMessageEvent sendChatMessage;
    public void SendChatMessage(TMP_InputField inputField)
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        string message = inputField.text + "=" + colorNumber;

        chatClient.PublishMessage(worldChat, message);
        sendChatMessage?.Invoke(inputField.text);
    }
    public static string ColorToStr(Color color)
    {
        string r = ((int)(color.r * 255)).ToString("X2");
        string g = ((int)(color.g * 255)).ToString("X2");
        string b = ((int)(color.b * 255)).ToString("X2");
        string a = ((int)(color.a * 255)).ToString("X2");


        string result = string.Format("{0}{1}{2}{3}", r, g, b, a);


        return result;
    }
    public void OnSpawnNPC()
    {

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

            string[] command = messages[^1].ToString().Split('=');
            Debug.Log(messages[^1].ToString());
            Debug.Log(command[0]);
            Debug.Log(command[1]);
            string message = $"<color=#{ColorToStr(colorList[int.Parse(command[1])])}><b>{senders[^1]}</b>\n{command[0]}</color>\n";
            chatList[0].text += message;
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
        Debug.Log("On Subscribed");
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
