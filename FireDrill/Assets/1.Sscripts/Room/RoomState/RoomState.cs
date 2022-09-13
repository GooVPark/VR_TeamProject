using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class RoomState : MonoBehaviourPun
{
    public RoomSceneManager roomSceneManager;
    protected User user;

    [Header("UI")]
    [SerializeField] protected ButtonState megaphone;
    [SerializeField] protected ButtonState scoreBoard;
    [SerializeField] protected ButtonState voiceChat;
    [SerializeField] protected ButtonState textChat;

    private void Awake()
    {
    }

    public virtual void OnStateEnter()
    {
        gameObject.SetActive(true);

        roomSceneManager = FindObjectOfType<RoomSceneManager>();

        megaphone = roomSceneManager.megaPhoneButton;
        scoreBoard = roomSceneManager.scoreBoardButton;
        voiceChat = roomSceneManager.voiceChatButton;
        textChat = roomSceneManager.textChatButton;

        user = NetworkManager.User;
    }

    public virtual void OnStateExit()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnUpdate()
    {
        
    }
}
