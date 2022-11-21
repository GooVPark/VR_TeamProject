using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public static VoiceManager Instance;

    public delegate void EventMessage(string message);
    public static EventMessage eventMessage;

    [Header("Voice Chat UI")]
    public VoiceChatRequestToast voiceChatRequestToast;
    public Toast acceptVoiceChatToast;
    public Toast deacceptVoiceChatToastSender;
    public Toast deacceptVoiceChatToastReciever;
    public ToastOneButton cancelVoiceChatToast;
    public Toast voiceChatCanceledToast;
    public Toast voiceChatDisconnectToast;

    public ButtonInteractor acceptVoiceChatButton;
    public ButtonInteractor deacceptVoiceChatButton;

    public ButtonInteractor cancelVoiceChatButton;

    public TMP_Text groupText;

    public Button voiceChatToggleButton;

    private GameObject currentToast;
    public GameObject CurrentToast
    {
        set
        {
            if(currentToast != null)
            {
                currentToast.SetActive(false);
            }
            if(value != null)
            {
                currentToast = value;
                currentToast.SetActive(true);
            }
        }
    }

    public Transform playersTransform;

    [SerializeField] private Recorder recorder;

    [SerializeField] private LoundgeSceneManager loundgeSceneManager;

    private int senderID;
    private int recieverID;

    public LoundgeUser sender;
    public LoundgeUser reciever;

    private VoiceContorller target;
    public NPCController localPlayer;

    public int viewID;
    public bool onRequest = false;

    private Hashtable roomCustomProperties;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Start()
    {
        recorder = FindObjectOfType<Recorder>();
        //acceptVoiceChatButton.onClick += AcceptVoiceChat;
        //deacceptVoiceChatButton.onClick += DeaccpetVoiceChat;
        //cancelVoiceChatButton.onClick += CancelVoiceChat;
    }

    public void Initialize(int viewID)
    {
        //this.viewID = viewID;
        //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[1] { (byte)viewID });
        //Recorder recorder = GetComponent<Recorder>();
        //recorder.InterestGroup = (byte)viewID;
    }

    public void OnVoiceChatSendEvent(LoundgeUser sender, LoundgeUser reciever)
    {
        //localPlayer.SetVoiceState(VoiceChatState.Send);
        Debug.Log($"OnVoiceChatSendEvent - Sender: {sender.email}, Reciever: {reciever.email}");
        DataManager.Instance.SetUserOnRequest(sender.email, true);

        CurrentToast = cancelVoiceChatToast.gameObject;
        string userName = reciever.name;
        cancelVoiceChatToast.message.text = $"{userName}님의 대화 수락 요청을 기다리는 중...";

        this.sender = sender;
        this.reciever = reciever;
    }

    private IEnumerator CatchRequestOverlapException()
    {
        while(true)
        {
            yield return null;
        }
    }
    public void OnVoiceChatRecieveEvent(LoundgeUser sender, LoundgeUser reciever)
    {
        //localPlayer.SetVoiceState(VoiceChatState.Recieve);
        Debug.Log($"OnVoiceChatRecieveEvent - Sender: {sender.email}, Reciever: {reciever.email}");
        DataManager.Instance.SetUserOnRequest(reciever.email, true);
        CurrentToast = voiceChatRequestToast.gameObject;
        string userName = sender.name;
        voiceChatRequestToast.message.text = $"{userName}님이 대화 요청을 보냈습니다. 대화를 원하시면 YES, 아니면 NO를\n클릭해주세요.";

        this.sender = sender;
        this.reciever = reciever;
    }

    public void OnVoiceChatCancelEvent(LoundgeUser user)
    {
        //localPlayer.SetVoiceState(VoiceChatState.Off);
        Debug.Log($"OnVoiceChatCancleEvent - Sender: {sender.email}, Reciever: {reciever.email}");

        CurrentToast = voiceChatCanceledToast.gameObject;
        DataManager.Instance.SetUserOnRequest(user.email, false);
        string userName = user.name;
        voiceChatCanceledToast.message.text = $"{userName}님과의 대화가 취소되었습니다.";

        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.onVoiceChat = false;

        sender = null;
        reciever = null;
    }

    public void CancelVoiceChat()
    {
        Debug.Log($"CancelVoiceChat - Sender: {sender.email}, Reciever: {reciever.email}");
        string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.CANCEL}_{sender.email}_{reciever.email}";
        eventMessage?.Invoke(message);
    }

    public void OnVoiceChatDisconnectEvent(int viewID)
    {
        Debug.Log("Disconnect Voice Chat");
        //localPlayer.SetVoiceState(VoiceChatState.Off);
        recorder.TransmitEnabled = false;

        CurrentToast = voiceChatDisconnectToast.gameObject;
        string email = PhotonNetwork.CurrentRoom.CustomProperties[viewID.ToString()].ToString();
        voiceChatDisconnectToast.message.text = $"{email}님과의 대화가 종료되었습니다";

        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.onVoiceChat = false;

        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { 255 });
        groupText.text = (255).ToString();

        voiceChatToggleButton.onClick.RemoveAllListeners();

     
    }

    public void DisconnectVoiceChat()
    {
        string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.DISCONNECT}_{sender.email}_{reciever.email}";
        eventMessage?.Invoke(message);
    }
    public void AcceptVoiceChat()
    {
        string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.ACCEPT}_{sender.email}_{reciever.email}";
        eventMessage?.Invoke(message);
    }

    public void DeaccpetVoiceChat()
    {
        string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.DEACCEPT}_{sender.email}_{reciever.email}";
        eventMessage?.Invoke(message);
    }

    public void OnAcceptVoiceChatEventSender(LoundgeUser sender, LoundgeUser reciever)
    {
        CurrentToast = acceptVoiceChatToast.gameObject;
        acceptVoiceChatToast.message.text = $"{reciever.name}님과의 1:1 대화가 수락 되었습니다.";
        DataManager.Instance.SetUserOnRequest(sender.email, false);
        DataManager.Instance.UpdateLobbyUser(sender);
        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.onVoiceChat = true;

        string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.CONNECT}_{sender.email}_{reciever.email}";
        eventMessage?.Invoke(message);

    }
    public void OnAcceptVoiceChatEventReciever(LoundgeUser sender, LoundgeUser reciever)
    {
        CurrentToast = acceptVoiceChatToast.gameObject;
        acceptVoiceChatToast.message.text = $"{sender.name}님과의 1:1 대화가 수락 되었습니다.";

        DataManager.Instance.SetUserOnRequest(reciever.email, false);

        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.onVoiceChat = true;

        reciever.onVoiceChat = true;

        DataManager.Instance.UpdateLobbyUser(reciever);

        //string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.CONNECT}_{sender.email}_{reciever.email}";
        //eventMessage?.Invoke(message);

    }

    public void OnDeacceptVoiceChatEventSender(LoundgeUser sender, LoundgeUser reciever)
    {
        CurrentToast = deacceptVoiceChatToastSender.gameObject;
        DataManager.Instance.SetUserOnRequest(sender.email, false);
        deacceptVoiceChatToastSender.message.text = $"{reciever.name}님과의 1:1 대화가 거절 되었습니다.";
    }
    public void OnDeacceptVoiceChatEventReciever(LoundgeUser sender, LoundgeUser reciever)
    {
        CurrentToast = deacceptVoiceChatToastReciever.gameObject;
        DataManager.Instance.SetUserOnRequest(reciever.email, false);
        deacceptVoiceChatToastReciever.message.text = $"{sender.name}님과의 1:1 대화가 거절 되었습니다.";
    }

    public void OnConnectVoiceManagerEvent(string userEmail)
    {
        LoundgeUser user;
        if (userEmail.Equals(sender.email))
        {
            user = sender;
        }
        else if (userEmail.Equals(reciever.email))
        {
            user = reciever;
        }
        else user = null;

        CurrentToast = acceptVoiceChatToast.gameObject;
        user.onVoiceChat = true;

        DataManager.Instance.UpdateLobbyUser(user);
    }

    public void OnDisconnectVoiceChatEvent(string userEmail)
    {
        LoundgeUser user;
        if (userEmail.Equals(sender.email))
        {
            user = sender;
        }
        else if (userEmail.Equals(reciever.email))
        {
            user = reciever;
        }
        else user = null;

        CurrentToast = voiceChatDisconnectToast.gameObject;
        user.onVoiceChat = false;

        DataManager.Instance.UpdateLobbyUser(user);
    }

    //public void ResponeVoiceChat(bool value)
    //{
    //    CurrentToast = voiceChatRequestToast.gameObject;
    //    foreach (Transform player in playersTransform)
    //    {
    //        PhotonView view = player.GetComponent<PhotonView>();
    //        if (view.ViewID == senderID)
    //        {
    //            player.GetComponent<VoiceContorller>().OnResponePrivateVoiceChat(senderID, recieverID, value);
    //        }
    //    }

    //     CurrentToast = acceptVoiceChatToast.gameObject;
    //    if(value)
    //    {
    //        //localPlayer.SetVoiceState(VoiceChatState.On);

    //        acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 수락";
    //        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { (byte)senderID });
    //        groupText.text = ((byte)senderID).ToString();

    //        recorder.InterestGroup = (byte)senderID;
    //        recorder.TransmitEnabled = true;

    //        voiceChatToggleButton.onClick.AddListener(() => DisconnectVoiceChat());
    //    }
    //    else
    //    {
    //        //localPlayer.SetVoiceState(VoiceChatState.Off);
    //        acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 거절";
    //    }

    //    Invoke(nameof(CloseToast), 3f);
    //}

    //public void OnPrivateVoiceChatRespone(int senderID, int recieverID, bool value)
    //{
    //    CurrentToast = acceptVoiceChatToast.gameObject;
    //    if (value)
    //    {
    //        //localPlayer.SetVoiceState(VoiceChatState.On);



    //        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { (byte)senderID });
    //        groupText.text = ((byte)senderID).ToString();

    //        recorder.TransmitEnabled = true;
    //        acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 수락";

    //        voiceChatToggleButton.onClick.AddListener(() => DisconnectVoiceChat());
    //    }
    //    else
    //    {
    //        //localPlayer.SetVoiceState(VoiceChatState.Off);
    //        acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 거절";
    //    }

    //    Invoke(nameof(CloseToast), 3f);
    //}


    public int GetSenderID()
    {
        foreach(Transform player in playersTransform)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view.IsMine)
            {
                return view.ViewID;
            }
        }

        return -1;
    }

    public VoiceContorller GetVoiceController(int viewID)
    {
        foreach(Transform player in playersTransform)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if(view.ViewID == viewID)
            {
                return player.GetComponent<VoiceContorller>();
            }
        }

        return null;
    }

    private void OnApplicationQuit()
    {
        DisconnectVoiceChat();
    }
}
