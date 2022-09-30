using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class VoiceManager : MonoBehaviourPun
{
    public static VoiceManager Instance;

    [Header("Voice Chat UI")]
    public VoiceChatRequestToast voiceChatRequestToast;
    public Toast acceptVoiceChatToast;
    public Toast deacceptVoiceChatToastSender;
    public Toast deacceptVoiceChatToastReciever;

    public Transform playersTransform;

    private int senderID;
    private int recieverID;

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
        acceptVoiceChatToast.gameObject.SetActive(true);
        string email = PhotonNetwork.CurrentRoom.CustomProperties[recieverID.ToString()].ToString();
        acceptVoiceChatToast.message.text = $"{email}님에게 1:1 대화 요청";
    }

    public void OnVoiceChatRecieveEvent(int senderID, int recieverID)
    {
        voiceChatRequestToast.gameObject.SetActive(true);
        string email = PhotonNetwork.CurrentRoom.CustomProperties[senderID.ToString()].ToString();
        voiceChatRequestToast.message.text = $"{email}님의 1:1 대화 요청";
        this.senderID = senderID;
        this.recieverID = recieverID;
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
        voiceChatRequestToast.gameObject.SetActive(false);
        foreach (Transform player in playersTransform)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view.ViewID == senderID)
            {
                player.GetComponent<VoiceContorller>().OnResponePrivateVoiceChat(senderID, recieverID, value);
            }
        }

        acceptVoiceChatToast.gameObject.SetActive(true);
        if(value)
        {
            acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 수락";
            PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[1] { (byte)recieverID }, new byte[1] { (byte)senderID });
            Recorder recorder = GetComponent<Recorder>();
            recorder.InterestGroup = (byte)senderID;
        }
        else
        {
            acceptVoiceChatToast.message.text = $"{senderID}님과의 1 : 1 대화 거절";
        }
    }

    public void OnPrivateVoiceChatRespone(int senderID, int recieverID, bool value)
    {
        acceptVoiceChatToast.gameObject.SetActive(true);
        if (value)
        {
            acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 수락";
        }
        else
        {
            acceptVoiceChatToast.message.text = $"{recieverID}님이 1 : 1 대화 거절";
        }
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
}
