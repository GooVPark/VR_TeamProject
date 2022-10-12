using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public enum VoiceChatState { On, Off, Send, Recieve }

public class VoiceContorller : MonoBehaviour
{
    public delegate void PrivateVoiceChatRequestEvent(LoundgeUser sender, LoundgeUser reciever);
    public PrivateVoiceChatRequestEvent voiceChatSendEvent;
    public PrivateVoiceChatRequestEvent voiceChatRecieveEvent;

    public delegate void PrivateVoiceChatResponeEvent(int senderID, int recieverID, bool value);
    public PrivateVoiceChatResponeEvent voiceChatResponeEvent;

    public delegate void PrivateVoiceChatCancelEvent(int viewID);
    public PrivateVoiceChatCancelEvent voiceChatCancelEvent;

    public delegate void PrivateVoiceChatDisconnectEvent(int viewID);
    public PrivateVoiceChatDisconnectEvent voiceChatDisconnectEvent;

    public delegate void EventMessage(string message);
    public static EventMessage eventMessage;

    private NPCController npcController;

    public bool isVoiceChatReady = false;

    private void Start()
    {
        npcController = GetComponent<NPCController>();

        voiceChatRecieveEvent += VoiceManager.Instance.OnVoiceChatRecieveEvent;
        voiceChatSendEvent += VoiceManager.Instance.OnVoiceChatSendEvent;
        voiceChatResponeEvent += VoiceManager.Instance.OnPrivateVoiceChatRespone;
        
        voiceChatDisconnectEvent += VoiceManager.Instance.OnVoiceChatDisconnectEvent;
    }

    public void OnRequestPrivateVoiceChat()
    {
        string message = $"{EventMessageType.VOICECHAT}_{NetworkManager.User.email}_{npcController.user.email}";
        eventMessage?.Invoke(message);
    }

    public void OnResponePrivateVoiceChat(int senderID, int recieverID, bool value)
    {
     
    }

    public void CancelVoiceChat(int targetID)
    {
      
    }

    public void DisconnectVoiceChat(int targetID)
    {
        
    }
}
