using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }

    [SerializeField] private GameObject[] models;
    public GameObject selectTest;

    private void Start()
    {
        
    }


    public void InvokeProperties()
    {
        Debug.Log("Invoke Properties");
        CharacterNumber = CharacterNumber;
    }

    public void Initialize(User userData)
    {
        CharacterNumber = userData.characterNumber;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
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
