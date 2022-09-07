using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_GoToA : RoomState
{
    [Header("Room State")]
    public RoomState_InClass roomStateInClass;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventArea;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;

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
        if(roomSceneManager.IsReady(eventArea.playerCount))
        {
            roomSceneManager.RoomState = roomStateInClass;
        }
    }
}
