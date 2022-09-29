using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPCController : MonoBehaviourPun
{
    [SerializeField] private GameObject[] models;

    [SerializeField] private int characterNumber;
    public int CharacterNumber
    {
        get => characterNumber;
        set => ActionRPC(nameof(SetCharacterNumber), value); 
    }
    private void SetCharacterNumber(int value)
    {
        characterNumber = value;
        models[characterNumber].gameObject.SetActive(true); 
    }
    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }


    public void InvokeProperties()
    {
        CharacterNumber = CharacterNumber;
    }

    public void Initialize(User userData)
    {
       
    }
}
