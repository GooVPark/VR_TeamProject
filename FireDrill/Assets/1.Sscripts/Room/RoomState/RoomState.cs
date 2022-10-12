using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class RoomState : MonoBehaviourPun
{
    public RoomSceneManager roomSceneManager;
    protected User user;

    [Header("UI")]
    [SerializeField] protected ButtonStateHandler megaphone;
    [SerializeField] protected ButtonStateHandler scoreBoard;
    [SerializeField] protected ButtonStateHandler voiceChat;
    [SerializeField] protected ButtonStateHandler textChat;

    private void Awake()
    {
    }

    public virtual void OnStateEnter()
    {
        gameObject.SetActive(true);

        roomSceneManager = FindObjectOfType<RoomSceneManager>();

        megaphone = roomSceneManager.megaphoneButton;
        scoreBoard = roomSceneManager.scoreboardButton;
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
