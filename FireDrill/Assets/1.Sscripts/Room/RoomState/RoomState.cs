using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomState : MonoBehaviour
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
