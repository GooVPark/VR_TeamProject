using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_StartQuiz : RoomState
{
    [Header("Room State")]
    public RoomState_Quiz roomStateQuiz;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
