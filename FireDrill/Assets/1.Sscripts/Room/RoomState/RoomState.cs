using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class RoomState : MonoBehaviourPun
{
    public RoomSceneManager roomSceneManager;

    public virtual void OnStateEnter()
    {
        gameObject.SetActive(true);
        roomSceneManager = FindObjectOfType<RoomSceneManager>();
    }

    public virtual void OnStateExit()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnUpdate()
    {
        
    }
}
