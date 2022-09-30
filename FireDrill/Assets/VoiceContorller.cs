using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public enum VoiceChatState { On, Off, Send, Recieve }

public class VoiceContorller : MonoBehaviourPun
{
    public delegate void PrivateVoiceChatRequestEvent(int senderID, int recieverID);
    public PrivateVoiceChatRequestEvent voiceChatSendEvent;
    public PrivateVoiceChatRequestEvent voiceChatRecieveEvent;

    public delegate void PrivateVoiceChatResponeEvent(int senderID, int recieverID, bool value);
    public PrivateVoiceChatResponeEvent voiceChatResponeEvent;

    private NPCController npcController;

    private void Start()
    {
        npcController = GetComponent<NPCController>();

        voiceChatRecieveEvent += VoiceManager.Instance.OnVoiceChatRecieveEvent;
        voiceChatSendEvent += VoiceManager.Instance.OnVoiceChatSendEvent;
        voiceChatResponeEvent += VoiceManager.Instance.OnPrivateVoiceChatRespone;
    }

    public void OnRequestPrivateVoiceChat()
    {
        //if(npcController.VoiceChatState != VoiceChatState.Off)
        //{
        //    return;
        //}

        //npcController.VoiceChatState = VoiceChatState.Recieve;

        int senderID = VoiceManager.Instance.GetSenderID();
        int recieverID = photonView.ViewID;

        voiceChatSendEvent?.Invoke(senderID, recieverID);
        photonView.RPC(nameof(OnRequestPrivateVoiceChatRPC), RpcTarget.All, senderID, recieverID);
    }

    [PunRPC]
    private void OnRequestPrivateVoiceChatRPC(int senderID, int receverID)
    {
        if(photonView.IsMine)
        {
            voiceChatRecieveEvent?.Invoke(senderID, receverID);
        }
    }

    public void OnResponePrivateVoiceChat(int senderID, int recieverID, bool value)
    {
        photonView.RPC(nameof(OnResponePrivateVoiceChatRPC), RpcTarget.All, senderID, recieverID, value);
    }

    [PunRPC]
    private void OnResponePrivateVoiceChatRPC(int senderID, int recieverID, bool value)
    {
        if(photonView.IsMine)
        {
            voiceChatResponeEvent?.Invoke(senderID, recieverID, value);
        }
    }
}
