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

    public delegate void PrivateVoiceChatCancelEvent(int viewID);
    public PrivateVoiceChatCancelEvent voiceChatCancelEvent;

    public delegate void PrivateVoiceChatDisconnectEvent(int viewID);
    public PrivateVoiceChatDisconnectEvent voiceChatDisconnectEvent;


    private NPCController npcController;

    public bool isVoiceChatReady = false;

    private void Start()
    {
        npcController = GetComponent<NPCController>();

        voiceChatRecieveEvent += VoiceManager.Instance.OnVoiceChatRecieveEvent;
        voiceChatSendEvent += VoiceManager.Instance.OnVoiceChatSendEvent;
        voiceChatResponeEvent += VoiceManager.Instance.OnPrivateVoiceChatRespone;
        voiceChatCancelEvent += VoiceManager.Instance.OnVoiceChatCancelEvent;
        voiceChatDisconnectEvent += VoiceManager.Instance.OnVoiceChatDisconnectEvent;
    }

    public void OnRequestPrivateVoiceChat()
    {
        if(!isVoiceChatReady || npcController.VoiceChatState != VoiceChatState.Off)
        {
            return;
        }
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

    public void CancelVoiceChat(int targetID)
    {
        photonView.RPC(nameof(CancelVoiceChatRPC), RpcTarget.All, targetID);
    }

    [PunRPC]
    private void CancelVoiceChatRPC(int targetID)
    {
        if(photonView.IsMine)
        {
            voiceChatCancelEvent?.Invoke(targetID);
        }
    }

    public void DisconnectVoiceChat(int targetID)
    {
        photonView.RPC(nameof(DisconnectVoiceChatRPC), RpcTarget.All, targetID);
    }

    [PunRPC]
    private void DisconnectVoiceChatRPC(int targetID)
    {
        if(photonView.IsMine)
        {
            voiceChatDisconnectEvent?.Invoke(targetID);
        }
    }
}
