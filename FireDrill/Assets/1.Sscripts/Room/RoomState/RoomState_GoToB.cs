using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_GoToB : RoomState
{
    [Header("Room State")]
    public RoomState_InQuiz roomStateInQuiz;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventArea;
    

    public override void OnStateEnter()
    {
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady(eventArea.playerCount))
        {
            roomSceneManager.RoomState = roomStateInQuiz;
        }
    }
}
