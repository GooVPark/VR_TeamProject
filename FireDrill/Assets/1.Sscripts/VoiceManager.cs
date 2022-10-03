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

    [Header("Voice Chat UI")]
    public VoiceChatRequestToast voiceChatRequestToast;
    public Toast acceptVoiceChatToast;
    public Toast deacceptVoiceChatToastSender;
    public Toast deacceptVoiceChatToastReciever;
    public ToastOneButton cancelVoiceChatToast;
    public Toast voiceChatCanceledToast;
    public Toast voiceChatDisconnectToast;

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

    private int senderID;
    private int recieverID;

    private VoiceContorller target;
    public NPCController localPlayer;

    public int viewID;

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
        groupText.text = recorder.InterestGroup.ToString();
    }

    public void Initialize(int viewID)
    {
        //this.viewID = viewID;
        //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[1] { (byte)viewID });
        //Recorder recorder = GetComponent<Recorder>();
        //recorder.InterestGroup = (byte)viewID;
    }

    public void OnVoiceChatSendEvent(int senderID, int recieverID)
    {
        localPlayer.SetVoiceState(VoiceChatState.Send);

        CurrentToast = cancelVoiceChatToast.gameObject;
        string email = PhotonNetwork.CurrentRoom.CustomProperties[recieverID.ToString()].ToString();
        cancelVoiceChatToast.message.text = $"{email}님에게 1:1 대화 요청";
        this.senderID = senderID;
        this.recieverID = recieverID;
    }

    public void OnVoiceChatRecieveEvent(int senderID, int recieverID)
    {
        localPlayer.SetVoiceState(VoiceChatState.Recieve);

        CurrentToast = voiceChatRequestToast.gameObject;
        string email = PhotonNetwork.CurrentRoom.CustomProperties[senderID.ToString()].ToString();
        voiceChatRequestToast.message.text = $"{email}님의 1:1 대화 요청";
        this.senderID = senderID;
        this.recieverID = recieverID;
    }

    public void OnVoiceChatCancelEvent(int viewID)
    {
        localPlayer.SetVoiceState(VoiceChatState.Off);

        CurrentToast = voiceChatCanceledToast.gameObject;
        string email = PhotonNetwork.CurrentRoom.CustomProperties[viewID.ToString()].ToString();
        voiceChatCanceledToast.message.text = $"{email}님과의 대화가 취소되었습니다";

        Invoke(nameof(CloseToast), 3f);
    }

    public void CancelVoiceChat()
    {
        GetVoiceController(senderID).CancelVoiceChat(recieverID);
        GetVoiceController(recieverID).CancelVoiceChat(senderID);
    }

    public void OnVoiceChatDisconnectEvent(int viewID)
    {
        Debug.Log("Disconnect Voice Chat");
        localPlayer.SetVoiceState(VoiceChatState.Off);
        recorder.TransmitEnabled = false;

        CurrentToast = voiceChatDisconnectToast.gameObject;
        string email = PhotonNetwork.CurrentRoom.CustomProperties[viewID.ToString()].ToString();
        voiceChatDisconnectToast.message.text = $"{email}님과의 대화가 종료되었습니다";

        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { 255 });
        groupText.text = (255).ToString();

        voiceChatToggleButton.onClick.RemoveAllListeners();

        Invoke(nameof(CloseToast), 3f);
    }

    public void DisconnectVoiceChat()
    {
        GetVoiceController(senderID).DisconnectVoiceChat(recieverID);
        GetVoiceController(recieverID).DisconnectVoiceChat(senderID);
    }


    public void AcceptVoiceChat()
    {
        ResponeVoiceChat(true);
    }

    public void DeaccpetVoiceChat()
    {
        ResponeVoiceChat(false);
    }

    public void ResponeVoiceChat(bool value)
    {
        CurrentToast = voiceChatRequestToast.gameObject;
        foreach (Transform player in playersTransform)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view.ViewID == senderID)
            {
                player.GetComponent<VoiceContorller>().OnResponePrivateVoiceChat(senderID, recieverID, value);
            }
        }

         CurrentToast = acceptVoiceChatToast.gameObject;
        if(value)
        {
            localPlayer.SetVoiceState(VoiceChatState.On);

            acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 수락";
            PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { (byte)senderID });
            groupText.text = ((byte)senderID).ToString();

            recorder.InterestGroup = (byte)senderID;
            recorder.TransmitEnabled = true;

            voiceChatToggleButton.onClick.AddListener(() => DisconnectVoiceChat());
        }
        else
        {
            localPlayer.SetVoiceState(VoiceChatState.Off);
            acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 거절";
        }

        Invoke(nameof(CloseToast), 3f);
    }

    public void OnPrivateVoiceChatRespone(int senderID, int recieverID, bool value)
    {
        CurrentToast = acceptVoiceChatToast.gameObject;
        if (value)
        {
            localPlayer.SetVoiceState(VoiceChatState.On);

            

            PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[] { }, new byte[] { (byte)senderID });
            groupText.text = ((byte)senderID).ToString();
            
            recorder.TransmitEnabled = true;
            acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 수락";

            voiceChatToggleButton.onClick.AddListener(() => DisconnectVoiceChat());
        }
        else
        {
            localPlayer.SetVoiceState(VoiceChatState.Off);
            acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 거절";
        }

        Invoke(nameof(CloseToast), 3f);
    }


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

    private void CloseToast()
    {
        CurrentToast = null;
    }
}
