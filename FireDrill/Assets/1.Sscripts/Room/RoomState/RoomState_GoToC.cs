using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_GoToC : RoomState
{
    [Header("Room State")]
    public RoomState_InTraning roomStateInTraning;
    [Space(5)]

    [Header("Toasts")]
    public Toast toast;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventAreaMR;
    public EventArea eventArea;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        toast.gameObject.SetActive(true);
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady(eventAreaMR.playerCount) && roomSceneManager.IsReady(eventArea.playerCount))
        {
            roomSceneManager.RoomState = roomStateInTraning;
        }
    }
}
