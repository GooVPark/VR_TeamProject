using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;

public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    private Recorder recorder;

    private void Awake()
    {
        InputManager.PrimaryReaded += GetPrimaryValue;
    }

    private void Start()
    {
        recorder = GetComponent<Recorder>();
    }    

    private void GetPrimaryValue(bool value)
    {
        recorder.TransmitEnabled = value;
    }
}


