using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NPCController : MonoBehaviourPun //, IPunInstantiateMagicCallback
{
    public delegate void EventMessage(string message);
    public EventMessage eventMessage;

    //[SerializeField] private int characterNumber;
    //public int CharacterNumber
    //{
    //    get => characterNumber;
    //    set => ActionRPC(nameof(SetCharacterNumber), value);
    //}
    //[PunRPC]
    //private void SetCharacterNumber(int value)
    //{
    //    characterNumber = value;
    //    if (!photonView.IsMine)
    //    {
    //        models[characterNumber].gameObject.SetActive(true);
    //    }

    //}

    //[SerializeField] private VoiceChatState voiceChatState;
    //public VoiceChatState VoiceChatState
    //{
    //    get => voiceChatState;
    //    set => ActionRPC(nameof(SetVoiceChatState), value);
    //}
    //[PunRPC]
    //private void SetVoiceChatState(VoiceChatState value)
    //{
    //    Debug.Log("Character State: " + value);
    //    voiceChatState = value;
    //}


    //private void ActionRPC(string functionName, object value)
    //{
    //    photonView.RPC(functionName, RpcTarget.All, value);
    //}

    [SerializeField] private GameObject[] models;

    [SerializeField] private Mesh[] meshs;
    [SerializeField] private Material[] materials;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererFemale;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererMale;

    private SkinnedMeshRenderer outline;
    [SerializeField] private SkinnedMeshRenderer outlineMale;
    [SerializeField] private SkinnedMeshRenderer outlineFemale;

    [SerializeField] private TMP_Text userName;
    [SerializeField] private GameObject lectureIcon;
    [SerializeField] private GameObject studentIcon;
    [SerializeField] private Image onVoiceChatIcon;

    public SpeachBubble speachBubble;

    public bool senderIsOnVoiceChat = false;
    public bool isVoiceChatReady = false;
    private bool isHovered = false;
    public LoundgeUser user;

    private void Start()
    {
        //if (photonView.IsMine)
        //{
        //    FindObjectOfType<VoiceManager>().Initialize(photonView.ViewID);
        //    string key = photonView.ViewID.ToString();

        //    string user = NetworkManager.User.email;

        //    Hashtable setValue = new Hashtable();
        //    setValue.Add(key, user);

        //    PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);

        //    GetComponent<CapsuleCollider>().enabled = false;
        //    models[characterNumber].SetActive(false);
        //}
    }


    //public void InvokeProperties()
    //{
    //    Debug.Log("Invoke Properties");
    //    CharacterNumber = CharacterNumber;
    //}

    public void Initialize(LoundgeUser userData)
    {
        if (userData.characterNumber < materials.Length / 2)
        {
            models[1].SetActive(false);
            models[0].SetActive(true);

            skinnedMeshRenderer = skinnedMeshRendererMale;
            outline = outlineMale;
        }
        else
        {
            models[0].SetActive(false);
            models[1].SetActive(true);

            skinnedMeshRenderer = skinnedMeshRendererFemale;
            outline = outlineFemale;
        }

        skinnedMeshRenderer.sharedMaterial = materials[userData.characterNumber];
        skinnedMeshRenderer.sharedMesh = meshs[userData.characterNumber];
        outline.sharedMesh = meshs[userData.characterNumber];

        userName.text = userData.name;

        switch (userData.userType)
        {
            case UserType.Lecture:
                lectureIcon.gameObject.SetActive(true);
                break;
            case UserType.Student:
                studentIcon.gameObject.SetActive(true);
                break;
        }

        if(userData.onVoiceChat)
        {
            onVoiceChatIcon.gameObject.SetActive(true);
        }

        user = userData;
    }


    //public void SetVoiceState(VoiceChatState state)
    //{
    //    VoiceChatState = state;
    //}
    public void OnRequestPrivateVoiceChat()
    {
        DataManager.Instance.FindLobbyUser(NetworkManager.User);
        LoundgeUser reciever = DataManager.Instance.GetUser(user.email);
        

        bool recieverIsOnVoiceChat = reciever.onVoiceChat;
        bool recieverIsOnRequestVoiceChat = reciever.onRequestVoiceChat;

        LoundgeUser sender = DataManager.Instance.GetUser(NetworkManager.LoundgeUser.email);

        bool senderIsOnVoiceChat = sender.onVoiceChat;
        bool senderIsOnRequestVoiceChat = sender.onRequestVoiceChat;
        if (isVoiceChatReady && isHovered && !senderIsOnVoiceChat && !senderIsOnRequestVoiceChat && !recieverIsOnRequestVoiceChat && !recieverIsOnVoiceChat)
        {
            string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.REQUEST}_{NetworkManager.User.email}_{user.email}";
            eventMessage?.Invoke(message);
        }
    }

    private Coroutine requestPrivateVoiceChat;
    private IEnumerator RequestPrivateVoiceChat()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        LoundgeUser reciever = null;
        while(reciever == null)
        {
            reciever = DataManager.Instance.GetUser(user.email);
            yield return delay;
        }

        bool recieverIsOnVoiceChat = reciever.onVoiceChat;
        bool recieverIsOnRequestVoiceChat = reciever.onRequestVoiceChat;

        LoundgeUser sender = null;
        while(sender == null)
        {
            sender = DataManager.Instance.GetUser(NetworkManager.LoundgeUser.email);
            yield return delay;
        }

        bool senderIsOnVoiceChat = sender.onVoiceChat;
        bool senderIsOnRequestVoiceChat = sender.onRequestVoiceChat;

        if (isVoiceChatReady && isHovered && !senderIsOnVoiceChat && !senderIsOnRequestVoiceChat && !recieverIsOnRequestVoiceChat && !recieverIsOnVoiceChat)
        {
            string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.REQUEST}_{NetworkManager.User.email}_{user.email}";
            eventMessage?.Invoke(message);
        }
    }

    public void OnVoiceChat(bool value)
    {
        onVoiceChatIcon.gameObject.SetActive(value);
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        OutlineEnable();
    }

    public void OnHoverExit()
    {
        isHovered = false;
        OutlineDisable();
    }


    public void OnSelect()
    {

    }

    public void SetVoiceChatState(bool value)
    {
        senderIsOnVoiceChat = value;
    }

    public void OutlineEnable()
    {
        outline.gameObject.SetActive(true);
    }

    public void OutlineDisable()
    {
        outline.gameObject.SetActive(false);
    }

    public void ShowBubble(string message)
    {
        Debug.Log(user.name +  " ShowChatBubble");
        if(speachTimer != null)
        {
            StopCoroutine(speachTimer);
        }
        speachTimer = null;
        if (gameObject.activeInHierarchy)
        {
            speachTimer = StartCoroutine(SpeachTimer(message));
        }
    }

    private Coroutine speachTimer;
    private IEnumerator SpeachTimer(string message)
    {
        speachBubble.ShowBubble(message);

        yield return new WaitForSeconds(5f);

        speachBubble.HideBubble();
    }
}
