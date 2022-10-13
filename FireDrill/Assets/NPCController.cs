using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using SelectedEffectOutline;

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
    public GameObject selectTest;

    public Outline outline;
    private Dictionary<int, OutlineNormalsCalculator[]> outlines = new Dictionary<int, OutlineNormalsCalculator[]>();

    public bool isVoiceChatReady = false;
    private bool isHovered = false;
    public LoundgeUser user;

    private void Start()
    {
        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);
        outline = GetComponentInChildren<Outline>();

        for(int i = 0; i < models.Length; i++)
        {
            OutlineNormalsCalculator[] oncs = models[i].GetComponentsInChildren<OutlineNormalsCalculator>(true);
            outlines.Add(i, oncs);
        }

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
        models[userData.characterNumber].SetActive(true);
        user = userData;
    }


    //public void SetVoiceState(VoiceChatState state)
    //{
    //    VoiceChatState = state;
    //}
    public void OnRequestPrivateVoiceChat()
    {
        if (!user.onVoiceChat && isVoiceChatReady && isHovered)
        {
            string message = $"{EventMessageType.VOICECHAT}_{VoiceEventType.REQUEST}_{NetworkManager.User.email}_{user.email}";
            eventMessage?.Invoke(message);
        }
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        if (!user.onVoiceChat && isVoiceChatReady)
        {
            OutlineEnable();
        }
    }

    public void OnHoverExit()
    {
        isHovered = false;
        OutlineDisable();
    }


    public void OnSelect()
    {

    }


    public void OutlineEnable()
    {
        foreach (OutlineNormalsCalculator outline in outlines[user.characterNumber])
        {
            outline.gameObject.SetActive(true);
        }
    }

    public void OutlineDisable()
    {
        foreach (OutlineNormalsCalculator outline in outlines[user.characterNumber])
        {
            outline.gameObject.SetActive(false);
        }
    }
}
