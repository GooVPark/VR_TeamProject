using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using SelectedEffectOutline;

public class NPCController : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private int characterNumber;
    public int CharacterNumber
    {
        get => characterNumber;
        set => ActionRPC(nameof(SetCharacterNumber), value);
    }
    [PunRPC]
    private void SetCharacterNumber(int value)
    {
        characterNumber = value;
        models[characterNumber].gameObject.SetActive(true);
    }

    [SerializeField] private VoiceChatState voiceChatState;
    public VoiceChatState VoiceChatState
    {
        get => voiceChatState;
        set => ActionRPC(nameof(SetVoiceChatState), value);
    }
    [PunRPC]
    private void SetVoiceChatState(VoiceChatState value)
    {
        voiceChatState = value;
    }


    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }

    [SerializeField] private GameObject[] models;
    public GameObject selectTest;

    public Outline outline;

    public bool isVoiceChatReady = false;

    private void Start()
    {
        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);
        outline = GetComponentInChildren<Outline>();

        if (photonView.IsMine)
        {
            FindObjectOfType<VoiceManager>().Initialize(photonView.ViewID);
            string key = photonView.ViewID.ToString();

            string user = NetworkManager.User.email;

            Hashtable setValue = new Hashtable();
            setValue.Add(key, user);

            PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);

            GetComponent<CapsuleCollider>().enabled = false;
            models[characterNumber].SetActive(false);
        }
    }


    public void InvokeProperties()
    {
        Debug.Log("Invoke Properties");
        CharacterNumber = CharacterNumber;
    }

    public void Initialize(User userData)
    {
        if(photonView.IsMine)
        {
            FindObjectOfType<VoiceManager>().localPlayer = this;
        }
        CharacterNumber = userData.characterNumber;
        VoiceChatState = VoiceChatState.Off;
    }


    public void SetVoiceState(VoiceChatState state)
    {
        VoiceChatState = state;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hover Enter");
        outline.m_OverlayFlash = true;
    }

    public void OnHoverExit()
    {
        Debug.Log("Hover Exit");
        outline.m_OverlayFlash = false;
    }


    public void OnSelect()
    {
        photonView.RPC(nameof(OnSelectRPC), RpcTarget.All, photonView.ViewID);
        selectTest.SetActive(true);
    }

    [PunRPC]
    private void OnSelectRPC(int senderViewID)
    {
        if(photonView.IsMine)
        {
            selectTest.SetActive(true);
        }
    }
}
