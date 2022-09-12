using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class RoomState : MonoBehaviourPun
{
    public RoomSceneManager roomSceneManager;
    protected User user;

    public virtual void OnStateEnter()
    {
        gameObject.SetActive(true);
        roomSceneManager = FindObjectOfType<RoomSceneManager>();
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
