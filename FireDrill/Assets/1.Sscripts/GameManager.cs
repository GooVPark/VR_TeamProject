using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Recorder localRecoder;

    public Transform playerTransforms;
    public NetworkPlayer player;

    public VoiceChatRequestToast voiceChatRequestToast;
    public TextChatManager textChatManager;

    [SerializeField] private ActionBasedController hapticTargetController;

    protected void Initialize()
    {
        localRecoder = FindObjectOfType<Recorder>();
        //SpawnPlayer();
        NetworkManager.ChatCallback += UpdateChat;


        switch (NetworkManager.User.userType)
        {
            case UserType.Lecture:
                megaphoneButton.gameObject.SetActive(true);
                scoreboardButton.gameObject.SetActive(true);
                break;
            case UserType.Student:
                megaphoneButton.gameObject.SetActive(false);
                scoreboardButton.gameObject.SetActive(false);
                break;
        }

    }

    protected Vector3 SpawnPlayer(Vector3 pivot)
    {
        Vector2 randomPosition = Random.insideUnitCircle * 3f;
        Vector3 spawnPosition = new Vector3(randomPosition.x, 0, randomPosition.y) + pivot;
        GameObject playerObject = PhotonNetwork.Instantiate("Player", spawnPosition, transform.rotation);
        player = playerObject.GetComponent<NetworkPlayer>();

        return spawnPosition;
    }

    #region Camera UI

    [Header("Camera UI")]
    public ButtonStateHandler megaphoneButton;
    public ButtonStateHandler scoreboardButton;
    public ButtonStateHandler voiceChatButton;
    public ButtonStateHandler textChatButton;

    public Button joinRoomButton;
    #endregion

    #region Megaphone

    private bool onMegaphone = false;
    private bool onVoice = false;

    public void MegaphoneOn()
    {
        onMegaphone = true;
        localRecoder.TransmitEnabled = true;
        
        player.MegaphoneOn();

        NetworkManager.Instance.onMegaphone = true;
        //megaphoneButton.UpdateState(ButtonState.Activate);

        //Haptic(0.5f, 0.1f);
    }

    public void MegaphoneOff()
    {
        onMegaphone = false;
        localRecoder.TransmitEnabled = false;

        player.MegaphoneOff();

        NetworkManager.Instance.onMegaphone = false;
        //megaphoneButton.UpdateState(ButtonState.Deactivate);

        //Haptic(0.5f, 0.1f);
    }

    public void MegaphoneToggle()
    {
        if(onVoice)
        {
            DeactivateVoiceChat();
        }
        if(onMegaphone)
        {
            MegaphoneOff();
        }
        else
        {
            MegaphoneOn();
        }
    }

    #endregion

    #region Voice Chat

    private int voiceTargetID;
    private Transform voiceChatTarget;

    public Toast voiceChatAcceptToast;
    public Toast voiceChatDeacceptToast;

    public void ToggleVoiceChat()
    {
        if(onMegaphone)
        {
            MegaphoneOff();
        }
        if(onVoice)
        {
            DeactivateVoiceChat();
        }
        else
        {
            ActivateVoiceChat();
        }
        Haptic(0.5f, 0.1f);
    }

    public void ActivateVoiceChat()
    {
        onVoice = true;
        localRecoder.TransmitEnabled = true;
        NetworkManager.Instance.onVoiceChat = true;
        //voiceChatButton.UpdateState(ButtonState.Activate);
    }

    public void DeactivateVoiceChat()
    {
        onVoice = false;
        localRecoder.TransmitEnabled = false;
        NetworkManager.Instance.onVoiceChat = false;
        //voiceChatButton.UpdateState(ButtonState.Deactivate);
    }

    public void RequsetVoiceChat(int receiverID, int senderID)
    {
        //photonView.RPC(nameof(RequestVoiceChatRPC), RpcTarget.All, receiverID, senderID);
    }

    //[PunRPC]
    //public void RequestVoiceChatRPC(int senderID, int recieverID)
    //{
    //    if (NetworkManager.User.id == recieverID)
    //    {
    //        voiceChatRequestToast.gameObject.SetActive(true);
    //        voiceChatRequestToast.SetToastMessage($"{senderID}의 음성 대화 초대");

    //        voiceChatRequestToast.accept.onClick.AddListener(() => AcceptVoiceChat(senderID, recieverID));
    //        voiceChatRequestToast.deaccept.onClick.AddListener(() => DeacceptVoiceChat(senderID, recieverID));
    //    }
    //}

    /// <summary>
    /// 로컬에서 1 : 1 음성채팅을 하는 두 사람을 제외한 모든 사람의 AudioSource르 꺼버림
    /// </summary>
    public void AcceptVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(AcceptVoiceChatRPC), RpcTarget.All, senderID, recieverID);
        voiceChatRequestToast.gameObject.SetActive(false);
    }

    [PunRPC]
    public void AcceptVoiceChatRPC(int senderID, int recieverID)
    {
        NetworkPlayer sender = FindTargetPlayer(senderID);
        NetworkPlayer reciever = FindTargetPlayer(recieverID);

        sender.OnVoiceChat = true;
        reciever.OnVoiceChat = true;

        if (NetworkManager.User.id == senderID || NetworkManager.User.id == recieverID)
        {
            voiceChatDistanceCheck = StartCoroutine(VoiceChatDistanceCheck(senderID, recieverID));
            Haptic(0.5f, 0.1f);
            localRecoder.TransmitEnabled = true;

            for (int i = 0; i < playerTransforms.childCount; i++)
            {
                NetworkPlayer other = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();

                //if (other.UserID != senderID && other.UserID != recieverID)
                //{
                //    other.VoiceOff();
                //}
            }
        }
        else
        {
            sender.VoiceOff();
            reciever.VoiceOff();
        }

        RemoveVoiceChatButtonEvent(recieverID);
    }

    public void DeacceptVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(DeacceptVoiceChatRPC), RpcTarget.All, senderID, recieverID);
        voiceChatRequestToast.gameObject.SetActive(false);
    }

    [PunRPC]
    public void DeacceptVoiceChatRPC(int senderID, int recieverID)
    {
        if (NetworkManager.User.id == senderID)
        {
            voiceChatDeacceptToast.gameObject.SetActive(true);
            voiceChatDeacceptToast.SetToastMessage($"{recieverID}가 음성 대화 거절");
        }
        RemoveVoiceChatButtonEvent(recieverID);
    }

    private void RemoveVoiceChatButtonEvent(int targetID)
    {
        if (NetworkManager.User.id == targetID)
        {
            //voiceChatRequestToast.accept.onClick.RemoveAllListeners();
            //voiceChatRequestToast.deaccept.onClick.RemoveAllListeners();
        }
    }

    public void DisconnectVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(DisconnectVoiceChatRPC), RpcTarget.All, senderID, recieverID);
    }

    [PunRPC]
    public void DisconnectVoiceChatRPC(int senderID, int recieverID)
    {
        NetworkPlayer sender = FindTargetPlayer(senderID);
        NetworkPlayer reciever = FindTargetPlayer(recieverID);

        sender.OnVoiceChat = false;
        reciever.OnVoiceChat = false;

        if (NetworkManager.User.id == senderID || NetworkManager.User.id == recieverID)
        {
            localRecoder.TransmitEnabled = false;
            Haptic(0.2f, 0.1f);

            if (voiceChatDistanceCheck != null)
            {
                StopCoroutine(voiceChatDistanceCheck);
                voiceChatDistanceCheck = null;
            }
            for (int i = 0; i < playerTransforms.childCount; i++)
            {
                NetworkPlayer other = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();

                //if (other.UserID != senderID && other.UserID != recieverID)
                //{
                //    if (!other.OnVoiceChat) other.VoiceOn();
                //}
            }
        }
        else
        {
            sender.VoiceOff();
            reciever.VoiceOff();
        }
    }

    Coroutine voiceChatDistanceCheck;
    private IEnumerator VoiceChatDistanceCheck(int senderID, int recieverID)
    {
        WaitForFixedUpdate delay = new WaitForFixedUpdate();
        float distance = 0f;
        do
        {
            distance = Vector3.Distance(player.transform.position, voiceChatTarget.position);
            yield return delay;
        }
        while (distance < 5f);

        DisconnectVoiceChat(senderID, recieverID);
    }
    #endregion

    #region Chat

    public delegate void ShowSpeechBubbleEvent(string text);
    public ShowSpeechBubbleEvent showSpeechBubble;

    [Header("Chat")]
    public GameObject virtualKeyboard;
    public GameObject chatUI;
    private bool onChatView;

    public TMP_InputField chatInputField;
    public TMP_Text[] chatList;

    public void ToggleTextChat()
    {
        if (onChatView)
        {
            virtualKeyboard.SetActive(false);
            chatUI.SetActive(false);

            onChatView = false;
            NetworkManager.Instance.onTextChat = false;
            //LoundgeSceneManager.Instance.Haptic(0.2f, 0.1f);
        }
        else
        {
            virtualKeyboard.SetActive(true);
            chatUI.SetActive(true);

            onChatView = true;
            NetworkManager.Instance.onTextChat = true;
            //LoundgeSceneManager.Instance.Haptic(0.5f, 0.1f);
        }

        //Haptic(0.5f, 0.1f);
    }

    public void DisableTextChat()
    {
        chatUI.SetActive(false);
        virtualKeyboard.SetActive(false);
        onChatView = false;
    }

    public void SendChatMessage(InputField input)
    {
        showSpeechBubble(input.text);
        ///LoundgeSceneManager.Instance.SendTextChat(chatInputField.text);
        chatInputField.text = "";
    }

    public void UpdateChat(string msg)
    {
        bool isInput = false;

        for (int i = 0; i < chatList.Length; i++)
        {
            if (chatList[i].text == "")
            {
                isInput = true;
                chatList[i].text = msg;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = 1; i < chatList.Length; i++)
            {
                chatList[i - 1].text = chatList[i].text;
            }

            chatList[chatList.Length - 1].text = msg;
        }

        showSpeechBubble?.Invoke(msg);
    }

    #endregion


    #region Enter Room

    #endregion

    private NetworkPlayer FindTargetPlayer(int id)
    {
        for (int i = 0; i < playerTransforms.childCount; i++)
        {
            NetworkPlayer player = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();

            //if (player.UserID == id)
            //{
            //    return player;
            //}
        }

        return null;
    }

    public void Haptic(float amplitude, float duration)
    {
        hapticTargetController.SendHapticImpulse(amplitude, duration);
    }
}
